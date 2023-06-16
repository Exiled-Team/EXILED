// -----------------------------------------------------------------------
// <copyright file="IDamageEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using Exiled.API.Features.DamageHandlers;

    /// <summary>
    ///     Event args used for all <see cref="CustomDamageHandler" /> related events.
    /// </summary>
    public interface IDamageEvent : IExiledEvent
    {
        /// <summary>
        ///     Gets or sets the <see cref="DamageHandlerBase" /> managing the damage to the target.
        /// </summary>
        public CustomDamageHandler DamageHandler { get; set; }
    }
}
