// -----------------------------------------------------------------------
// <copyright file="ExplosionGrenadeProjectile.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Projectiles
{
    using Exiled.API.Enums;
    using Exiled.API.Interfaces;
    using InventorySystem;
    using InventorySystem.Items.ThrowableProjectiles;

    using PlayerRoles;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for ExplosionGrenade.
    /// </summary>
    public class ExplosionGrenadeProjectile : EffectGrenadeProjectile, IWrapper<ExplosionGrenade>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosionGrenadeProjectile"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="ExplosionGrenade"/> class.</param>
        public ExplosionGrenadeProjectile(ExplosionGrenade pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosionGrenadeProjectile"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal ExplosionGrenadeProjectile(ItemType type)
            : this((ExplosionGrenade)Object.Instantiate(InventoryItemLoader.AvailableItems[type] as ThrowableItem).Projectile)
        {
        }

        /// <summary>
        /// Gets the <see cref="ExplosionGrenade"/> that this class is encapsulating.
        /// </summary>
        public new ExplosionGrenade Base { get; }

        /// <summary>
        /// Gets or sets the maximum radius of the ExplosionGrenade.
        /// </summary>
        public float MaxRadius
        {
            get => Base._maxRadius;
            set => Base._maxRadius = value;
        }

        /// <summary>
        /// Gets or sets the minimum duration of player can take the effect.
        /// </summary>
        public float MinimalDurationEffect
        {
            get => Base._minimalDuration;
            set => Base._minimalDuration = value;
        }

        /// <summary>
        /// Gets or sets the maximum duration of the <see cref="EffectType.Burned"/> effect.
        /// </summary>
        public float BurnDuration
        {
            get => Base._burnedDuration;
            set => Base._burnedDuration = value;
        }

        /// <summary>
        /// Gets or sets the maximum duration of the <see cref="EffectType.Deafened"/> effect.
        /// </summary>
        public float DeafenDuration
        {
            get => Base._deafenedDuration;
            set => Base._deafenedDuration = value;
        }

        /// <summary>
        /// Gets or sets the maximum duration of the <see cref="EffectType.Concussed"/> effect.
        /// </summary>
        public float ConcussDuration
        {
            get => Base._concussedDuration;
            set => Base._concussedDuration = value;
        }

        /// <summary>
        /// Gets or sets the damage of the <see cref="Team.SCPs"/> going to get.
        /// </summary>
        public float ScpDamageMultiplier
        {
            get => Base._scpDamageMultiplier;
            set => Base._scpDamageMultiplier = value;
        }

        /// <summary>
        /// Returns the ExplosionGrenadePickup in a human readable format.
        /// </summary>
        /// <returns>A string containing ExplosionGrenadePickup-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Position}| -{IsLocked}- ={InUse}=";
    }
}
