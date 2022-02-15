// -----------------------------------------------------------------------
// <copyright file="OwnerHandcuffingEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.CustomItems.API.EventArgs
{
    using SEXiled.API.Features;
    using SEXiled.API.Features.Items;
    using SEXiled.CustomItems.API.Features;
    using SEXiled.Events.EventArgs;

    /// <summary>
    /// Contains all information of a <see cref="CustomItem"/> before handcuffing a <see cref="Player"/>.
    /// </summary>
    public class OwnerHandcuffingEventArgs : HandcuffingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerHandcuffingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="ev">The <see cref="HandcuffingEventArgs"/> instance.</param>
        public OwnerHandcuffingEventArgs(Item item, HandcuffingEventArgs ev)
            : this(item, ev.Cuffer, ev.Target, ev.IsAllowed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerHandcuffingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="cuffer"><inheritdoc cref="HandcuffingEventArgs.Cuffer"/></param>
        /// <param name="target"><inheritdoc cref="HandcuffingEventArgs.Target"/></param>
        /// <param name="isAllowed"><inheritdoc cref="HandcuffingEventArgs.IsAllowed"/></param>
        public OwnerHandcuffingEventArgs(Item item, Player cuffer, Player target, bool isAllowed = true)
            : base(cuffer, target, isAllowed)
        {
            Item = item;
        }

        /// <summary>
        /// Gets the item in the player's inventory.
        /// </summary>
        public Item Item { get; }
    }
}
