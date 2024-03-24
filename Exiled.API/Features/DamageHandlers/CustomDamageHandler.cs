// -----------------------------------------------------------------------
// <copyright file="CustomDamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DamageHandlers
{
    using System.Collections.Generic;

    using CustomPlayerEffects;
    using Enums;
    using Exiled.API.Extensions;
    using Items;
    using PlayerStatsSystem;
    using UnityEngine;

    using BaseFirearmHandler = PlayerStatsSystem.FirearmDamageHandler;
    using BaseHandler = PlayerStatsSystem.DamageHandlerBase;
    using BaseScpDamageHandler = PlayerStatsSystem.ScpDamageHandler;

    /// <summary>
    /// A wrapper to easily manipulate the behavior of <see cref="BaseHandler"/>.
    /// </summary>
    public sealed class CustomDamageHandler : AttackerDamageHandler
    {
        private static readonly Dictionary<ItemType, DamageType> ItemTypeToDamage = new()
        {
            [ItemType.GunCrossvec] = DamageType.Crossvec,
            [ItemType.GunLogicer] = DamageType.Logicer,
            [ItemType.GunRevolver] = DamageType.Revolver,
            [ItemType.GunShotgun] = DamageType.Shotgun,
            [ItemType.GunCOM15] = DamageType.Com15,
            [ItemType.GunCOM18] = DamageType.Com18,
            [ItemType.GunFSP9] = DamageType.Fsp9,
            [ItemType.GunE11SR] = DamageType.E11Sr,
            [ItemType.GunCom45] = DamageType.Com45,
            [ItemType.GunFRMG0] = DamageType.Frmg0,
            [ItemType.GunA7] = DamageType.A7,
            [ItemType.GunAK] = DamageType.AK,
        };

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDamageHandler"/> class.
        /// </summary>
        /// <param name="target">The target to be set.</param>
        /// <param name="baseHandler">The base <see cref="BaseHandler"/>.</param>
        public CustomDamageHandler(Player target, BaseHandler baseHandler)
            : base(target, baseHandler)
        {
            if (Attacker)
            {
                if (baseHandler is BaseScpDamageHandler)
                    CustomBase = new ScpDamageHandler(target, baseHandler);
                else if (Attacker.CurrentItem is not null && Attacker.CurrentItem.Type.IsWeapon() && baseHandler is BaseFirearmHandler)
                    CustomBase = new FirearmDamageHandler(Attacker.CurrentItem, target, baseHandler);
                else
                    CustomBase = new DamageHandler(target, Attacker);
            }
            else
            {
                CustomBase = new DamageHandler(target, baseHandler);
            }

            Type = CustomBase.Type;
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

            Firearm firearm = new(ItemType.GunAK)
            {
                Base = { Owner = attacker.ReferenceHub },
            };

            CustomBase = new FirearmDamageHandler(firearm, target, new BaseFirearmHandler(firearm.Base, damage));
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomDamageHandler"/> class.
        /// </summary>
        /// <param name="target">The target to be set.</param>
        /// <param name="attacker">The attacker to be set.</param>
        /// <param name="damage">The amount of damage to be set.</param>
        /// <param name="firearm">The <see cref="Firearm"/> to be used.</param>
        public CustomDamageHandler(Player target, Player attacker, float damage, Firearm firearm)
            : base(target, attacker)
        {
            Damage = damage;
            Type = ItemTypeToDamage[firearm.Type];

            if (firearm.Owner != attacker)
                firearm.ChangeOwner(firearm.Owner, attacker);

            CustomBase = new FirearmDamageHandler(firearm, target, new BaseFirearmHandler(firearm.Base, damage));
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
        public DamageHandlerBase CustomBase { get; }

        /// <inheritdoc/>
        public override Action ApplyDamage(Player player)
        {
            if (Damage <= 0f)
                return Action.None;

            StartVelocity = player.Velocity;

            BaseFirearmHandler baseFirearmHandler = null;

            if (Base is BaseFirearmHandler)
                baseFirearmHandler = Base as BaseFirearmHandler;

            if (baseFirearmHandler is not null)
                baseFirearmHandler.StartVelocity.y = Mathf.Max(baseFirearmHandler.StartVelocity.y, 0f);

            AhpStat ahpModule = player.GetModule<AhpStat>();
            HealthStat healthModule = player.GetModule<HealthStat>();

            if (Damage <= StandardDamageHandler.KillValue)
                return KillPlayer(player, CustomBase);

            ProcessDamage(player);

            if (baseFirearmHandler is not null)
            {
                foreach (StatusEffectBase statusEffect in player.ActiveEffects)
                {
                    if (statusEffect is IDamageModifierEffect damageModifierEffect)
                        Damage *= damageModifierEffect.GetDamageModifier(Damage, CustomBase, baseFirearmHandler.Hitbox);
                }
            }

            DealtHealthDamage = ahpModule.ServerProcessDamage(Damage);
            AbsorbedAhpDamage = Damage - DealtHealthDamage;

            return healthModule.CurValue - DealtHealthDamage > 0f ? Action.Damage : KillPlayer(player, CustomBase);
        }

        private static Action KillPlayer(Player player, DamageHandlerBase damageHandlerBase)
        {
            player.ReferenceHub.playerStats.KillPlayer(damageHandlerBase);

            return Action.Death;
        }
    }
}