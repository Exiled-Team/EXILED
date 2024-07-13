// -----------------------------------------------------------------------
// <copyright file="EffectGrenadeProjectile.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Projectiles
{
    using Exiled.API.Extensions;
    using Exiled.API.Interfaces;
    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// A wrapper class for EffectGrenade.
    /// </summary>
    public class EffectGrenadeProjectile : TimeGrenadeProjectile, IWrapper<EffectGrenade>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EffectGrenadeProjectile"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="EffectGrenade"/> class.</param>
        public EffectGrenadeProjectile(EffectGrenade pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EffectGrenadeProjectile"/> class.
        /// </summary>
        /// <param name="type">The <see cref="ItemType"/> of the pickup.</param>
        internal EffectGrenadeProjectile(ItemType type)
            : base(type)
        {
            Base = (EffectGrenade)((Pickup)this).Base;
        }

        /// <summary>
        /// Gets the <see cref="EffectGrenade"/> that this class is encapsulating.
        /// </summary>
        public new EffectGrenade Base { get; }

        /// <summary>
        /// Returns the EffectGrenadePickup in a human readable format.
        /// </summary>
        /// <returns>A string containing EffectGrenadePickup-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Position}| -{IsLocked}- ={InUse}=";
    }
}
