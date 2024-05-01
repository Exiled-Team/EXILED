// -----------------------------------------------------------------------
// <copyright file="DamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DamageHandlers2
{
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using PlayerStatsSystem;

    /// <summary>
    /// A standard damage handler.
    /// </summary>
    public class DamageHandler : DamageHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DamageHandler"/> class.
        /// </summary>
        /// <param name="baseHandler">The base <see cref="StandardDamageHandler"/>.</param>
        /// <param name="damageType">The <see cref="DamageType"/> that caused damage.</param>
        public DamageHandler(StandardDamageHandler baseHandler, DamageType damageType)
            : base(baseHandler, damageType)
        {
            Base = baseHandler;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageHandler"/> class.
        /// </summary>
        /// <param name="target">The target of damage.</param>
        /// <param name="damage">The amount of health to be dealt.</param>
        /// <param name="damageType">The type of damage.</param>
        public DamageHandler(Player target, float damage, DamageType damageType)
        {
            Target = target;
            Type = damageType;
            Damage = damage;
            Base = new UniversalDamageHandler(damage, DamageTypeExtensions.TranslationConversion.First(x => x.Value == damageType).Key);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageHandler"/> class.
        /// </summary>
        protected DamageHandler()
        {
        }

        /// <summary>
        /// Gets the target of damage.
        /// </summary>
        public Player Target { get; }

        /// <summary>
        /// Gets or sets an affected hitbox.
        /// </summary>
        public HitboxType HitboxType
        {
            get => BaseAs<StandardDamageHandler>().Hitbox;
            set => BaseAs<StandardDamageHandler>().Hitbox = value;
        }

        /// <summary>
        /// Gets or sets an amount of damage to be dealt.
        /// </summary>
        public float Damage
        {
            get => BaseAs<StandardDamageHandler>().Damage;
            set => BaseAs<StandardDamageHandler>().Damage = value;
        }

        /// <summary>
        /// Gets the dealt amount of damage.
        /// </summary>
        /// <remarks>Has a correct value only after <see cref="StandardDamageHandler.ApplyDamage"/>.</remarks>
        public float DealtHealthDamage => BaseAs<StandardDamageHandler>().DealtHealthDamage;

        /// <summary>
        /// Gets the amount of AHP that was dealt.
        /// </summary>
        /// <remarks>Has a correct value only after <see cref="StandardDamageHandler.ApplyDamage"/>.</remarks>
        public float AbsorbedAhpDamage => BaseAs<StandardDamageHandler>().AbsorbedAhpDamage;

        /// <summary>
        /// Gets the amount of Hume Shield that was dealt.
        /// </summary>
        /// <remarks>Has a correct value only after <see cref="StandardDamageHandler.ApplyDamage"/>.</remarks>
        public float AbsorbedHumeShieldDamage => BaseAs<StandardDamageHandler>().AbsorbedHumeDamage;

        /// <summary>
        /// Applies the damage to the specified <see cref="Player"/>.
        /// </summary>
        /// <returns>The <see cref="DamageHandlerBase.Action"/> of the call to this method.</returns>
        public Action ApplyDamage() => (Action)BaseAs<StandardDamageHandler>().ApplyDamage(Target.ReferenceHub);
    }
}