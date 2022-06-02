// -----------------------------------------------------------------------
// <copyright file="BodyArmorPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using BaseBodyArmor = InventorySystem.Items.Armor.BodyArmorPickup;

    /// <summary>
    /// A wrapper class for BodyArmor.
    /// </summary>
    public class BodyArmorPickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BodyArmorPickup"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="BaseBodyArmor"/> class.</param>
        internal BodyArmorPickup(BaseBodyArmor itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BodyArmorPickup"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal BodyArmorPickup(ItemType type)
            : base(type)
        {
            Base = (BaseBodyArmor)((Pickup)this).Base;
        }

        /// <summary>
        /// Gets the <see cref="BaseBodyArmor"/> that this class is encapsulating.
        /// </summary>
        public new BaseBodyArmor Base { get; }

        /// <summary>
        /// Returns the AmmoPickup in a human readable format.
        /// </summary>
        /// <returns>A string containing AmmoPickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}*";
    }
}
