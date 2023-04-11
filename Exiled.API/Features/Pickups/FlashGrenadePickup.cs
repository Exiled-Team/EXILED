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
    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// .
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

        /// <summary>
        /// .
        /// </summary>
        /// <param name="item"> ..</param>
        /// <returns> ...</returns>
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
    }
}
