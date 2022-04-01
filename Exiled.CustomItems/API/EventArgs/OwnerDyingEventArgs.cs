// -----------------------------------------------------------------------
// <copyright file="OwnerDyingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.EventArgs {
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;

    using PlayerStatsSystem;

    using Item = Exiled.API.Features.Items.Item;
    using Player = Exiled.API.Features.Player;

    /// <summary>
    /// Contains all information of a <see cref="CustomItem"/> before a <see cref="Exiled.API.Features.Player"/> dies.
    /// </summary>
    public class OwnerDyingEventArgs : DyingEventArgs {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerDyingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="ev">The <see cref="HandcuffingEventArgs"/> instance.</param>
        public OwnerDyingEventArgs(Item item, DyingEventArgs ev)
            : this(item, ev.Target, ev.Handler.Base) {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerDyingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="target"><inheritdoc cref="DyingEventArgs.Target"/></param>
        /// <param name="damageHandler"><inheritdoc cref="DyingEventArgs.DamageHandler"/></param>
        public OwnerDyingEventArgs(Item item, Player target, DamageHandlerBase damageHandler)
            : base(target, damageHandler) {
            Item = item;
        }

        /// <summary>
        /// Gets the item in the player's inventory.
        /// </summary>
        public Item Item { get; }
    }
}
