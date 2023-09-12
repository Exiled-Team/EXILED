// -----------------------------------------------------------------------
// <copyright file="ExplosiveGrenadePickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups.Projectiles;

    using InventorySystem.Items;
    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// A wrapper class for dropped Explosive Pickup.
    /// </summary>
    internal class ExplosiveGrenadePickup : GrenadePickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosiveGrenadePickup"/> class.
        /// </summary>
        /// <param name="pickupBase">.</param>
        internal ExplosiveGrenadePickup(TimedGrenadePickup pickupBase)
            : base(pickupBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosiveGrenadePickup"/> class.
        /// </summary>
        internal ExplosiveGrenadePickup()
            : base(ItemType.GrenadeHE)
        {
        }

        /// <summary>
        /// Gets or sets the maximum radius of the grenade.
        /// </summary>
        public float MaxRadius { get; set; }

        /// <summary>
        /// Gets or sets the multiplier for damage against <see cref="Side.Scp"/> players.
        /// </summary>
        public float ScpDamageMultiplier { get; set; }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Burned"/> effect will last.
        /// </summary>
        public float BurnDuration { get; set; }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Deafened"/> effect will last.
        /// </summary>
        public float DeafenDuration { get; set; }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Concussed"/> effect will last.
        /// </summary>
        public float ConcussDuration { get; set; }

        /// <inheritdoc/>
        internal override void ReadItemInfo(Item item)
        {
            base.ReadItemInfo(item);
            if (item is ExplosiveGrenade explosiveGrenadeitem)
            {
                MaxRadius = explosiveGrenadeitem.MaxRadius;
                ScpDamageMultiplier = explosiveGrenadeitem.ScpDamageMultiplier;
                BurnDuration = explosiveGrenadeitem.BurnDuration;
                DeafenDuration = explosiveGrenadeitem.DeafenDuration;
                ConcussDuration = explosiveGrenadeitem.ConcussDuration;
                FuseTime = explosiveGrenadeitem.FuseTime;
            }
        }

        /// <inheritdoc/>
        internal override Pickup GetPickupInfo(Projectile projectile)
        {
            if (projectile is ExplosionGrenadeProjectile explosionGrenadeProjectile)
            {
                explosionGrenadeProjectile.MaxRadius = MaxRadius;
                explosionGrenadeProjectile.ScpDamageMultiplier = ScpDamageMultiplier;
                explosionGrenadeProjectile.BurnDuration = BurnDuration;
                explosionGrenadeProjectile.DeafenDuration = DeafenDuration;
                explosionGrenadeProjectile.ConcussDuration = ConcussDuration;
                explosionGrenadeProjectile.FuseTime = FuseTime;
            }

            return projectile;
        }

        /// <inheritdoc/>
        protected override void InitializeProperties(ItemBase itemBase)
        {
            base.InitializeProperties(itemBase);
            if (itemBase is ThrowableItem throwable && throwable.Projectile is ExplosionGrenade explosiveGrenade)
            {
                MaxRadius = explosiveGrenade._maxRadius;
                ScpDamageMultiplier = explosiveGrenade._scpDamageMultiplier;
                BurnDuration = explosiveGrenade._burnedDuration;
                DeafenDuration = explosiveGrenade._deafenedDuration;
                ConcussDuration = explosiveGrenade._concussedDuration;
                FuseTime = explosiveGrenade._fuseTime;
            }
        }
    }
}