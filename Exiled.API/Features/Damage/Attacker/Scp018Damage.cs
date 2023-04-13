// -----------------------------------------------------------------------
// <copyright file="Scp018Damage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage.Attacker
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Pickups.Projectiles;
    using PlayerStatsSystem;
    using UnityEngine;

    /// <summary>
    /// A wrapper class for Scp018DamageHandler.
    /// </summary>
    public class Scp018Damage : AttackerDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp018Damage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="Scp018DamageHandler"/> class.</param>
        internal Scp018Damage(Scp018DamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
        }

        /// <summary>
        /// Gets the <see cref="Scp018DamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new Scp018DamageHandler Base { get; }

        /// <inheritdoc/>
        public override DamageType Type { get; internal set; } = DamageType.Scp018;

        /// <summary>
        /// Gets .
        /// </summary>
        public Scp018Projectile Scp018 { get; internal set; }

        /// <summary>
        /// Gets .
        /// </summary>
        public Vector3 BallVelocity => Base._ballImpactVelocity;
    }
}
