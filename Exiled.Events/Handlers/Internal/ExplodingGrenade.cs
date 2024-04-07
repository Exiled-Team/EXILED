// -----------------------------------------------------------------------
// <copyright file="ExplodingGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using Exiled.Events.EventArgs.Map;

    /// <summary>
    /// Handles <see cref="Map.ChangedIntoGrenade"/> event.
    /// </summary>
    internal static class ExplodingGrenade
    {
        /// <inheritdoc cref="Map.OnChangedIntoGrenade(ChangedIntoGrenadeEventArgs)" />
        public static void OnChangedIntoGrenade(ChangedIntoGrenadeEventArgs ev)
        {
            ev.Pickup.WriteProjectileInfo(ev.Projectile);
        }
    }
}
