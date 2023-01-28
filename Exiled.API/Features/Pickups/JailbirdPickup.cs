// -----------------------------------------------------------------------
// <copyright file="JailbirdPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Interfaces;

    using InventorySystem.Items.Jailbird;
    using UnityEngine;

    using BaseJailbirdPickup = InventorySystem.Items.Jailbird.JailbirdPickup;

    /// <summary>
    /// A wrapper class for a jailbird pickup.
    /// </summary>
    public class JailbirdPickup : Pickup, IWrapper<BaseJailbirdPickup>, IWorldSpace
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
        /// Gets or sets the amount of damage remaining before the Jailbird breaks.
        /// </summary>
        /// <remarks>Modifying this value will directly modify <see cref="TotalDamageDealt"/>.</remarks>
        /// <seealso cref="TotalDamageDealt"/>
        public float RemainingDamage
        {
            get => JailbirdItem.DamageLimit - TotalDamageDealt;
            set => TotalDamageDealt = Mathf.Clamp(JailbirdItem.DamageLimit - value, 0, JailbirdItem.DamageLimit);
        }

        /// <summary>
        /// Gets or sets the number of times the item has been charged and used.
        /// </summary>
        public int TotalCharges
        {
            get => Base.TotalCharges;
            set => Base.TotalCharges = value;
        }

        /// <summary>
        /// Gets or sets the amount of charges remaining before the Jailbird breaks.
        /// </summary>
        /// <remarks>Modifying this value will directly modify <see cref="TotalCharges"/>.</remarks>
        /// <seealso cref="TotalCharges"/>
        public int RemainingCharges
        {
            get => JailbirdItem.ChargesLimit - TotalCharges;
            set => TotalCharges = Mathf.Clamp(JailbirdItem.ChargesLimit - value, 0, JailbirdItem.ChargesLimit);
        }

        /// <summary>
        /// Returns the jailbird in a human readable format.
        /// </summary>
        /// <returns>A string containing jailbird related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}*";
    }
}
