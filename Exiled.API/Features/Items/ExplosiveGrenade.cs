// -----------------------------------------------------------------------
// <copyright file="ExplosiveGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Items
{
    using Exiled.API.Enums;

    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// A wrapper class for <see cref="ExplosionGrenade"/>.
    /// </summary>
    public class ExplosiveGrenade : Throwable
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosiveGrenade"/> class.
        /// </summary>
        /// <param name="itemBase"><inheritdoc cref="Throwable.Base"/></param>
        public ExplosiveGrenade(ThrowableItem itemBase)
            : base(itemBase)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ExplosiveGrenade"/> class.
        /// </summary>
        /// <param name="type"><inheritdoc cref="Throwable.Base"/></param>
        public ExplosiveGrenade(ItemType type)
            : base(type)
        {
        }

        /// <summary>
        /// Gets the <see cref="ExplosionGrenade"/> for this item.
        /// </summary>
        public new ExplosionGrenade Projectile => (ExplosionGrenade)Base.Projectile;

        /// <summary>
        /// Gets or sets the maximum radius of the grenade.
        /// </summary>
        public float MaxRadius
        {
            get => Projectile._maxRadius;
            set => Projectile._maxRadius = value;
        }

        /// <summary>
        /// Gets or sets the multiplier for damage against <see cref="Side.Scp"/> players.
        /// </summary>
        public float ScpMultiplier
        {
            get => Projectile._scpDamageMultiplier;
            set => Projectile._scpDamageMultiplier = value;
        }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Burned"/> effect will last.
        /// </summary>
        public float BurnDuration
        {
            get => Projectile._burnedDuration;
            set => Projectile._burnedDuration = value;
        }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Deafened"/> effect will last.
        /// </summary>
        public float DeafenDuration
        {
            get => Projectile._deafenedDuration;
            set => Projectile._deafenedDuration = value;
        }

        /// <summary>
        /// Gets or sets how long the <see cref="EffectType.Concussed"/> effect will last.
        /// </summary>
        public float ConcussDuration
        {
            get => Projectile._concussedDuration;
            set => Projectile._concussedDuration = value;
        }

        /// <summary>
        /// Gets or sets how long the fuse will last.
        /// </summary>
        public float FuseTime
        {
            get => Projectile._fuseTime;
            set => Projectile._fuseTime = value;
        }
    }
}
