// -----------------------------------------------------------------------
// <copyright file="GenericDamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features.Items;

    using Footprinting;

    using PlayerStatsSystem;

    using Subtitles;

    /// <summary>
    /// Allows generic damage to player.
    /// </summary>
    internal class GenericDamageHandler : PlayerStatsSystem.CustomReasonDamageHandler
    {
        private Player player;
        private DamageType damageType;
        private DamageHandlers.DamageHandlerBase.CassieAnnouncement customCassieAnnouncement;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDamageHandler"/> class.
        /// Transform input data to custom generic handler.
        /// </summary>
        /// <param name="player"> Current player (Target). </param>
        /// <param name="attacker"> Attacker. </param>
        /// <param name="damage"> Damage quantity. </param>
        /// <param name="damageType"> Damage type. </param>
        /// <param name="cassieAnnouncement"> Custom cassie announcment. </param>
        public GenericDamageHandler(Player player, Player attacker, float damage, DamageType damageType, DamageHandlers.DamageHandlerBase.CassieAnnouncement cassieAnnouncement)
            : base($"You were damaged by {damageType}")
        {
            this.player = player;
            this.damageType = damageType;
            this.customCassieAnnouncement = cassieAnnouncement;

            this.Attacker = attacker.Footprint;
            this.AllowSelfDamage = true;
            this.Damage = damage;
            this.ServerLogsText = $"You were damaged by {damageType}";

            switch (damageType)
            {
                case DamageType.Falldown:
                case DamageType.Hypothermia:
                case DamageType.Asphyxiation:
                case DamageType.Poison:
                case DamageType.Bleeding:
                case DamageType.Crushed:
                case DamageType.FemurBreaker:
                case DamageType.PocketDimension:
                case DamageType.FriendlyFireDetector:
                case DamageType.SeveredHands:
                    Base = new CustomReasonDamageHandler($"You were damaged by {damageType}", damage, cassieAnnouncement.Announcement);
                    break;
                case DamageType.Warhead:
                case DamageType.Decontamination:
                case DamageType.Tesla:
                    Base = new CustomReasonDamageHandler($"You were damaged by {damageType}", damage, cassieAnnouncement.Announcement);
                    break;

                case DamageType.Recontainment:
                    Base = new RecontainmentDamageHandler(Attacker);
                    break;

                case DamageType.Firearm:
                    GenericFirearm(player, attacker, damage, damageType, ItemType.GunAK);
                    break;
                case DamageType.MicroHid:
                    GenericFirearm(player, attacker, damage, damageType, ItemType.MicroHID);
                    break;
                case DamageType.Explosion:
                    GenericFirearm(player, attacker, damage, damageType, ItemType.GrenadeHE);
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
                case DamageType.Scp018:
                case DamageType.Scp207:
                case DamageType.Scp049:
                case DamageType.Scp173:
                case DamageType.Scp939:
                case DamageType.Scp0492:
                case DamageType.Scp106:
                    Base = new ScpDamageHandler(attacker.ReferenceHub, damage, DeathTranslations.Unknown);
                    break;
                case DamageType.Custom:
                case DamageType.Unknown:
                default:
                    Base = new CustomReasonDamageHandler($"You were damaged by {damageType}", damage, string.IsNullOrEmpty(cassieAnnouncement?.Announcement) ? $"{player.Nickname} killed by {attacker.Nickname} utilizing {damageType}" : cassieAnnouncement.Announcement);
                    break;
            }
        }

        /// <summary>
        /// Gets or sets custom base.
        /// </summary>
        public DamageHandlerBase Base { get; set; }

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
        /// Process damage for this custom damage source.
        /// </summary>
        /// <param name="ply"> Current player reference hub. </param>
        public override void ProcessDamage(ReferenceHub ply)
        {
            base.ProcessDamage(ply);
        }

        /// <summary>
        /// Custom Exiled process damage.
        /// </summary>
        /// <param name="ply"> Current player hub. </param>
        /// <returns> Handles processing damage outcome. </returns>
        public override HandlerOutput ApplyDamage(ReferenceHub ply)
        {
            HandlerOutput output = base.ApplyDamage(ply);
            if(output == HandlerOutput.Death)
            {
                this._deathReason = $"You were killed by {damageType}";

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
            Base = new FirearmDamageHandler(firearm.Base, amount);
        }
    }
}
