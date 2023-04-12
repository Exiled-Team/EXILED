// -----------------------------------------------------------------------
// <copyright file="StandardDamage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage
{
    using PlayerStatsSystem;
    using UnityEngine;

    public class StandardDamage : DamageBase
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="StandardDamageHandler"/> class.</param>
        internal StandardDamage(StandardDamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
        }

        /// <summary>
        /// Gets the <see cref="StandardDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new StandardDamageHandler Base { get; }

        /// <summary>
        /// Gets the damage to dealt.
        /// </summary>
        public float Damage
        {
            get => Base.Damage;
            set => Base.Damage = value;
        }

        /// <summary>
        /// Gets or sets .
        /// </summary>
        public HitboxType Hitbox
        {
            get => Base.Hitbox;
            set => Base.Hitbox = value;
        }

        /// <summary>
        /// Gets or sets .
        /// </summary>
        public Vector3 StartVelocity
        {
            get => Base.StartVelocity;
            set => Base.StartVelocity = value;
        }
    }
}
