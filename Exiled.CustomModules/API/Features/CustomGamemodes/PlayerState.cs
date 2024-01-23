// -----------------------------------------------------------------------
// <copyright file="PlayerState.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;
    using System.Diagnostics;

    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.DynamicEvents;

    /// <summary>
    /// Represents the state of an individual player within the custom game mode, derived from <see cref="EPlayerBehaviour"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// The <see cref="PlayerState"/> class manages the behavior and interactions of a single player within the custom game mode.
    /// </para>
    /// <para>
    /// It serves as a base class for defining player-specific actions and responses within the context of the custom game mode.
    /// </para>
    /// </remarks>
    public abstract class PlayerState : EPlayerBehaviour
    {
        /// <summary>
        /// Gets the <see cref="TDynamicEventDispatcher{T}"/> which handles all delegates to be fired after the <see cref="PlayerState"/> has been deployed.
        /// </summary>
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<PlayerState> DeployedDispatcher { get; private set; }

        /// <summary>
        /// Gets or sets the <see cref="PlayerState"/> score.
        /// </summary>
        public int Score { get; protected set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="PlayerState"/> is respawnable.
        /// </summary>
        public bool IsRespawnable { get; set; }

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

        /// <summary>
        /// Fired after a <see cref="PlayerState"/> has been deployed.
        /// <para/>
        /// It defines the initial state of the <see cref="PlayerState"/>.
        /// </summary>
        protected virtual void OnDeployed()
        {
            DeployedDispatcher.InvokeAll(this);
        }

        /// <summary>
        /// Respawns the owner of the <see cref="PlayerState"/>.
        /// </summary>
        protected virtual void Respawn()
        {
            if (!IsRespawnable)
                return;
        }
    }
}