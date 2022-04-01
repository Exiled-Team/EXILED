// -----------------------------------------------------------------------
// <copyright file="Extensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems.API {
    using System;
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.CustomItems.API.Features;

    using Interactables.Interobjects.DoorUtils;

    using UnityEngine;

    /// <summary>
    /// A collection of API methods.
    /// </summary>
    public static class Extensions {
        /// <summary>
        /// Resets the player's inventory to the provided list of items and/or customitems names, clearing any items it already possess.
        /// </summary>
        /// <param name="player">The player to which items will be given.</param>
        /// <param name="newItems">The new items that have to be added to the inventory.</param>
        /// <param name="displayMessage">Indicates a value whether <see cref="CustomItem.ShowPickedUpMessage"/> will be called when the player receives the <see cref="CustomItem"/> or not.</param>
        public static void ResetInventory(this Player player, List<string> newItems, bool displayMessage = false) {
            foreach (Item item in player.Items) {
                if (CustomItem.TryGet(item, out CustomItem customItem))
                    customItem.TrackedSerials.Remove(item.Serial);
            }

            player.ClearInventory();

            foreach (string item in newItems) {
                if (Enum.TryParse(item, true, out ItemType parsedItem)) {
                    player.AddItem(parsedItem);
                }
                else if (!CustomItem.TryGive(player, item, displayMessage)) {
                    Log.Debug($"\"{item}\" is not a valid item name, nor a custom item name.", CustomItems.Instance.Config.Debug);
                }
            }
        }

        /// <summary>
        /// Registers an <see cref="IEnumerable{T}"/> of <see cref="CustomItem"/>s.
        /// </summary>
        /// <param name="customItems"><see cref="CustomItem"/>s to be registered.</param>
        public static void Register(this IEnumerable<CustomItem> customItems) {
            if (customItems == null)
                throw new ArgumentNullException("customItems");

            foreach (CustomItem customItem in customItems)
                customItem.TryRegister();
        }

        /// <summary>
        /// Unregisters an <see cref="IEnumerable{T}"/> of <see cref="CustomItem"/>s.
        /// </summary>
        /// <param name="customItems"><see cref="CustomItem"/>s to be unregistered.</param>
        public static void Unregister(this IEnumerable<CustomItem> customItems) {
            if (customItems == null)
                throw new ArgumentNullException("customItems");

            foreach (CustomItem customItem in customItems)
                customItem.TryUnregister();
        }
    }
}
