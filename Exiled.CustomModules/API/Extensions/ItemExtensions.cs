// -----------------------------------------------------------------------
// <copyright file="ItemExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Extensions
{
    using Exiled.API.Features.Items;
    using Exiled.CustomModules.API.Features.CustomItems;

    /// <summary>
    /// A set of extensions for <see cref="Item"/>.
    /// </summary>
    public static class ItemExtensions
    {
        /// <inheritdoc cref="CustomItem.Get(Item)"/>
        public static CustomItem Get(this Item item) => CustomItem.Get(item);

        /// <inheritdoc cref="CustomItem.TryGet(Item, out CustomItem)"/>
        public static bool TryGet(this Item item, out CustomItem customItem) => CustomItem.TryGet(item, out customItem);
    }
}