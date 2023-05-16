// -----------------------------------------------------------------------
// <copyright file="FlashGrenadePickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups.Projectiles;
    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// A wrapper class for dropped Flash Pickup.
    /// </summary>
    internal class FlashGrenadePickup : GrenadePickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlashGrenadePickup"/> class.
        /// </summary>
        /// <param name="pickupBase">.</param>
        internal FlashGrenadePickup(TimedGrenadePickup pickupBase)
            : base(pickupBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="FlashGrenadePickup"/> class.
        /// </summary>
        internal FlashGrenadePickup()
            : base(ItemType.GrenadeFlash)
        {
        }

        /// <summary>
        /// Gets or sets the minimum duration of player can take the effect.
        /// </summary>
        public float MinimalDurationEffect { get; set; }

        /// <summary>
        /// Gets or sets the additional duration of the <see cref="EffectType.Blinded"/> effect.
        /// </summary>
        public float AdditionalBlindedEffect { get; set; }

        /// <summary>
        /// Gets or sets the how mush the flash grenade going to be intensified when explode at <see cref="RoomType.Surface"/>.
        /// </summary>
        public float SurfaceDistanceIntensifier { get; set; }

        /// <inheritdoc/>
        internal override Pickup GetItemInfo(Item item)
        {
            base.GetItemInfo(item);
            if (item is FlashGrenade flashGrenadeitem)
            {
                MinimalDurationEffect = flashGrenadeitem.MinimalDurationEffect;
                AdditionalBlindedEffect = flashGrenadeitem.AdditionalBlindedEffect;
                SurfaceDistanceIntensifier = flashGrenadeitem.SurfaceDistanceIntensifier;
                FuseTime = flashGrenadeitem.FuseTime;
            }

            return this;
        }

        /// <inheritdoc/>
        internal override Item GetPickupInfo(Item item)
        {
            base.GetPickupInfo(item);
            if (item is FlashGrenade flashGrenadeitem)
            {
                flashGrenadeitem.MinimalDurationEffect = MinimalDurationEffect;
                flashGrenadeitem.AdditionalBlindedEffect = AdditionalBlindedEffect;
                flashGrenadeitem.SurfaceDistanceIntensifier = SurfaceDistanceIntensifier;
                flashGrenadeitem.FuseTime = FuseTime;
            }

            return item;
        }

        /// <inheritdoc/>
        internal override Pickup GetPickupInfo(Projectile projectile)
        {
            if (projectile is FlashbangProjectile flashbangProjectile)
            {
                flashbangProjectile.MinimalDurationEffect = MinimalDurationEffect;
                flashbangProjectile.AdditionalBlindedEffect = AdditionalBlindedEffect;
                flashbangProjectile.SurfaceDistanceIntensifier = SurfaceDistanceIntensifier;
                flashbangProjectile.FuseTime = FuseTime;
            }

            return projectile;
        }
    }
}
