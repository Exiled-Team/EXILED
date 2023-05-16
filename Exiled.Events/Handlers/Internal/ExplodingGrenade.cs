// -----------------------------------------------------------------------
// <copyright file="ExplodingGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using Exiled.API.Features.Pickups;
    using Exiled.Events.EventArgs.Map;

    /// <summary>
    /// Handles <see cref="Handlers.Map.ChangedIntoGrenade"/> event.
    /// </summary>
    internal static class ExplodingGrenade
    {
        /// <inheritdoc cref="Handlers.Map.OnChangedIntoGrenade(ChangedIntoGrenadeEventArgs)" />
        public static void OnChangedIntoGrenade(ChangedIntoGrenadeEventArgs ev)
        {
            if (ev.Pickup is GrenadePickup grenadePickup)
                grenadePickup.GetPickupInfo(ev.Projectile);
        }
    }
}
