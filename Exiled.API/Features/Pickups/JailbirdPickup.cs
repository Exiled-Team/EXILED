// -----------------------------------------------------------------------
// <copyright file="JailbirdPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using InventorySystem.Items.Jailbird;

    using BaseJailbirdPickup = InventorySystem.Items.Jailbird.JailbirdPickup;

    /// <summary>
    /// A wrapper class for a jailbird pickup.
    /// </summary>
    public class JailbirdPickup : Pickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JailbirdPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseJailbirdPickup"/> class.</param>
        internal JailbirdPickup(BaseJailbirdPickup pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="JailbirdPickup"/> class.
        /// </summary>
        internal JailbirdPickup()
            : base(ItemType.Jailbird)
        {
            Base = (BaseJailbirdPickup)((Pickup)this).Base;
        }

        /// <summary>
        /// Gets the <see cref="BaseJailbirdPickup"/> that this class is encapsulating.
        /// </summary>
        public new BaseJailbirdPickup Base { get; }

        /// <summary>
        /// Gets or sets the total amount of damage dealt with the Jailbird.
        /// </summary>
        public float TotalDamageDealt
        {
            get => Base.TotalMelee;
            set => Base.TotalMelee = value;
        }

        /// <summary>
        /// Gets the amount of damage remaining.
        /// </summary>
        /// <remarks>To extend the amount of damage a Jailbird can deal, <see cref="TotalDamageDealt"/> can be modified.</remarks>
        /// <seealso cref="TotalDamageDealt"/>
        public float RemainingDamage => JailbirdItem.DamageLimit - TotalDamageDealt;

        /// <summary>
        /// Gets or sets the number of times the item has been charged and used.
        /// </summary>
        public int TotalCharges
        {
            get => Base.TotalCharges;
            set => Base.TotalCharges = value;
        }

        /// <summary>
        /// Gets the amount of charges remaining.
        /// </summary>
        /// <remarks>To extend the amount of charges a Jailbird has, <see cref="TotalCharges"/> can be modified.</remarks>
        /// <seealso cref="TotalCharges"/>
        public int RemainingCharges => JailbirdItem.ChargesLimit - Base.TotalCharges;

        /// <summary>
        /// Returns the jailbird in a human readable format.
        /// </summary>
        /// <returns>A string containing jailbird related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}*";
    }
}
