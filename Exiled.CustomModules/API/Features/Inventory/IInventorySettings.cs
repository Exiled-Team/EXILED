// -----------------------------------------------------------------------
// <copyright file="IInventorySettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.Inventory
{
    using System.Collections.Generic;

    using Exiled.API.Enums;

    /// <summary>
    /// Defines the contract for config features related to inventory management.
    /// </summary>
    public interface IInventorySettings
    {
        /// <summary>
        /// Gets or sets the items to be given.
        /// </summary>
        public abstract List<ItemType> Items { get; set; }

        /// <summary>
        /// Gets or sets the custom items to be given.
        /// </summary>
        public abstract List<object> CustomItems { get; set; }

        /// <summary>
        /// Gets or sets the ammo box settings to be applied.
        /// </summary>
        public abstract Dictionary<AmmoType, ushort> AmmoBox { get; set; }

        /// <summary>
        /// Gets or sets the custom ammo box settings to be applied.
        /// </summary>
        public abstract Dictionary<uint, ushort> CustomAmmoBox { get; set; }
    }
}