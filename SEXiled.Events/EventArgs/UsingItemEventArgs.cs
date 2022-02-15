// -----------------------------------------------------------------------
// <copyright file="UsingItemEventArgs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.EventArgs
{
    using SEXiled.API.Features;

    using InventorySystem.Items.Usables;

    /// <summary>
    /// Contains all information before a player uses an item.
    /// </summary>
    public class UsingItemEventArgs : UsedItemEventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="UsingItemEventArgs"/> class.
        /// </summary>
        /// <param name="player">The player who's going to use the item.</param>
        /// <param name="cooldown"><inheritdoc cref="Cooldown"/></param>
        /// <param name="item"><inheritdoc cref="UsedItemEventArgs.Item"/></param>
        public UsingItemEventArgs(Player player, UsableItem item, float cooldown)
            : base(player, item)
        {
            Cooldown = cooldown;
        }

        /// <summary>
        /// Gets or sets the item cooldown.
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether or not the player can use the item.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}
