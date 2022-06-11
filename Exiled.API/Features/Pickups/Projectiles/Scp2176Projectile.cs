// -----------------------------------------------------------------------
// <copyright file="Scp2176Projectile.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Projectiles
{
    using InventorySystem.Items.ThrowableProjectiles;

    using BaseScp2176Projectile = InventorySystem.Items.ThrowableProjectiles.Scp2176Projectile;

    /// <summary>
    /// A wrapper class for Scp2176Projectile.
    /// </summary>
    public class Scp2176Projectile : EffectGrenadeProjectile
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
        /// Gets the <see cref="ExplosionGrenade"/> that this class is encapsulating.
        /// </summary>
        public new BaseScp2176Projectile Base { get; }

        /// <summary>
        /// Gets a value indicating whether know if the item has being Shatered.
        /// </summary>
        public bool IsAlreadyTriggered => Base._hasTriggered;

        /// <summary>
        /// Gets or sets a value indicating whether the next time of when the Pickup going to collisioning something it's make sound.
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
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{FuseTime}| -{TargetTime}- ={IsAlreadyDetonated}=";
    }
}
