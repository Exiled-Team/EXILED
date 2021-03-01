// -----------------------------------------------------------------------
// <copyright file="OwnerDyingEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API.EventArgs
{
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;
    using Exiled.Events.EventArgs;

    /// <summary>
    /// Contains all informations of a <see cref="CustomItem"/> before a <see cref="Player"/> dies.
    /// </summary>
    public class OwnerDyingEventArgs : DyingEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerDyingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="ev">The <see cref="HandcuffingEventArgs"/> instance.</param>
        public OwnerDyingEventArgs(Inventory.SyncItemInfo item, DyingEventArgs ev)
            : this(item, ev.Killer, ev.Target, ev.HitInformation, ev.IsAllowed)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="OwnerDyingEventArgs"/> class.
        /// </summary>
        /// <param name="item"><inheritdoc cref="Item"/></param>
        /// <param name="killer"><inheritdoc cref="DyingEventArgs.Killer"/></param>
        /// <param name="target"><inheritdoc cref="DyingEventArgs.Target"/></param>
        /// <param name="hitInformation"><inheritdoc cref="DyingEventArgs.HitInformation"/></param>
        /// <param name="isAllowed"><inheritdoc cref="DyingEventArgs.IsAllowed"/></param>
        public OwnerDyingEventArgs(Inventory.SyncItemInfo item, Player killer, Player target, PlayerStats.HitInfo hitInformation, bool isAllowed = true)
            : base(killer, target, hitInformation, isAllowed)
        {
            Item = item;
        }

        /// <summary>
        /// Gets the item in the player's inventory.
        /// </summary>
        public Inventory.SyncItemInfo Item { get; }
    }
}
