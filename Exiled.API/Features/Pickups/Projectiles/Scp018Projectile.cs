// -----------------------------------------------------------------------
// <copyright file="Scp018Projectile.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Pickups.Projectiles
{
    using InventorySystem.Items.ThrowableProjectiles;

    using BaseScp018Projectile = InventorySystem.Items.ThrowableProjectiles.Scp018Projectile;

    /// <summary>
    /// A wrapper class for Scp018Projectile.
    /// </summary>
    public class Scp018Projectile : ExplosionGrenadeProjectile
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp018Projectile"/> class.
        /// </summary>
        /// <param name="pickupBase">The base <see cref="BaseScp018Projectile"/> class.</param>
        public Scp018Projectile(BaseScp018Projectile pickupBase)
            : base(pickupBase)
        {
            Base = pickupBase;
        }

        /// <summary>
        /// Gets the <see cref="ExplosionGrenade"/> that this class is encapsulating.
        /// </summary>
        public new BaseScp018Projectile Base { get; }

        /// <summary>
        /// Gets a value indicating whether the damage going to be done on the teamate of the guys who trow it.
        /// </summary>
        public bool IgnoreFriendlyFire => Base.IgnoreFriendlyFire;

        /// <summary>
        /// Gets or sets the time for SCP-018 not to ignore the friendly fire.
        /// </summary>
        public float FriendlyFireTime
        {
            get => Base._friendlyFireTime;
            set => Base._friendlyFireTime = value;
        }

        /// <summary>
        /// Gets the damage when the ball hit something.
        /// </summary>
        public float Damage => Base.CurrentDamage;

        /// <summary>
        /// Returns the Scp018Pickup in a human readable format.
        /// </summary>
        /// <returns>A string containing Scp018Pickup-related data.</returns>
        public override string ToString() => $"{Type} ({Serial}) [{Weight}] *{Scale}* |{Position}| -{Damage}- ={IgnoreFriendlyFire}=";
    }
}
