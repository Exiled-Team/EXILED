// -----------------------------------------------------------------------
// <copyright file="FlashbangPickup.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Projectiles
{
    using InventorySystem.Items.ThrowableProjectiles;

    public class FlashbangPickup : EffectGrenadePickup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlashbangPickup"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="FlashbangGrenade"/> class.</param>
        public FlashbangPickup(FlashbangGrenade pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Gets the <see cref="FlashbangGrenade"/> that this class is encapsulating.
        /// </summary>
        public new FlashbangGrenade Base { get; }

        /// <summary>
        /// Returns the FlashbangPickup in a human readable format.
        /// </summary>
        /// <returns>A string containing FlashbangPickup-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Position}| -{Locked}- ={InUse}=";
    }
}
