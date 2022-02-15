// -----------------------------------------------------------------------
// <copyright file="OwnerDyingEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.CustomItems.API.EventArgs
{
    using SEXiled.API.Features;
    using SEXiled.CustomItems.API.Features;
    using SEXiled.Events.EventArgs;

    using Item = SEXiled.API.Features.Items.Item;
    using Player = SEXiled.API.Features.Player;

    /// <summary>
    /// Contains all information of a <see cref="CustomItem"/> before a <see cref="Player"/> dies.
    /// </summary>
    public class OwnerDyingEventArgs : DyingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerDyingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="ev">The <see cref="HandcuffingEventArgs"/> instance.</param>
        public OwnerDyingEventArgs(Item item, DyingEventArgs ev)
            : base(ev.Target, ev.Handler.Base)
        {
            if (item == null)
                Log.Warn("Item is null");
            if (ev.Target == null)
                Log.Warn("Target is null");
            if (ev.Handler.Base == null)
                Log.Warn("handler base is null");
            Item = item;
        }

        /// <summary>
        /// Gets the item in the player's inventory.
        /// </summary>
        public Item Item { get; }
    }
}
