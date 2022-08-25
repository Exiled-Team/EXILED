// -----------------------------------------------------------------------
// <copyright file="GenericDamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DamageHandlers
{
    using Exiled.API.Enums;
    using Exiled.API.Features.Items;

    using Footprinting;

    using PlayerStatsSystem;

    /// <summary>
    /// Allows generic damage to player.
    /// </summary>
    public class GenericDamageHandler : PlayerStatsSystem.CustomReasonDamageHandler
    {
        private const string DamageTextDefault = "You were damaged by Unknown Cause";
        private string genericDamageText;
        private string genericEnvironmentDamageText;
        private Player player;
        private DamageType damageType;
        private Features.DamageHandlers.DamageHandlerBase.CassieAnnouncement customCassieAnnouncement;

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
        public GenericDamageHandler(Player player, Player attacker, float damage, DamageType damageType, Features.DamageHandlers.DamageHandlerBase.CassieAnnouncement cassieAnnouncement, string damageText = null)
            : base(DamageTextDefault)
        {
            this.player = player;
            this.damageType = damageType;
            this.customCassieAnnouncement = cassieAnnouncement;
            if (this.customCassieAnnouncement is not null)
            {
                this.customCassieAnnouncement.Announcement = customCassieAnnouncement.Announcement ?? $"{player.Nickname} killed by {attacker.Nickname} utilizing {damageType}";
            }

            this.Attacker = attacker.Footprint;
            this.AllowSelfDamage = true;
            this.Damage = damage;
            this.ServerLogsText = $"GenericDamageHandler damage processing";
            this.genericDamageText = $"You were damaged by {damageType}";
            this.genericEnvironmentDamageText = $"Environemntal damage of type {damageType}";

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
                case DamageType.ParticleDisruptor:
                    Base = new DisruptorDamageHandler(Attacker, damage);
                    break;

                case DamageType.Scp096:
                    PlayableScps.Scp096 curr096 = attacker.CurrentScp as PlayableScps.Scp096 ?? new PlayableScps.Scp096();
                    if (curr096 != null)
                    {
                        curr096.Hub = attacker.ReferenceHub;
                    }

                    Base = new Scp096DamageHandler(curr096, damage, Scp096DamageHandler.AttackType.Slap);
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
                case DamageType.Scp939:
                    Base = new PlayerStatsSystem.ScpDamageHandler(attacker.ReferenceHub, damage, DeathTranslations.Scp939);
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
                if (this.customCassieAnnouncement?.Announcement != null)
                {
                    Cassie.Message(this.customCassieAnnouncement.Announcement);
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
