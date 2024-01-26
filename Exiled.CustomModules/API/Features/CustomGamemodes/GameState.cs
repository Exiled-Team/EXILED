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

    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.CustomModules.API.Enums;
    using Exiled.Events.EventArgs.Player;

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
        public virtual void Start()
        {
            foreach (PlayerState ps in PlayerStates)
                ps.Deploy();
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

            foreach (Player player in Player.List)
            {
                if (player.HasComponent(PlayerStateComponent, true))
                    continue;

                player.AddComponent(PlayerStateComponent);
            }
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.Verified += OnVerifiedInternal;
            Exiled.Events.Handlers.Player.Destroying += OnDestroyingInternal;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.Verified -= OnVerifiedInternal;
            Exiled.Events.Handlers.Player.Destroying -= OnDestroyingInternal;
        }

        private void OnVerifiedInternal(VerifiedEventArgs ev)
        {
            if (ev.Player.HasComponent(PlayerStateComponent, true))
                return;

            ev.Player.AddComponent(PlayerStateComponent);
        }

        private void OnDestroyingInternal(DestroyingEventArgs ev)
        {
            if (!ev.Player.TryGetComponent(PlayerStateComponent, out EActor comp))
                return;

            comp.Destroy();
        }
    }
}