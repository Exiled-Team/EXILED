// -----------------------------------------------------------------------
// <copyright file="UsingItemEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using API.Features;
    using Exiled.API.Features.Items;
    using Interfaces;

    using InventorySystem.Items.Usables;

    /// <summary>
    ///     Contains all information before a player uses an item.
    /// </summary>
    public class UsingItemEventArgs : IPlayerEvent, IDeniableEvent, IUsableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="UsingItemEventArgs" /> class.
        /// </summary>
        /// <param name="player">The player who's going to use the item.</param>
        /// <param name="cooldown">
        ///     <inheritdoc cref="Cooldown" />
        /// </param>
        /// <param name="item">
        ///     <inheritdoc cref="UsedItemEventArgs.Item" />
        /// </param>
        public UsingItemEventArgs(Player player, UsableItem item, float cooldown)
        {
            Player = player;
            Usable = Item.Get(item) is Usable usable ? usable : null;
            Cooldown = cooldown;
        }

        /// <summary>
        ///     Gets the item that the player using.
        /// </summary>
        public Usable Usable { get; }

        /// <inheritdoc/>
        public Item Item => Usable;

        /// <summary>
        ///     Gets the player who using the item.
        /// </summary>
        public Player Player { get; }

        /// <summary>
        ///     Gets or sets the item cooldown.
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether or not the player can use the item.
        /// </summary>
        public bool IsAllowed { get; set; } = true;
    }
}