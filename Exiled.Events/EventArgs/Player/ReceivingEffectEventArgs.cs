// -----------------------------------------------------------------------
// <copyright file="ReceivingEffectEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using CustomPlayerEffects;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before a player receives a <see cref="PlayerEffect" />.
    /// </summary>
    public class ReceivingEffectEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ReceivingEffectEventArgs" /> class.
        /// </summary>
        /// <param name="player">The <see cref="Player" /> receiving the effect.</param>
        /// <param name="effect">The <see cref="PlayerEffect" /> being added to the player.</param>
        /// <param name="state">The state the effect is being changed to.</param>
        /// <param name="currentState">The current state of the effect being changed.</param>
        public ReceivingEffectEventArgs(Player player, PlayerEffect effect, byte state, byte currentState)
        {
            Player = player;
            Effect = effect;
            State = state;
            CurrentState = currentState;
        }

        /// <summary>
        ///     Gets the <see cref="PlayerEffect" /> being received.
        /// </summary>
        public PlayerEffect Effect { get; }

        /// <summary>
        ///     Gets or sets a value indicating how long the effect will last.
        /// </summary>
        public float Duration { get; set; } = 0.0f;

        /// <summary>
        ///     Gets or sets the value of the new state of the effect.
        /// </summary>
        public byte State { get; set; }

        /// <summary>
        ///     Gets the value of the current state of this effect on the player.
        /// </summary>
        public byte CurrentState { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the effect will be applied.
        /// </summary>
        public bool IsAllowed { get; set; } = true;

        /// <summary>
        ///     Gets the <see cref="Player" /> receiving the effect.
        /// </summary>
        public Player Player { get; }
    }
}
