// -----------------------------------------------------------------------
// <copyright file="OwnerHandcuffingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.CustomItems
{
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomModules.API.Features.CustomItems;
    using Exiled.Events.EventArgs.Player;

    /// <summary>
    /// Contains all information of a <see cref="API.Features.CustomItems.CustomItem"/>  before handcuffing a <see cref="Player"/>.
    /// </summary>
    public class OwnerHandcuffingEventArgs : HandcuffingEventArgs, ICustomItemEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerHandcuffingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="customItem"><inheritdoc cref="CustomItem"/></param>
        /// <param name="itemBehaviour"><inheritdoc cref="ItemBehaviour"/></param>
        /// <param name="ev">The <see cref="HandcuffingEventArgs"/> instance.</param>
        public OwnerHandcuffingEventArgs(Item item, CustomItem customItem, ItemBehaviour itemBehaviour, HandcuffingEventArgs ev)
            : this(item, customItem, itemBehaviour, ev.Player, ev.Target, ev.IsAllowed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerHandcuffingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="customItem"><inheritdoc cref="CustomItem"/></param>
        /// <param name="itemBehaviour"><inheritdoc cref="ItemBehaviour"/></param>
        /// <param name="cuffer"><inheritdoc cref="HandcuffingEventArgs.Player"/></param>
        /// <param name="target"><inheritdoc cref="HandcuffingEventArgs.Target"/></param>
        /// <param name="isAllowed"><inheritdoc cref="HandcuffingEventArgs.IsAllowed"/></param>
        public OwnerHandcuffingEventArgs(Item item, CustomItem customItem, ItemBehaviour itemBehaviour, Player cuffer, Player target, bool isAllowed = true)
            : base(cuffer, target, isAllowed)
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