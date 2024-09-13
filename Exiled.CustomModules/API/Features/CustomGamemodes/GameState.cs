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
    using System.Diagnostics;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Roles;
    using Exiled.API.Interfaces;
    using Exiled.CustomModules.API.Enums;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.CustomModules.API.Features.Generic;
    using Exiled.CustomModules.Events.EventArgs.CustomRoles;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Server;
    using LightContainmentZoneDecontamination;
    using PlayerRoles;
    using Respawning;

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
        private GameModeSettings settings;
        private ModulePointer config;

        /// <summary>
        /// Gets the relative <see cref="CustomGameModes.CustomGameMode"/>.
        /// </summary>
        public CustomGameMode CustomGameMode { get; private set; }

        /// <summary>
        /// Gets the <see cref="PlayerState"/> associated to the current <see cref="CustomGameMode"/>.
        /// </summary>
        public Type PlayerStateComponent => cachedPlayerStateType ??=
            CustomGameMode.BehaviourComponents.FirstOrDefault(t => typeof(PlayerState).IsAssignableFrom(t));

        /// <summary>
        /// Gets or sets the settings associated with the custom game mode.
        /// </summary>
        public GameModeSettings Settings
        {
            get => settings ??= CustomGameMode.Settings;
            set => settings = value;
        }

        /// <summary>
        /// Gets or sets the game state's configs.
        /// </summary>
        public virtual ModulePointer Config
        {
            get => config ??= CustomGameMode.Config;
            protected set => config = value;
        }

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

            ModuleBehaviour<GameEntity>.ImplementConfigs_DefaultImplementation(this, Config);

            if (CustomGameMode is null || Settings is null || Config is null)
            {
                Log.Error($"Custom game mode ({GetType().Name}) has invalid configuration.");
                Destroy();
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
            if (!typeof(World).IsAssignableFrom(new StackTrace().GetFrame(1)?.GetMethod()?.DeclaringType))
                throw new Exception("Only the World can start a GameState.");

            if (!isForced && PlayerCount < Settings.MinimumPlayers)
                return;

            World.Get().RunningGameMode = CustomGameMode.Get(this).Id;

            foreach (PlayerState ps in PlayerStates)
                ps.Deploy();
        }

        /// <summary>
        /// Ends the <see cref="GameState"/>.
        /// </summary>
        /// <param name="isForced">A value indicating whether the <see cref="GameState"/> should be ended regardless any conditions.</param>
        public virtual void End(bool isForced = false)
        {
            if (!typeof(GameState).IsAssignableFrom(new StackTrace().GetFrame(1)?.GetMethod()?.DeclaringType))
                throw new Exception("Only the World can end a GameState.");

            if (!isForced && !CanBeEnded)
                return;

            World.Get().RunningGameMode = UUGameModeType.None;

            foreach (PlayerState ps in PlayerStates)
                ps.Destroy();
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

        /// <inheritdoc />
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            Door.UnlockAll();
            Lift.UnlockAll();
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            StaticActor.Get<RoleAssigner>().AssigningHumanCustomRolesDispatcher.Bind(this, OnAssigningCustomHumanRolesInternal);
            StaticActor.Get<RoleAssigner>().AssigningScpCustomRolesDispatcher.Bind(this, OnAssigningCustomScpRolesInternal);

            StaticActor.Get<Features.RespawnManager>().SelectingCustomTeamRespawnDispatcher.Bind(this, OnSelectingCustomTeamRespawnInternal);

            Exiled.Events.Handlers.Server.AssigningHumanRoles += OnAssigningHumanRolesInternal;
            Exiled.Events.Handlers.Server.AssigningScpRoles += OnAssigningScpRolesInternal;
            Exiled.Events.Handlers.Server.EndingRound += OnEndingRoundInternal;
            Exiled.Events.Handlers.Server.SelectingRespawnTeam += OnSelectingRespawnTeamInternal;
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

            StaticActor.Get<RoleAssigner>().AssigningHumanCustomRolesDispatcher.Unbind(this);
            StaticActor.Get<Features.RespawnManager>().SelectingCustomTeamRespawnDispatcher.Unbind(this);

            Exiled.Events.Handlers.Server.AssigningHumanRoles -= OnAssigningHumanRolesInternal;
            Exiled.Events.Handlers.Server.AssigningScpRoles -= OnAssigningScpRolesInternal;
            Exiled.Events.Handlers.Server.EndingRound -= OnEndingRoundInternal;
            Exiled.Events.Handlers.Server.SelectingRespawnTeam -= OnSelectingRespawnTeamInternal;
            Exiled.Events.Handlers.Server.RespawningTeam -= OnRespawningTeamInternal;
            Exiled.Events.Handlers.Server.RespawnedTeam -= OnRespawnedTeamInternal;

            Exiled.Events.Handlers.Player.PreAuthenticating -= OnPreAuthenticatingInternal;
            Exiled.Events.Handlers.Player.Verified -= OnVerifiedInternal;
            Exiled.Events.Handlers.Player.Destroying -= OnDestroyingInternal;
        }

        private void OnAssigningHumanRolesInternal(AssigningHumanRolesEventArgs ev)
        {
            if (!Settings.SpawnableRoles.IsEmpty())
            {
                IEnumerable<RoleTypeId> roles = ev.Roles.Where(role => Settings.SpawnableRoles.Contains(role));
                int amount = roles.Count();
                if (amount < ev.Roles.Length)
                {
                    List<RoleTypeId> newRoles = new();

                    for (int i = 0; i < amount; i++)
                        newRoles.Add(Role.Random(false, Settings.SpawnableRoles.Except(ev.Roles)));

                    ev.Roles = roles.Concat(newRoles).ToArray();
                }

                return;
            }

            if (!Settings.NonSpawnableRoles.IsEmpty())
            {
                IEnumerable<RoleTypeId> roles = ev.Roles.Where(role => !Settings.NonSpawnableRoles.Contains(role));
                int amount = roles.Count();
                if (amount < ev.Roles.Length)
                {
                    List<RoleTypeId> newRoles = new();

                    for (int i = 0; i < amount; i++)
                        newRoles.Add(Role.Random(false, Settings.NonSpawnableRoles));

                    ev.Roles = roles.Concat(newRoles).ToArray();
                }
            }
        }

        private void OnAssigningScpRolesInternal(AssigningScpRolesEventArgs ev)
        {
            if (!Settings.SpawnableRoles.IsEmpty())
            {
                IEnumerable<RoleTypeId> roles = ev.Roles.Where(role => Settings.SpawnableRoles.Contains(role));
                int amount = roles.Count();
                if (amount < ev.Roles.Count)
                {
                    List<RoleTypeId> newRoles = new();

                    for (int i = 0; i < amount; i++)
                        newRoles.Add(Role.Random(false, Settings.SpawnableRoles.Except(ev.Roles)));

                    ev.Roles = roles.Concat(newRoles).ToList();
                }

                return;
            }

            if (!Settings.NonSpawnableRoles.IsEmpty())
            {
                IEnumerable<RoleTypeId> roles = ev.Roles.Where(role => !Settings.NonSpawnableRoles.Contains(role));
                int amount = roles.Count();
                if (amount < ev.Roles.Count)
                {
                    List<RoleTypeId> newRoles = new();

                    for (int i = 0; i < amount; i++)
                        newRoles.Add(Role.Random(false, Settings.NonSpawnableRoles));

                    ev.Roles = roles.Concat(newRoles).ToList();
                }
            }
        }

        private void OnAssigningCustomHumanRolesInternal(Events.EventArgs.CustomRoles.AssigningHumanCustomRolesEventArgs ev)
        {
            if (!Settings.SpawnableCustomRoles.IsEmpty())
            {
                List<uint> customRoles = new();
                foreach (object role in ev.Roles)
                {
                    if (!CustomRole.TryGet(role, out CustomRole cr) || !Settings.SpawnableCustomRoles.Contains(cr.Id))
                        continue;

                    customRoles.Add(cr.Id);
                }

                ev.Roles.RemoveAll(o => CustomRole.TryGet(o, out CustomRole cr) && !Settings.SpawnableCustomRoles.Contains(cr.Id));

                int amount = customRoles.Count();
                if (amount < (ev.Roles.Count - customRoles.Count))
                {
                    List<uint> newRoles = new();

                    for (int i = 0; i < amount; i++)
                        newRoles.Add(CustomRole.Get(role => !role.IsScp && !role.IsTeamUnit).Random().Id);

                    ev.Roles = ev.Roles.Where(role => role is not RoleTypeId).Cast<object>().Concat(newRoles.Cast<object>()).ToList();
                }

                return;
            }

            if (!Settings.NonSpawnableCustomRoles.IsEmpty())
            {
                List<uint> customRoles = new();
                foreach (object role in ev.Roles)
                {
                    if (!CustomRole.TryGet(role, out CustomRole cr) || Settings.NonSpawnableCustomRoles.Contains(cr.Id))
                        continue;

                    customRoles.Add(cr.Id);
                }

                ev.Roles.RemoveAll(o => CustomRole.TryGet(o, out CustomRole cr) && Settings.NonSpawnableCustomRoles.Contains(cr.Id));

                int amount = customRoles.Count();
                if (amount < (ev.Roles.Count - customRoles.Count))
                {
                    List<uint> newRoles = new();

                    for (int i = 0; i < amount; i++)
                        newRoles.Add(CustomRole.Get(role => !role.IsScp && !role.IsTeamUnit).Random().Id);

                    ev.Roles = ev.Roles.Where(role => role is not RoleTypeId).Cast<object>().Concat(newRoles.Cast<object>()).ToList();
                }
            }
        }

        private void OnAssigningCustomScpRolesInternal(Events.EventArgs.CustomRoles.AssigningScpCustomRolesEventArgs ev)
        {
            if (!Settings.SpawnableCustomRoles.IsEmpty())
            {
                List<uint> customRoles = new();
                foreach (object role in ev.Roles)
                {
                    if (!CustomRole.TryGet(role, out CustomRole cr) || !Settings.SpawnableCustomRoles.Contains(cr.Id))
                        continue;

                    customRoles.Add(cr.Id);
                }

                ev.Roles.RemoveAll(o => CustomRole.TryGet(o, out CustomRole cr) && !Settings.SpawnableCustomRoles.Contains(cr.Id));

                int amount = customRoles.Count();
                if (amount < (ev.Roles.Count - customRoles.Count))
                {
                    List<uint> newRoles = new();

                    for (int i = 0; i < amount; i++)
                        newRoles.Add(CustomRole.Get(role => !role.IsScp && !role.IsTeamUnit).Random().Id);

                    ev.Roles = ev.Roles.Where(role => role is not RoleTypeId).Cast<object>().Concat(newRoles.Cast<object>()).ToList();
                }

                return;
            }

            if (!Settings.NonSpawnableCustomRoles.IsEmpty())
            {
                List<uint> customRoles = new();
                foreach (object role in ev.Roles)
                {
                    if (!CustomRole.TryGet(role, out CustomRole cr) || Settings.NonSpawnableCustomRoles.Contains(cr.Id))
                        continue;

                    customRoles.Add(cr.Id);
                }

                ev.Roles.RemoveAll(o => CustomRole.TryGet(o, out CustomRole cr) && Settings.NonSpawnableCustomRoles.Contains(cr.Id));

                int amount = customRoles.Count();
                if (amount < (ev.Roles.Count - customRoles.Count))
                {
                    List<uint> newRoles = new();

                    for (int i = 0; i < amount; i++)
                        newRoles.Add(CustomRole.Get(role => role.IsScp && !role.IsTeamUnit).Random().Id);

                    ev.Roles = ev.Roles.Where(role => role is not RoleTypeId).Cast<object>().Concat(newRoles.Cast<object>()).ToList();
                }
            }
        }

        private void OnEndingRoundInternal(EndingRoundEventArgs ev)
        {
            if (!Settings.UseCustomEndingConditions || !EvaluateEndingConditions())
                return;

            ev.IsAllowed = true;
        }

        private void OnSelectingRespawnTeamInternal(SelectingRespawnTeamEventArgs ev)
        {
            if (!Settings.SpawnableTeams.IsEmpty() && Settings.SpawnableTeams.Contains(ev.Team))
            {
                if (ev.Team is SpawnableTeamType.ChaosInsurgency && !Settings.SpawnableTeams.Contains(SpawnableTeamType.NineTailedFox))
                {
                    ev.Team = SpawnableTeamType.NineTailedFox;
                    return;
                }

                if (ev.Team is SpawnableTeamType.NineTailedFox && !Settings.SpawnableTeams.Contains(SpawnableTeamType.ChaosInsurgency))
                {
                    ev.Team = SpawnableTeamType.ChaosInsurgency;
                    return;
                }

                ev.Team = SpawnableTeamType.None;
                return;
            }

            if (!Settings.NonSpawnableTeams.IsEmpty() && Settings.NonSpawnableTeams.Contains(ev.Team))
            {
                if (ev.Team is SpawnableTeamType.ChaosInsurgency && !Settings.NonSpawnableTeams.Contains(SpawnableTeamType.NineTailedFox))
                {
                    ev.Team = SpawnableTeamType.NineTailedFox;
                    return;
                }

                if (ev.Team is SpawnableTeamType.NineTailedFox && !Settings.NonSpawnableTeams.Contains(SpawnableTeamType.ChaosInsurgency))
                {
                    ev.Team = SpawnableTeamType.ChaosInsurgency;
                    return;
                }

                ev.Team = SpawnableTeamType.None;
            }
        }

        private void OnSelectingCustomTeamRespawnInternal(SelectingCustomTeamRespawnEventArgs ev)
        {
            if (!CustomTeam.TryGet(ev.Team, out CustomTeam team))
                return;

            if (!Settings.SpawnableCustomTeams.IsEmpty() && Settings.SpawnableCustomTeams.Contains(team.Id))
            {
                CustomTeam newTeam = CustomTeam.Get(t => Settings.SpawnableCustomTeams.Contains(t.Id)).Shuffle().FirstOrDefault();
                ev.Team = newTeam ? newTeam.Id : SpawnableTeamType.None;
                return;
            }

            if (!Settings.NonSpawnableCustomTeams.IsEmpty() && Settings.NonSpawnableCustomTeams.Contains(team.Id))
            {
                CustomTeam newTeam = CustomTeam.Get(t => !Settings.NonSpawnableCustomTeams.Contains(t.Id)).Shuffle().FirstOrDefault();
                ev.Team = newTeam ? newTeam.Id : SpawnableTeamType.None;
            }
        }

        private void OnRespawningTeamInternal(RespawningTeamEventArgs ev)
        {
            if ((!Settings.SpawnableTeams.IsEmpty() && !Settings.SpawnableTeams.Contains(ev.NextKnownTeam)) ||
                (!Settings.NonSpawnableTeams.IsEmpty() && Settings.NonSpawnableTeams.Contains(ev.NextKnownTeam)))
                ev.IsAllowed = false;
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