// -----------------------------------------------------------------------
// <copyright file="DamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DamageHandlers
{
    using CustomPlayerEffects;

    using Footprinting;

    using PlayerStatsSystem;
    using UnityEngine;

    using BaseHandler = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// A wrapper to easily manipulate the behavior of the <see cref="BaseHandler"/>.
    /// </summary>
    public class DamageHandler : DamageHandlerBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DamageHandler"/> class.
        /// </summary>
        public DamageHandler()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageHandler"/> class.
        /// </summary>
        /// <param name="target">The target to be set.</param>
        /// <param name="attacker">The attacker to be set.</param>
        public DamageHandler(Player target, Player attacker)
        {
            Target = target;
            Attacker = attacker;
            TargetFootprint = target?.Footprint ?? default;
            AttackerFootprint = Attacker?.Footprint ?? default;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageHandler"/> class.
        /// </summary>
        /// <param name="target">The target to be set.</param>
        /// <param name="baseHandler"><inheritdoc cref="DamageHandlerBase.Base"/></param>
        public DamageHandler(Player target, BaseHandler baseHandler)
            : base(baseHandler)
        {
            Target = target;
            Attacker = baseHandler is PlayerStatsSystem.AttackerDamageHandler handler ? Player.Get(handler.Attacker.Hub) : null;
            TargetFootprint = target?.Footprint ?? default;
            AttackerFootprint = baseHandler is PlayerStatsSystem.AttackerDamageHandler handle ? handle.Attacker : Attacker?.Footprint ?? default;
        }

        /// <summary>
        /// Gets or sets the <see cref="Player"/> target.
        /// </summary>
        public Player Target { get; protected set; }

        /// <summary>
        /// Gets or sets the <see cref="Player"/> attacker. CAN BE NULL!.
        /// </summary>
        public Player Attacker { get; set; }

        /// <summary>
        /// Gets or sets the target's <see cref="Footprint"/>.
        /// </summary>
        public Footprint TargetFootprint { get; protected set; }

        /// <summary>
        /// Gets or sets the attacker's <see cref="Footprint"/>.
        /// </summary>
        public Footprint AttackerFootprint { get; protected set; }

        /// <summary>
        /// Gets or sets the amount of damage to be dealt.
        /// </summary>
        public virtual float Damage
        {
            get => Base is StandardDamageHandler handler ? handler.Damage : 0f;
            set
            {
                if (Base is StandardDamageHandler handler)
                    handler.Damage = value;
            }
        }

        /// <summary>
        /// Gets or sets the start velocity.
        /// </summary>
        public Vector3 StartVelocity
        {
            get => Base is StandardDamageHandler handler ? handler.StartVelocity : Vector3.zero;
            set
            {
                if (Base is StandardDamageHandler handler)
                    handler.StartVelocity = value;
            }
        }

        /// <summary>
        /// Gets or sets the direct damage.
        /// </summary>
        public float DealtHealthDamage
        {
            get => Base is StandardDamageHandler handler ? handler.DealtHealthDamage : 0f;
            set
            {
                if (Base is StandardDamageHandler handler)
                    handler.DealtHealthDamage = value;
            }
        }

        /// <summary>
        /// Gets or sets the damage absorbed by AHP processes.
        /// </summary>
        public float AbsorbedAhpDamage
        {
            get => Base is StandardDamageHandler handler ? handler.AbsorbedAhpDamage : 0f;
            set
            {
                if (Base is StandardDamageHandler handler)
                    handler.AbsorbedAhpDamage = value;
            }
        }

        /// <inheritdoc/>
        public override Action ApplyDamage(Player player)
        {
            if (Base is not StandardDamageHandler handler)
                return player.GetModule<HealthStat>().CurValue > 0f ? Action.Damage : Action.Death;

            if (Damage <= 0f)
                return Action.None;

            handler.ApplyDamage(player.ReferenceHub);

            StartVelocity = player.Velocity;
            handler.StartVelocity.y = Mathf.Max(handler.StartVelocity.y, 0f);
            AhpStat ahpModule = player.GetModule<AhpStat>();
            HealthStat healthModule = player.GetModule<HealthStat>();

            if (Damage <= StandardDamageHandler.KillValue)
            {
                ahpModule.CurValue = 0f;
                healthModule.CurValue = 0f;
                return Action.Death;
            }

            ProcessDamage(player);

            foreach (StatusEffectBase effect in player.ActiveEffects)
            {
                if (effect is IDamageModifierEffect damageModifierEffect)
                    Damage *= damageModifierEffect.GetDamageModifier(Damage, handler, handler.Hitbox);
            }

            // DealtHealthDamage = ahpModule.ServerProcessDamage(Damage);
            AbsorbedAhpDamage = Damage - DealtHealthDamage;

            // healthModule.CurValue -= DealtHealthDamage;
            return player.GetModule<HealthStat>().CurValue > 0f ? Action.Damage : Action.Death;
        }

        /// <inheritdoc/>
        public override string ToString() => $"{Target} {Damage} ({Type}) {(Attacker is not null ? Attacker.Nickname : "No one")}";
    }
}