// -----------------------------------------------------------------------
// <copyright file="OwnerDyingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomItems
{
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.Events.EventArgs.Player;

    using Item = Exiled.API.Features.Items.Item;
    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Contains all information of a <see cref="API.Features.CustomItems.CustomItem"/> before a <see cref="Player"/> dies.
    /// </summary>
    public class OwnerDyingEventArgs : DyingEventArgs, ICustomItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerDyingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="customItem"><inheritdoc cref="CustomItem"/></param>
        /// <param name="itemBehaviour"><inheritdoc cref="ItemBehaviour"/></param>
        /// <param name="ev">The <see cref="HandcuffingEventArgs"/> instance.</param>
        public OwnerDyingEventArgs(Item item, CustomItem customItem, ItemBehaviour itemBehaviour, DyingEventArgs ev)
            : base(ev.Player, ev.DamageHandler.Base)
        {
            Item = item;
            CustomItem = customItem;
            ItemBehaviour = itemBehaviour;
        }

        /// <inheritdoc/>
        public Item Item { get; }

        /// <inheritdoc/>
        public CustomItem CustomItem { get; }

        /// <inheritdoc/>
        public ItemBehaviour ItemBehaviour { get; }
    }
}