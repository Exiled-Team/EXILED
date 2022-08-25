// -----------------------------------------------------------------------
// <copyright file="IProjectileEvent.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Interfaces
{
    using Exiled.API.Features.Pickups.Projectiles;

    /// <summary>
    ///     Event args used for all <see cref="Exiled.API.Features.Pickups.Projectiles.Projectile" /> related events.
    /// </summary>
    public interface IProjectileEvent : IExiledEvent
    {
        /// <summary>
        ///     Gets the <see cref="Exiled.API.Features.Pickups.Projectiles.Projectile" /> triggering the event.
        /// </summary>
        public Projectile Projectile { get; }
    }
}
