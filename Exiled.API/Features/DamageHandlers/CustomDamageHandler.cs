// -----------------------------------------------------------------------
// <copyright file="CustomDamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DamageHandlers
{
    using CustomPlayerEffects;

    using Exiled.API.Enums;
    using Exiled.API.Features.Items;

    using PlayerStatsSystem;

    using UnityEngine;

    using BaseAttackerHandler = PlayerStatsSystem.AttackerDamageHandler;
    using BaseFirearmHandler = PlayerStatsSystem.FirearmDamageHandler;
    using BaseHandler = PlayerStatsSystem.DamageHandlerBase;

    /// <summary>
    /// A wrapper to easily manipulate the behavior of <see cref="BaseHandler"/>.
    /// </summary>
    public sealed class CustomDamageHandler : AttackerDamageHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDamageHandler"/> class.
        /// </summary>
        /// <param name="target">The target to be set.</param>
        /// <param name="baseHandler">The base <see cref="BaseHandler"/>.</param>
        public CustomDamageHandler(Player target, BaseHandler baseHandler)
            : base(target, baseHandler)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDamageHandler"/> class.
        /// </summary>
        /// <param name="target">The target to be set.</param>
        /// <param name="attacker">The attacker to be set.</param>
        /// <param name="damage">The amount of damage to be set.</param>
        /// <param name="damageType">The <see cref="DamageType"/> to be set.</param>
        public CustomDamageHandler(Player target, Player attacker, float damage, DamageType damageType = DamageType.Unknown)
            : base(target, attacker)
        {
            Damage = damage;
            Type = damageType;
            Firearm firearm = new Firearm(ItemType.GunAK)
            {
                Base =
                {
                    Owner = attacker.ReferenceHub,
                },
            };
            Base = new FirearmDamageHandler(firearm, target, new BaseFirearmHandler(firearm.Base, damage));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDamageHandler"/> class.
        /// </summary>
        /// <param name="target">The target to be set.</param>
        /// <param name="attacker">The attacker to be set.</param>
        /// <param name="damage">The amount of damage to be set.</param>
        /// <param name="damageType">The <see cref="DamageType"/> to be set.</param>
        /// <param name="cassieAnnouncement">The <see cref="DamageHandlerBase.CassieAnnouncement"/> to be set.</param>
        public CustomDamageHandler(Player target, Player attacker, float damage, DamageType damageType, CassieAnnouncement cassieAnnouncement)
            : this(target, attacker, damage, damageType) => CassieDeathAnnouncement = cassieAnnouncement;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDamageHandler"/> class.
        /// </summary>
        /// <param name="target">The target to be set.</param>
        /// <param name="attacker">The attacker to be set.</param>
        /// <param name="damage">The amount of damage to be set.</param>
        /// <param name="damageType">The <see cref="DamageType"/> to be set.</param>
        /// <param name="cassieAnnouncement">The <see cref="DamageHandlerBase.CassieAnnouncement"/> to be set.</param>
        public CustomDamageHandler(Player target, Player attacker, float damage, DamageType damageType, string cassieAnnouncement)
            : this(target, attacker, damage, damageType) => CassieDeathAnnouncement = new CassieAnnouncement(cassieAnnouncement);

        /// <summary>
        /// Gets the base <see cref="DamageHandlerBase"/>.
        /// </summary>
        public new DamageHandlerBase Base { get; }

        /// <inheritdoc/>
        public override Action ApplyDamage(Player player)
        {
            if (Damage <= 0f)
                return Action.None;

            StartVelocity = player.ReferenceHub.playerMovementSync.PlayerVelocity;
            Cast<BaseFirearmHandler>().StartVelocity.y = Mathf.Max(Cast<BaseFirearmHandler>().StartVelocity.y, 0f);
            AhpStat ahpModule = player.GetModule<AhpStat>();
            HealthStat healthModule = player.GetModule<HealthStat>();

            if (Damage <= -1f)
                return KillPlayer(player, Base);

            ProcessDamage(player);

            foreach (PlayerEffect effect in player.ActiveEffects)
            {
                if (effect is IDamageModifierEffect damageModifierEffect)
                    Damage *= damageModifierEffect.GetDamageModifier(Damage, Base, Cast<BaseFirearmHandler>().Hitbox);
            }

            DealtHealthDamage = ahpModule.ServerProcessDamage(Damage);
            AbsorbedAhpDamage = Damage - DealtHealthDamage;

            return healthModule.CurValue - DealtHealthDamage > 0f ? Action.Damage : KillPlayer(player, Base);
        }

        private static Action KillPlayer(Player player, DamageHandlerBase damageHandlerBase)
        {
            Ragdoll.Spawn(player, damageHandlerBase.Base);

            if (damageHandlerBase.SafeCast(out BaseAttackerHandler handler) && damageHandlerBase.BaseCast<FirearmDamageHandler>().Attacker != null)
                player.ReferenceHub.playerStats.TargetReceiveAttackerDeathReason(damageHandlerBase.BaseCast<FirearmDamageHandler>().Attacker.Nickname, damageHandlerBase.BaseCast<FirearmDamageHandler>().Attacker.Role);
            else
                player.ReferenceHub.playerStats.TargetReceiveSpecificDeathReason(handler);

            player.DropItems();
            player.SetRole(RoleType.Spectator, SpawnReason.Died);
            player.SendConsoleMessage("You died. Reason: " + handler.ServerLogsText, "yellow");

            return Action.Death;
        }
    }
}
