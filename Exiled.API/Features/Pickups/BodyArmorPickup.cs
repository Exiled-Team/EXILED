// -----------------------------------------------------------------------
// <copyright file="BodyArmorPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Interfaces;

    using BaseBodyArmor = InventorySystem.Items.Armor.BodyArmorPickup;

    /// <summary>
    /// A wrapper class for a Body Armor pickup.
    /// </summary>
    public class BodyArmorPickup : Pickup, IWrapper<BaseBodyArmor>, IWorldSpace
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BodyArmorPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseBodyArmor"/> class.</param>
        internal BodyArmorPickup(BaseBodyArmor pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
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
        /// Returns the BodyArmorPickup in a human readable format.
        /// </summary>
        /// <returns>A string containing BodyArmorPickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}*";
    }
}
