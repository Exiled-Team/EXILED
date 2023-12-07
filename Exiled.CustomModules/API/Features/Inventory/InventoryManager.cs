// -----------------------------------------------------------------------
// <copyright file="InventoryManager.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.Inventory
{
    using System.Collections.Generic;

    using Exiled.API.Enums;

    /// <summary>
    /// Manages the inventory settings for human players, providing a convenient interface for customization.
    /// </summary>
    public class InventoryManager : IInventorySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryManager"/> class.
        /// </summary>
        public InventoryManager()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="InventoryManager"/> class.
        /// </summary>
        /// <param name="inventory">The list of items to be given.</param>
        /// <param name="customItems">The list of custom items to be given.</param>
        /// <param name="ammoBox">The ammo box settings to be applied.</param>
        public InventoryManager(
            List<ItemType> inventory,
            List<object> customItems,
            Dictionary<AmmoType, ushort> ammoBox)
        {
            Items = inventory;
            CustomItems = customItems;
            AmmoBox = ammoBox;
        }

        /// <inheritdoc/>
        public List<ItemType> Items { get; set; } = new();

        /// <inheritdoc/>
        public List<object> CustomItems { get; set; } = new();

        /// <inheritdoc/>
        public Dictionary<AmmoType, ushort> AmmoBox { get; set; } = new();

        /// <summary>
        /// Gets or sets the probability associated with this inventory slot.
        /// <br>Useful for inventory tweaks involving one or more probability values.</br>
        /// </summary>
        public float Chance { get; set; }
    }
}