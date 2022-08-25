// -----------------------------------------------------------------------
// <copyright file="PickingUpArmorEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Exiled.API.Features;

    using InventorySystem.Items.Pickups;

    /// <summary>
    ///     Contains all information before a player picks up <see cref="API.Features.Items.Armor" />.
    /// </summary>
    public class PickingUpArmorEventArgs : PickingUpItemEventArgs
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="PickingUpArmorEventArgs" /> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="PickingUpItemEventArgs.Player" />
        /// </param>
        /// <param name="pickup">
        ///     <inheritdoc cref="PickingUpItemEventArgs.Pickup" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="PickingUpItemEventArgs.IsAllowed" />
        /// </param>
        public PickingUpArmorEventArgs(Player player, ItemPickupBase pickup, bool isAllowed = true)
            : base(player, pickup, isAllowed)
        {
        }
    }
}
