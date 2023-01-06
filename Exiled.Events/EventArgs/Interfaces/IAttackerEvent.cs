// -----------------------------------------------------------------------
// <copyright file="IAttackerEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using API.Features;
    using API.Features.DamageHandlers;

    /// <summary>
    ///     Event args for when a player is taking damage.
    /// </summary>
    public interface IAttackerEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets the attacker <see cref="Player" />.
        /// </summary>
        public Player Attacker { get; }

        /// <summary>
        ///     Gets or sets the <see cref="DamageHandlerBase" /> managing the damage to the target.
        /// </summary>
        public CustomDamageHandler DamageHandler { get; set; }
    }
}