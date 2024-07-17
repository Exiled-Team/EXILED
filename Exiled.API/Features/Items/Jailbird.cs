// -----------------------------------------------------------------------
// <copyright file="Jailbird.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using System;

    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Interfaces;
    using InventorySystem.Items.Autosync;
    using InventorySystem.Items.Jailbird;
    using Mirror;
    using UnityEngine;

    using JailbirdPickup = Pickups.JailbirdPickup;

    /// <summary>
    /// A wrapped class for <see cref="JailbirdItem"/>.
    /// </summary>
    public class Jailbird : Item, IWrapper<JailbirdItem>
    {
        private readonly ConstProperty<double> chargeTolerance = new(-0.4000000059604645, new[] { typeof(JailbirdItem) });

        /// <summary>
        /// Initializes a new instance of the <see cref="Jailbird"/> class.
        /// </summary>
        /// <param name="itemBase">The base <see cref="JailbirdItem"/> class.</param>
        public Jailbird(JailbirdItem itemBase)
            : base(itemBase)
        {
            Base = itemBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Jailbird"/> class, as well as a new Jailbird item.
        /// </summary>
        internal Jailbird()
            : this((JailbirdItem)Server.Host.Inventory.CreateItemInstance(new(ItemType.Jailbird, 0), false))
        {
        }

        /// <summary>
        /// Gets the <see cref="JailbirdItem"/> that this class is encapsulating.
        /// </summary>
        public new JailbirdItem Base { get; }

        /// <summary>
        /// Gets or sets the amount of damage dealt with a Jailbird melee hit.
        /// </summary>
        [EProperty(category: nameof(Jailbird))]
        public float MeleeDamage
        {
            get => Base._hitreg._damageMelee;
            set => Base._hitreg._damageMelee = value;
        }

        /// <summary>
        /// Gets or sets the amount of damage dealt with a Jailbird charge hit.
        /// </summary>
        [EProperty(category: nameof(Jailbird))]
        public float ChargeDamage
        {
            get => Base._hitreg._damageCharge;
            set => Base._hitreg._damageCharge = value;
        }

        /// <summary>
        /// Gets or sets the amount of time in seconds that the <see cref="CustomPlayerEffects.Flashed"/> effect will be applied on being hit.
        /// </summary>
        [EProperty(category: nameof(Jailbird))]
        public float FlashDuration
        {
            get => Base._hitreg._concussionDuration;
            set => Base._hitreg._concussionDuration = value;
        }

        /// <summary>
        /// Gets or sets the radius of the Jailbird's hit register.
        /// </summary>
        [EProperty(category: nameof(Jailbird))]
        public float Radius
        {
            get => Base._hitreg._hitregRadius;
            set => Base._hitreg._hitregRadius = value;
        }

        /// <summary>
        /// Gets or sets the total amount of damage dealt with the Jailbird.
        /// </summary>
        [EProperty(category: nameof(Jailbird))]
        public float TotalDamageDealt
        {
            get => Base._hitreg.TotalMeleeDamageDealt;
            set => Base._hitreg.TotalMeleeDamageDealt = value;
        }

        /// <summary>
        /// Gets or sets the number of times the item has been charged and used.
        /// </summary>
        [EProperty(category: nameof(Jailbird))]
        public int TotalCharges
        {
            get => Base.TotalChargesPerformed;
            set => Base.TotalChargesPerformed = value;
        }

        /// <summary>
        /// Gets or sets the <see cref="JailbirdWearState"/> for this item.
        /// </summary>
        [EProperty(category: nameof(Jailbird))]
        public JailbirdWearState WearState
        {
            get => Base._deterioration.WearState;
            set
            {
                TotalDamageDealt = GetDamage(value);
                TotalCharges = GetCharge(value);
                Base._deterioration.RecheckUsage();
            }
        }

        /// <summary>
        /// Gets or sets amount of time when Jailbird is charging.
        /// </summary>
        public double ChargeTolerance
        {
            get => chargeTolerance;
            set => chargeTolerance.Value = value;
        }

        /// <summary>
        /// Calculates the damage corresponding to a given <see cref="JailbirdWearState"/>.
        /// </summary>
        /// <param name="wearState">The wear state to calculate damage for.</param>
        /// <returns>The amount of damage associated with the specified wear state.</returns>
        public float GetDamage(JailbirdWearState wearState)
        {
            foreach (Keyframe keyframe in Base._deterioration._damageToWearState.keys)
            {
                if (Base._deterioration.FloatToState(keyframe.value) == wearState)
                    return keyframe.time;
            }

            throw new Exception("Wear state not found in charges to wear state mapping.");
        }

        /// <summary>
        /// Gets the charge needed to reach a specific <see cref="JailbirdWearState"/>.
        /// </summary>
        /// <param name="wearState">The desired wear state to calculate the charge for.</param>
        /// <returns>The charge value required to achieve the specified wear state.</returns>
        public int GetCharge(JailbirdWearState wearState)
        {
            foreach (Keyframe keyframe in Base._deterioration._chargesToWearState.keys)
            {
                if (Base._deterioration.FloatToState(keyframe.value) == wearState)
                    return Mathf.RoundToInt(keyframe.time);
            }

            throw new Exception("Wear state not found in charges to wear state mapping.");
        }

        /// <summary>
        /// Resets the Jailbird to normal.
        /// </summary>
        public void Reset() => Base.ServerReset();

        /// <summary>
        /// Breaks the Jailbird.
        /// </summary>
        public void Break()
        {
            WearState = JailbirdWearState.Broken;
            using (new AutosyncRpc(Base, true, out NetworkWriter networkWriter))
            {
                networkWriter.WriteByte(0);
                networkWriter.WriteByte((byte)JailbirdWearState.Broken);
            }

            using (new AutosyncRpc(Base, true, out NetworkWriter networkWriter2))
            {
                networkWriter2.WriteByte(1);
            }
        }

        /// <summary>
        /// Clones current <see cref="Jailbird"/> object.
        /// </summary>
        /// <returns> New <see cref="Jailbird"/> object. </returns>
        public override Item Clone() => new Jailbird()
        {
            MeleeDamage = MeleeDamage,
            ChargeDamage = ChargeDamage,
            TotalDamageDealt = TotalDamageDealt,
            TotalCharges = TotalCharges,
        };

        /// <summary>
        /// Returns the JailBird in a human readable format.
        /// </summary>
        /// <returns>A string containing JailBird-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}*";

        /// <inheritdoc/>
        internal override void ReadPickupInfo(Pickup pickup)
        {
            base.ReadPickupInfo(pickup);
            if (pickup is JailbirdPickup jailbirdPickup)
            {
                MeleeDamage = jailbirdPickup.MeleeDamage;
                ChargeDamage = jailbirdPickup.ChargeDamage;
                FlashDuration = jailbirdPickup.FlashDuration;
                Radius = jailbirdPickup.Radius;
            }
        }
    }
}