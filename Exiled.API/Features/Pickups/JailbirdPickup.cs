// -----------------------------------------------------------------------
// <copyright file="JailbirdPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Extensions;
    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Features.Items;
    using Exiled.API.Interfaces;
    using InventorySystem.Items;
    using InventorySystem.Items.Jailbird;

    using BaseJailbirdPickup = InventorySystem.Items.Jailbird.JailbirdPickup;

    /// <summary>
    /// A wrapper class for a jailbird pickup.
    /// </summary>
    public class JailbirdPickup : Pickup, IWrapper<BaseJailbirdPickup>
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
            : this((BaseJailbirdPickup)ItemType.Jailbird.GetItemBase().ServerDropItem())
        {
        }

        /// <summary>
        /// Gets the <see cref="BaseJailbirdPickup"/> that this class is encapsulating.
        /// </summary>
        public new BaseJailbirdPickup Base { get; }

        /// <summary>
        /// Gets or sets the amount of damage dealt with a Jailbird melee hit.
        /// </summary>
        [EProperty(category: nameof(JailbirdPickup))]
        public float MeleeDamage { get; set; }

        /// <summary>
        /// Gets or sets the amount of damage dealt with a Jailbird charge hit.
        /// </summary>
        [EProperty(category: nameof(JailbirdPickup))]
        public float ChargeDamage { get; set; }

        /// <summary>
        /// Gets or sets the amount of time in seconds that the <see cref="CustomPlayerEffects.Flashed"/> effect will be applied on being hit.
        /// </summary>
        [EProperty(category: nameof(JailbirdPickup))]
        public float FlashDuration { get; set; }

        /// <summary>
        /// Gets or sets the radius of the Jailbird's hit register.
        /// </summary>
        [EProperty(category: nameof(JailbirdPickup))]
        public float Radius { get; set; }

        /// <summary>
        /// Gets or sets the total amount of damage dealt with the Jailbird.
        /// </summary>
        [EProperty(category: nameof(JailbirdPickup))]
        public float TotalDamageDealt
        {
            get => Base.TotalMelee;
            set => Base.TotalMelee = value;
        }

        /// <summary>
        /// Gets or sets the number of times the item has been charged and used.
        /// </summary>
        [EProperty(category: nameof(JailbirdPickup))]
        public int TotalCharges
        {
            get => Base.TotalCharges;
            set => Base.TotalCharges = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="JailbirdWearState"/> of the item.
        /// </summary>
        [EProperty(category: nameof(JailbirdPickup))]
        public JailbirdWearState WearState
        {
            get => Base.NetworkWear;
            set => Base.NetworkWear = value;
        }

        /// <summary>
        /// Returns the jailbird in a human readable format.
        /// </summary>
        /// <returns>A string containing jailbird related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}*";

        /// <inheritdoc/>
        internal override void ReadItemInfo(Item item)
        {
            base.ReadItemInfo(item);

            if (item is Jailbird jailBirditem)
            {
                MeleeDamage = jailBirditem.MeleeDamage;
                ChargeDamage = jailBirditem.ChargeDamage;
                FlashDuration = jailBirditem.FlashDuration;
                Radius = jailBirditem.Radius;
            }
        }

        /// <inheritdoc/>
        protected override void InitializeProperties(ItemBase itemBase)
        {
            base.InitializeProperties(itemBase);
            if (itemBase is JailbirdItem jailbirdItem)
            {
                MeleeDamage = jailbirdItem._hitreg._damageMelee;
                ChargeDamage = jailbirdItem._hitreg._damageCharge;
                FlashDuration = jailbirdItem._hitreg._flashDuration;
                Radius = jailbirdItem._hitreg._hitregRadius;
            }
        }
    }
}
