// -----------------------------------------------------------------------
// <copyright file="GenericDamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DamageHandlers
{
    using Enums;

    using Footprinting;

    using Items;

    using PlayerRoles.PlayableScps.Scp096;
    using PlayerRoles.PlayableScps.Scp939;

    using PlayerStatsSystem;

    using UnityEngine;

    /// <summary>
    /// Allows generic damage to player.
    /// </summary>
    public class GenericDamageHandler : CustomReasonDamageHandler
    {
        private const string DamageTextDefault = "You were damaged by Unknown Cause";
        private string genericDamageText;
        private string genericEnvironmentDamageText;
        private Player player;
        private DamageType damageType;
        private DamageHandlerBase.CassieAnnouncement customCassieAnnouncement;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDamageHandler"/> class.
        /// Transform input data to custom generic handler.
        /// </summary>
        /// <param name="player"> Current player (Target). </param>
        /// <param name="attacker"> Attacker. </param>
        /// <param name="damage"> Damage quantity. </param>
        /// <param name="damageType"> Damage type. </param>
        /// <param name="cassieAnnouncement"> Custom cassie announcment. </param>
        /// <param name="damageText"> Text to provide to player death screen. </param>
        public GenericDamageHandler(Player player, Player attacker, float damage, DamageType damageType, DamageHandlerBase.CassieAnnouncement cassieAnnouncement, string damageText = null)
            : base(DamageTextDefault)
        {
            this.player = player;
            this.damageType = damageType;
            cassieAnnouncement ??= DamageHandlerBase.CassieAnnouncement.Default;
            customCassieAnnouncement = cassieAnnouncement;

            if (customCassieAnnouncement is not null)
                customCassieAnnouncement.Announcement ??= $"{player.Nickname} killed by {attacker.Nickname} utilizing {damageType}";

            Attacker = attacker.Footprint;
            AllowSelfDamage = true;
            Damage = damage;
            ServerLogsText = $"GenericDamageHandler damage processing";
            genericDamageText = $"You were damaged by {damageType}";
            genericEnvironmentDamageText = $"Environemntal damage of type {damageType}";

            switch (damageType)
            {
                case DamageType.Falldown:
                    Base = new UniversalDamageHandler(damage, DeathTranslations.Falldown, cassieAnnouncement);
                    break;
                case DamageType.Hypothermia:
                    Base = new UniversalDamageHandler(damage, DeathTranslations.Hypothermia, cassieAnnouncement);
                    break;
                case DamageType.Asphyxiation:
                    Base = new UniversalDamageHandler(damage, DeathTranslations.Asphyxiated, cassieAnnouncement);
                    break;
                case DamageType.Poison:
                    Base = new UniversalDamageHandler(damage, DeathTranslations.Poisoned, cassieAnnouncement);
                    break;
                case DamageType.Bleeding:
                    Base = new UniversalDamageHandler(damage, DeathTranslations.Bleeding, cassieAnnouncement);
                    break;
                case DamageType.Crushed:
                    Base = new UniversalDamageHandler(damage, DeathTranslations.Crushed, cassieAnnouncement);
                    break;
                case DamageType.FemurBreaker:
                    Base = new UniversalDamageHandler(damage, DeathTranslations.UsedAs106Bait, cassieAnnouncement);
                    break;
                case DamageType.PocketDimension:
                    Base = new UniversalDamageHandler(damage, DeathTranslations.PocketDecay, cassieAnnouncement);
                    break;
                case DamageType.FriendlyFireDetector:
                    Base = new UniversalDamageHandler(damage, DeathTranslations.FriendlyFireDetector, cassieAnnouncement);
                    break;
                case DamageType.SeveredHands:
                    Base = new UniversalDamageHandler(damage, DeathTranslations.SeveredHands, cassieAnnouncement);
                    break;
                case DamageType.Warhead:
                    Base = new WarheadDamageHandler();
                    break;
                case DamageType.Decontamination:
                    Base = new UniversalDamageHandler(damage, DeathTranslations.Decontamination, cassieAnnouncement);
                    break;
                case DamageType.Tesla:
                    Base = new UniversalDamageHandler(damage, DeathTranslations.Tesla, cassieAnnouncement);
                    break;
                case DamageType.Recontainment:
                    Base = new RecontainmentDamageHandler(Attacker);
                    break;
                case DamageType.Jailbird:
                    Base = new JailbirdDamageHandler(Attacker.Hub, damage, Vector3.zero);
                    break;
                case DamageType.MicroHid:
                    InventorySystem.Items.MicroHID.MicroHIDItem microHidOwner = new();
                    microHidOwner.Owner = attacker.ReferenceHub;
                    Base = new MicroHidDamageHandler(microHidOwner, damage);
                    break;
                case DamageType.Explosion:
                    Base = new ExplosionDamageHandler(attacker.Footprint, UnityEngine.Vector3.zero, damage, 0);
                    break;
                case DamageType.Firearm:
                    GenericFirearm(player, attacker, damage, damageType, ItemType.GunAK);
                    break;
                case DamageType.Crossvec:
                    GenericFirearm(player, attacker, damage, damageType, ItemType.GunCrossvec);
                    break;
                case DamageType.Logicer:
                    GenericFirearm(player, attacker, damage, damageType, ItemType.GunLogicer);
                    break;
                case DamageType.Revolver:
                    GenericFirearm(player, attacker, damage, damageType, ItemType.GunRevolver);
                    break;
                case DamageType.Shotgun:
                    GenericFirearm(player, attacker, damage, damageType, ItemType.GunShotgun);
                    break;
                case DamageType.AK:
                    GenericFirearm(player, attacker, damage, damageType, ItemType.GunAK);
                    break;
                case DamageType.Com15:
                    GenericFirearm(player, attacker, damage, damageType, ItemType.GunCOM15);
                    break;
                case DamageType.Com18:
                    GenericFirearm(player, attacker, damage, damageType, ItemType.GunCOM18);
                    break;
                case DamageType.Fsp9:
                    GenericFirearm(player, attacker, damage, damageType, ItemType.GunFSP9);
                    break;
                case DamageType.E11Sr:
                    GenericFirearm(player, attacker, damage, damageType, ItemType.GunE11SR);
                    break;
                case DamageType.Com45:
                    GenericFirearm(player, attacker, damage, damageType, ItemType.GunCom45);
                    break;
                case DamageType.ParticleDisruptor:
                    Base = new DisruptorDamageHandler(Attacker, damage);
                    break;
                case DamageType.Scp096:
                    Scp096Role curr096 = attacker.ReferenceHub.roleManager.CurrentRole as Scp096Role ?? new Scp096Role();

                    if (curr096 != null)
                        curr096._lastOwner = attacker.ReferenceHub;

                    Base = new Scp096DamageHandler(curr096, damage, Scp096DamageHandler.AttackType.SlapRight);
                    break;
                case DamageType.Scp939:
                    Scp939Role curr939 = attacker.ReferenceHub.roleManager.CurrentRole as Scp939Role ?? new Scp939Role();

                    if (curr939 != null)
                        curr939._lastOwner = attacker.ReferenceHub;

                    Base = new Scp939DamageHandler(curr939, Scp939DamageType.LungeTarget) { Damage = damage, };
                    break;
                case DamageType.Scp:
                    Base = new PlayerStatsSystem.ScpDamageHandler(attacker.ReferenceHub, damage, DeathTranslations.Unknown);
                    break;
                case DamageType.Scp018:
                    Base = new PlayerStatsSystem.ScpDamageHandler(attacker.ReferenceHub, damage, DeathTranslations.Unknown);
                    break;
                case DamageType.Scp207:
                    Base = new PlayerStatsSystem.ScpDamageHandler(attacker.ReferenceHub, damage, DeathTranslations.Scp207);
                    break;
                case DamageType.Scp049:
                    Base = new PlayerStatsSystem.ScpDamageHandler(attacker.ReferenceHub, damage, DeathTranslations.Scp049);
                    break;
                case DamageType.Scp173:
                    Base = new PlayerStatsSystem.ScpDamageHandler(attacker.ReferenceHub, damage, DeathTranslations.Scp173);
                    break;
                case DamageType.Scp0492:
                    Base = new PlayerStatsSystem.ScpDamageHandler(attacker.ReferenceHub, damage, DeathTranslations.Zombie);
                    break;
                case DamageType.Scp106:
                    Base = new PlayerStatsSystem.ScpDamageHandler(attacker.ReferenceHub, damage, DeathTranslations.PocketDecay);
                    break;
                case DamageType.Custom:
                case DamageType.Unknown:
                default:
                    Base = new CustomReasonDamageHandler(damageText ?? genericDamageText, damage, cassieAnnouncement.Announcement);
                    break;
            }
        }

        /// <summary>
        /// Gets or sets custom base.
        /// </summary>
        public PlayerStatsSystem.DamageHandlerBase Base { get; set; }

        /// <summary>
        /// Gets or sets current attacker.
        /// </summary>
        public Footprint Attacker { get; set; }

        /// <summary>
        /// Gets a value indicating whether allow self damage.
        /// </summary>
        public bool AllowSelfDamage { get; }

        /// <inheritdoc />
        public override float Damage { get; set; }

        /// <inheritdoc />
        public override string ServerLogsText { get; }

        /// <summary>
        /// Custom Exiled process damage.
        /// </summary>
        /// <param name="ply"> Current player hub. </param>
        /// <returns> Handles processing damage outcome. </returns>
        public override HandlerOutput ApplyDamage(ReferenceHub ply)
        {
            HandlerOutput output = base.ApplyDamage(ply);
            if (output is HandlerOutput.Death)
            {
                if (customCassieAnnouncement?.Announcement != null)
                {
                    Cassie.Message(customCassieAnnouncement.Announcement);
                }
            }

            return output;
        }

        /// <summary>
        /// Generic firearm path for handle type.
        /// </summary>
        /// <param name="player"> Current player. </param>
        /// <param name="attacker"> Current attacker. </param>
        /// <param name="amount"> Damage amount. </param>
        /// <param name="damageType"> Damage type. </param>
        /// <param name="itemType"> ItemType. </param>
        private void GenericFirearm(Player player, Player attacker, float amount, DamageType damageType, ItemType itemType)
        {
            Firearm firearm = new(itemType)
            {
                Base =
                {
                    Owner = attacker.ReferenceHub,
                },
            };
            Base = new PlayerStatsSystem.FirearmDamageHandler(firearm.Base, amount);
        }
    }
}