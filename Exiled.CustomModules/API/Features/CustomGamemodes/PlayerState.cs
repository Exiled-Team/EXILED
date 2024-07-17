// -----------------------------------------------------------------------
// <copyright file="PlayerState.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomGameModes
{
    using System;
    using System.Diagnostics;

    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.EventArgs.Warhead;
    using MEC;
    using PlayerRoles;

    /// <summary>
    /// Represents the state of an individual player within the custom game mode, derived from <see cref="ModuleBehaviour{T}"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="PlayerState"/> class manages the behavior and interactions of a single player within the custom game mode.
    /// </para>
    /// <para>
    /// It serves as a base class for defining player-specific actions and responses within the context of the custom game mode.
    /// </para>
    /// </remarks>
    public abstract class PlayerState : ModuleBehaviour<Player>
    {
        private bool isActive;
        private CoroutineHandle onReadyHandle;

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all delegates to be fired after the <see cref="PlayerState"/> has been deployed.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<PlayerState> DeployedDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all delegates to be fired after the <see cref="PlayerState"/> has been activated.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<PlayerState> ActivatedDispatcher { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all delegates to be fired after the <see cref="PlayerState"/> has been deactivated.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<PlayerState> DeactivatedDispatcher { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PlayerState"/> can behave regularly.
        /// </summary>
        public virtual bool IsActive
        {
            get => isActive;
            set
            {
                if (value == isActive)
                    return;

                if (value)
                {
                    isActive = value;
                    ActivatedDispatcher.InvokeAll(this);
                    return;
                }

                isActive = false;
                DeactivatedDispatcher.InvokeAll(this);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PlayerState"/> is respawnable.
        /// </summary>
        public virtual bool IsRespawnable { get; set; }

        /// <summary>
        /// Gets the respawn time for individual players.
        /// </summary>
        public float RespawnTime => World.Get().GameState.Settings.RespawnTime;

        /// <summary>
        /// Gets or sets the <see cref="PlayerState"/> score.
        /// </summary>
        public int Score { get; protected set; }

        /// <summary>
        /// Gets or sets the last time the player died.
        /// </summary>
        public DateTime LastDeath { get; protected set; }

        /// <summary>
        /// Gets a value indicating whether the player is ready to respawn.
        /// </summary>
        public virtual bool CanRespawn => DateTime.Now > LastDeath + TimeSpan.FromSeconds(RespawnTime);

        /// <summary>
        /// Gets or sets the remaining respawn time.
        /// </summary>
        public virtual float RemainingRespawnTime
        {
            get => (float)Math.Max(0f, (LastDeath + TimeSpan.FromSeconds(RespawnTime) - DateTime.Now).TotalSeconds);
            set => LastDeath = LastDeath.AddSeconds(value);
        }

        /// <summary>
        /// Forces the respawn procedure.
        /// </summary>
        /// <returns>The new <see cref="LastDeath"/> value, representing the updated death timestamp.</returns>
        public virtual DateTime ForceRespawn()
        {
            Respawn();
            LastDeath = DateTime.Now - TimeSpan.FromSeconds(RespawnTime + 1);
            return LastDeath;
        }

        /// <summary>
        /// Resets the respawn procedure.
        /// </summary>
        /// <returns>The new <see cref="LastDeath"/> value, representing the updated death timestamp.</returns>
        public virtual DateTime ResetRespawn()
        {
            if (onReadyHandle.IsRunning)
                Timing.KillCoroutines(onReadyHandle);

            onReadyHandle = Timing.CallDelayed((DateTime.Now + TimeSpan.FromSeconds(RespawnTime)).Second + 1, () =>
            {
                if (CanRespawn)
                    Respawn();
            });

            return LastDeath = DateTime.Now;
        }

        /// <summary>
        /// Deploys the <see cref="PlayerState"/> in game.
        /// </summary>
        public void Deploy()
        {
            if (!typeof(GameState).IsAssignableFrom(new StackTrace().GetFrame(1)?.GetMethod()?.DeclaringType))
                throw new Exception("A PlayerState can be deployed only by a GameState.");

            OnDeployed();
        }

        /// <summary>
        /// Increases the score of the <see cref="PlayerState"/>.
        /// </summary>
        /// <param name="score">The amount of score to be increased.</param>
        public virtual void IncreaseScore(int score) => Score += score;

        /// <summary>
        /// Decreases the score of the <see cref="PlayerState"/>.
        /// </summary>
        /// <param name="score">The amount of score to be decreased.</param>
        public virtual void DecreaseScore(int score) => Score -= score;

        /// <inheritdoc/>
        protected override void PostInitialize()
        {
            base.PostInitialize();

            World.Get().GameState.AddPlayerState(this);
        }

        /// <inheritdoc/>
        protected override void OnEndPlay()
        {
            base.OnEndPlay();

            World.Get().GameState.RemovePlayerState(this);
        }

        /// <inheritdoc/>
        protected override void SubscribeEvents()
        {
            base.SubscribeEvents();

            Exiled.Events.Handlers.Player.Died += OnDiedInternal;
            Exiled.Events.Handlers.Player.InteractingElevator += OnInteractingElevatorInternal;
        }

        /// <inheritdoc/>
        protected override void UnsubscribeEvents()
        {
            base.UnsubscribeEvents();

            Exiled.Events.Handlers.Player.Died -= OnDiedInternal;
            Exiled.Events.Handlers.Player.InteractingElevator -= OnInteractingElevatorInternal;
        }

        /// <summary>
        /// Fired after a <see cref="PlayerState"/> has been deployed.
        /// <para/>
        /// It defines the initial state of the <see cref="PlayerState"/>.
        /// </summary>
        protected virtual void OnDeployed()
        {
            DeployedDispatcher.InvokeAll(this);

            if (IsActive)
                OnDeployed_Active();
            else
                OnDeployed_Inactive();
        }

        /// <summary>
        /// Fired after a <see cref="PlayerState"/> has been deployed and is active.
        /// <para/>
        /// It defines the initial state of the <see cref="PlayerState"/>.
        /// </summary>
        protected virtual void OnDeployed_Active()
        {
            IsRespawnable = World.Get().GameState.Settings.IsRespawnEnabled;
        }

        /// <summary>
        /// Fired after a <see cref="PlayerState"/> has been deployed and is not active.
        /// <para/>
        /// It defines the initial state of the <see cref="PlayerState"/>.
        /// </summary>
        protected virtual void OnDeployed_Inactive()
        {
            Owner.Role.Set(RoleTypeId.Spectator);
        }

        /// <summary>
        /// Respawns the owner of the <see cref="PlayerState"/>.
        /// </summary>
        protected virtual void Respawn()
        {
            if (!IsRespawnable)
                return;
        }

        private void OnDiedInternal(DiedEventArgs ev)
        {
            if (!Check(ev.Player) || !IsRespawnable)
                return;

            ResetRespawn();
        }

        private void OnInteractingElevatorInternal(InteractingElevatorEventArgs ev)
        {
            if (!Check(ev.Player))
                return;

            ev.IsAllowed = !ev.Lift.IsLocked && !World.Get().GameState.Settings.LockedElevators.Contains(ev.Lift.Type);
        }

        private void OnInteractingWarhead(ChangingLeverStatusEventArgs ev)
        {
            GameModeSettings settings = World.Get().GameState.Settings;

            if (!settings.IsWarheadEnabled || !settings.IsWarheadInteractable)
                ev.IsAllowed = false;
        }
    }
}