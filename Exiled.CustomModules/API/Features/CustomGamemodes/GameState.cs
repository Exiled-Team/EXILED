// -----------------------------------------------------------------------
// <copyright file="GameState.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomGameModes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.Doors;
    using Exiled.CustomModules.API.Enums;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;

    using GameCore;
    using Interactables.Interobjects.DoorUtils;
    using LightContainmentZoneDecontamination;
    using Respawning;
    using UnityStandardAssets.CinematicEffects;
    using Utils.Networking;

    /// <summary>
    /// Represents the state of the game on the server within the custom game mode, derived from <see cref="EActor"/> and implementing <see cref="IAdditiveSettings{T}"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="GameState"/> class encapsulates the current state of the game during rounds within the custom game mode.
    /// </para>
    /// <para>
    /// It serves as the foundation for defining and enforcing rules, conditions, and settings specific to the custom game mode on the server.
    /// </para>
    /// </remarks>
    public abstract class GameState : EActor, IAdditiveSettings<GameModeSettings>
    {
        private readonly List<PlayerState> playerStates = new();
        private Type cachedPlayerStateType;

        /// <summary>
        /// Gets the relative <see cref="CustomGameModes.CustomGameMode"/>.
        /// </summary>
        public CustomGameMode CustomGameMode { get; private set; }

        /// <summary>
        /// Gets the <see cref="PlayerState"/> associated to the current <see cref="CustomGameMode"/>.
        /// </summary>
        public Type PlayerStateComponent => cachedPlayerStateType ??= CustomGameMode.BehaviourComponents.FirstOrDefault(t => typeof(PlayerState).IsAssignableFrom(t));

        /// <summary>
        /// Gets or sets the settings associated with the custom game mode.
        /// </summary>
        public GameModeSettings Settings { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="GameState"/>'s config.
        /// </summary>
        public object Config { get; set; }

        /// <summary>
        /// Gets all <see cref="PlayerState"/> instances.
        /// </summary>
        public IEnumerable<PlayerState> PlayerStates => playerStates;

        /// <summary>
        /// Gets or sets the <see cref="UEMatchState"/>.
        /// </summary>
        public UEMatchState MatchState { get; set; }

        /// <summary>
        /// Gets the time elapsed from the start of the round.
        /// </summary>
        public TimeSpan ElapsedTime => Round.ElapsedTime;

        /// <summary>
        /// Gets a value indicating whether the game state can be ended.
        /// </summary>
        public bool CanBeEnded => EvaluateEndingConditions();

        /// <summary>
        /// Gets the current player count.
        /// </summary>
        public int PlayerCount => playerStates.Count;

        /// <inheritdoc/>
        public virtual void AdjustAdditivePipe()
        {
            if (CustomGameMode.TryGet(GetType(), out CustomGameMode customGameMode))
                CustomGameMode = customGameMode;

            Settings = CustomGameMode.Settings;

            if (Config is null)
                return;

            foreach (PropertyInfo propertyInfo in Config.GetType().GetProperties())
            {
                PropertyInfo targetInfo = Config.GetType().GetProperty(propertyInfo.Name);
                if (targetInfo is null)
                    continue;

                targetInfo.SetValue(Settings, propertyInfo.GetValue(Config, null));
            }
        }

        /// <summary>
        /// Adds a new <see cref="PlayerState"/> to <see cref="PlayerStates"/>.
        /// </summary>
        /// <param name="playerState">The <see cref="PlayerState"/> to add.</param>
        public virtual void AddPlayerState(PlayerState playerState) => playerStates.Add(playerState);

        /// <summary>
        /// Removes a new <see cref="PlayerState"/> to <see cref="PlayerStates"/>.
        /// </summary>
        /// <param name="playerState">The <see cref="PlayerState"/> to remove.</param>
        public virtual void RemovePlayerState(PlayerState playerState) => playerStates.Remove(playerState);

        /// <summary>
        /// Starts the <see cref="GameState"/>.
        /// </summary>
        /// <param name="isForced">A value indicating whether the <see cref="GameState"/> should be started regardless any conditions.</param>
        public virtual void Start(bool isForced = false)
        {
            if (!isForced && PlayerCount < Settings.MinimumPlayers)
                return;

            foreach (PlayerState ps in PlayerStates)
                ps.Deploy();
        }

        /// <summary>
        /// Ends the <see cref="GameState"/>.
        /// </summary>
        /// <param name="isForced">A value indicating whether the <see cref="GameState"/> should be ended regardless any conditions.</param>
        public virtual void End(bool isForced = false)
        {
            if (isForced || CanBeEnded)
            {
                foreach (PlayerState ps in PlayerStates)
                    ps.Destroy();
            }
        }

        /// <summary>
        /// Defines and evaluates the ending conditions.
        /// </summary>
        /// <returns><see langword="true"/> if the current game state matches the defined ending conditions; otherwise, <see langword="false"/>.</returns>
        protected abstract bool EvaluateEndingConditions();

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            RespawnManager.Singleton._started = Settings.IsTeamRespawnEnabled;

            if (Settings.TeamRespawnTime > 0f)
                RespawnManager.Singleton.TimeTillRespawn = Settings.TeamRespawnTime;

            if (!Settings.IsDecontaminationEnabled)
                DecontaminationController.Singleton.NetworkDecontaminationOverride = DecontaminationController.DecontaminationStatus.Disabled;

            Warhead.IsLocked = Settings.IsWarheadEnabled;
            Warhead.AutoDetonate = Settings.AutoWarheadTime > 0f;
            Warhead.AutoDetonateTime = Settings.AutoWarheadTime;

            foreach (Player player in Player.List)
            {
                if (player.HasComponent(PlayerStateComponent, true))
                    continue;

                player.AddComponent(PlayerStateComponent);
            }
        }

        /// <inheritdoc/>
        protected override void OnBeginPlay()
        {
            base.OnBeginPlay();

            Door.LockAll(Settings.LockedZones);
            Door.LockAll(Settings.LockedDoors);
            Lift.LockAll(Settings.LockedElevators);
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Server.EndingRound += OnEndingRound;
            Exiled.Events.Handlers.Server.RespawningTeam += OnRespawningTeamInternal;
            Exiled.Events.Handlers.Server.RespawnedTeam += OnRespawnedTeamInternal;

            Exiled.Events.Handlers.Player.PreAuthenticating += OnPreAuthenticatingInternal;
            Exiled.Events.Handlers.Player.Verified += OnVerifiedInternal;
            Exiled.Events.Handlers.Player.Destroying += OnDestroyingInternal;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Server.EndingRound -= OnEndingRound;
            Exiled.Events.Handlers.Server.RespawningTeam -= OnRespawningTeamInternal;
            Exiled.Events.Handlers.Server.RespawnedTeam -= OnRespawnedTeamInternal;

            Exiled.Events.Handlers.Player.PreAuthenticating -= OnPreAuthenticatingInternal;
            Exiled.Events.Handlers.Player.Verified -= OnVerifiedInternal;
            Exiled.Events.Handlers.Player.Destroying -= OnDestroyingInternal;
        }

        private void OnEndingRound(EndingRoundEventArgs ev)
        {
            if (!Settings.UseCustomEndingConditions || !EvaluateEndingConditions())
                return;

            ev.IsAllowed = true;
        }

        private void OnRespawningTeamInternal(RespawningTeamEventArgs ev)
        {
            ev.IsAllowed = (!Settings.SpawnableTeams.IsEmpty() && !Settings.SpawnableTeams.Contains(ev.NextKnownTeam)) ||
                           (!Settings.NonSpawnableTeams.IsEmpty() && Settings.NonSpawnableTeams.Contains(ev.NextKnownTeam));
        }

        private void OnRespawnedTeamInternal(RespawnedTeamEventArgs ev)
        {
            RespawnManager.Singleton.TimeTillRespawn = Settings.TeamRespawnTime;
        }

        private void OnPreAuthenticatingInternal(PreAuthenticatingEventArgs ev)
        {
            if (PlayerCount == Settings.MaximumPlayers && Settings.RejectExceedingPlayers)
                ev.Reject(Settings.RejectExceedingMessage, true);
        }

        private void OnVerifiedInternal(VerifiedEventArgs ev)
        {
            if (ev.Player.HasComponent(PlayerStateComponent, true))
                return;

            EActor component = EObject.CreateDefaultSubobject(PlayerStateComponent, ev.Player.GameObject).Cast<EActor>();

            if(PlayerCount == Settings.MaximumPlayers)
                component.As<PlayerState>().IsRespawnable = false;

            ev.Player.AddComponent(component);
        }

        private void OnDestroyingInternal(DestroyingEventArgs ev)
        {
            if (!ev.Player.TryGetComponent(PlayerStateComponent, out EActor comp))
                return;

            comp.Destroy();
        }
    }
}