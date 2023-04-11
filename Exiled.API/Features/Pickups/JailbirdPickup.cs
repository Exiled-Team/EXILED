// -----------------------------------------------------------------------
// <copyright file="JailbirdPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Features.Items;
    using Exiled.API.Interfaces;

    using InventorySystem.Items.Jailbird;
    using UnityEngine;

    using BaseJailbirdPickup = InventorySystem.Items.Jailbird.JailbirdPickup;

    /// <summary>
    /// A wrapper class for a jailbird pickup.
    /// </summary>
    public class JailbirdPickup : Pickup, IWrapper<BaseJailbirdPickup>
    {
        /// <summary>
        /// Number of Charges use before the weapon become AlmostDepleted.
        /// </summary>
        public const int ChargesWarning = JailbirdItem.ChargesWarning;

        /// <summary>
        /// Number of Charges use before the weapon will being destroy.
        /// </summary>
        public const int ChargesLimit = JailbirdItem.ChargesLimit;

        /// <summary>
        /// Number of Damage made before the weapon become AlmostDepleted.
        /// </summary>
        public const float DamageWarning = JailbirdItem.DamageWarning;

        /// <summary>
        /// Number of Damage made before the weapon will being destroy.
        /// </summary>
        public const float DamageLimit = JailbirdItem.DamageLimit;

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
        /// Gets or sets the amount of damage dealt with a Jailbird melee hit.
        /// </summary>
        public float MeleeDamage { get; set; }

        /// <summary>
        /// Gets or sets the amount of damage dealt with a Jailbird charge hit.
        /// </summary>
        public float ChargeDamage { get; set; }

        /// <summary>
        /// Gets or sets the amount of time in seconds that the <see cref="CustomPlayerEffects.Flashed"/> effect will be applied on being hit.
        /// </summary>
        public float FlashDuration { get; set; }

        /// <summary>
        /// Gets or sets the radius of the Jailbird's hit register.
        /// </summary>
        public float Radius { get; set; }

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
        /// Gets a value indicating whether the weapon warn the player than the Item will be broken.
        /// </summary>
        public bool IsAlmostDepleted => IsDamageWarning || IsChargesWarning;

        /// <summary>
        /// Gets a value indicating whether .
        /// </summary>
        public bool IsDamageWarning => TotalDamageDealt >= DamageWarning;

        /// <summary>
        /// Gets a value indicating whether .
        /// </summary>
        public bool IsChargesWarning => TotalCharges >= ChargesWarning;

        /// <summary>
        /// Returns the jailbird in a human readable format.
        /// </summary>
        /// <returns>A string containing jailbird related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}*";

        /// <summary>
        /// .
        /// </summary>
        /// <param name="item"> ..</param>
        /// <returns> ...</returns>
        internal override Pickup GetItemInfo(Item item)
        {
            if (item is Jailbird jailBirditem)
            {
                MeleeDamage = jailBirditem.MeleeDamage;
                ChargeDamage = jailBirditem.ChargeDamage;
                FlashDuration = jailBirditem.FlashDuration;
                Radius = jailBirditem.Radius;
            }

            return this;
        }
    }
}
