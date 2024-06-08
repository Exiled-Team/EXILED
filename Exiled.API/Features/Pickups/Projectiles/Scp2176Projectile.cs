// -----------------------------------------------------------------------
// <copyright file="Scp2176Projectile.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Projectiles
{
    using Exiled.API.Extensions;
    using Exiled.API.Interfaces;
    using InventorySystem.Items.ThrowableProjectiles;

    using BaseScp2176Projectile = InventorySystem.Items.ThrowableProjectiles.Scp2176Projectile;

    /// <summary>
    /// A wrapper class for an SCP-2176 Projectile.
    /// </summary>
    public class Scp2176Projectile : EffectGrenadeProjectile, IWrapper<BaseScp2176Projectile>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp2176Projectile"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseScp2176Projectile"/> class.</param>
        public Scp2176Projectile(BaseScp2176Projectile pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Scp2176Projectile"/> class.
        /// </summary>
        internal Scp2176Projectile()
            : this((BaseScp2176Projectile)ItemType.SCP2176.GetItemBase().ServerDropItem())
        {
        }

        /// <summary>
        /// Gets the <see cref="ExplosionGrenade"/> that this class is encapsulating.
        /// </summary>
        public new BaseScp2176Projectile Base { get; }

        /// <summary>
        /// Gets a value indicating whether or not SCP-2176 has shattered.
        /// </summary>
        public bool IsAlreadyTriggered => Base._hasTriggered;

        /// <summary>
        /// Gets or sets a value indicating whether SCP-2176's next collision will make the dropped sound effect.
        /// </summary>
        public bool DropSound
        {
            get => Base.Network_playedDropSound;
            set => Base.Network_playedDropSound = value;
        }

        /// <summary>
        /// Returns the Scp2176Pickup in a human readable format.
        /// </summary>
        /// <returns>A string containing Scp2176Pickup related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{FuseTime}| ={IsAlreadyDetonated}=";
    }
}
