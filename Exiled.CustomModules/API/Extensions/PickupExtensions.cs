// -----------------------------------------------------------------------
// <copyright file="PickupExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Extensions
{
    using Exiled.API.Features.Pickups;
    using Exiled.CustomModules.API.Features.CustomItems;

    /// <summary>
    /// A set of extensions for <see cref="Pickup"/>.
    /// </summary>
    public static class PickupExtensions
    {
        /// <inheritdoc cref="CustomItem.Get(Pickup)"/>
        public static CustomItem Get(this Pickup pickup) => CustomItem.Get(pickup);

        /// <inheritdoc cref="CustomItem.TryGet(Pickup, out CustomItem)"/>
        public static bool TryGet(this Pickup pickup, out CustomItem item) => CustomItem.TryGet(pickup, out item);
    }
}