// -----------------------------------------------------------------------
// <copyright file="IAttackerEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using Exiled.API.Features;
    using Exiled.API.Features.DamageHandlers;

    /// <summary>
    ///     Event args for when a player is taking damage.
    /// </summary>
    public interface IAttackerEvent : IPlayerEvent
    {
        /// <summary>
        ///     Gets the <see cref="Player" /> being targeted.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        ///     Gets or sets the <see cref="DamageHandlerBase" /> managing the damage to the target.
        /// </summary>
        public CustomDamageHandler DamageHandler { get; set; }
    }
}
