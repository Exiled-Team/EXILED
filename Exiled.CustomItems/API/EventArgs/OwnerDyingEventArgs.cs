// -----------------------------------------------------------------------
// <copyright file="OwnerDyingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.EventArgs
{
    using Exiled.API.Features;
    using Exiled.API.Features.DamageHandlers;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;

    using PlayerStatsSystem;

    using DamageHandlerBase = PlayerStatsSystem.DamageHandlerBase;
    using Item = Exiled.API.Features.Items.Item;
    using Player = Exiled.API.Features.Player;

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
            Item = item;
        }

        /// <summary>
        /// Gets the item in the player's inventory.
        /// </summary>
        public Item Item { get; }
    }
}
