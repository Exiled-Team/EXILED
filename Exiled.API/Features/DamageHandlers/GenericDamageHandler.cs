// -----------------------------------------------------------------------
// <copyright file="GenericDamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DamageHandlers
{
    using System.Collections.Generic;

    using Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features.Pickups;
    using Footprinting;
    using InventorySystem.Items.MicroHID;
    using Items;
    using PlayerRoles.PlayableScps.Scp096;
    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.PlayableScps.Scp939;
    using PlayerStatsSystem;
    using UnityEngine;

    /// <summary>
    /// Allows generic damage to a player.
    /// </summary>
    public class GenericDamageHandler : CustomReasonDamageHandler
    {
        private const string DamageTextDefault = "You were damaged by Unknown Cause";

        private static readonly Dictionary<DamageType, DeathTranslation> DamageToTranslations = new()
        {
            [DamageType.Falldown] = DeathTranslations.Falldown,
            [DamageType.CardiacArrest] = DeathTranslations.CardiacArrest,
            [DamageType.Hypothermia] = DeathTranslations.Hypothermia,
            [DamageType.Asphyxiation] = DeathTranslations.Asphyxiated,
            [DamageType.Poison] = DeathTranslations.Poisoned,
            [DamageType.Bleeding] = DeathTranslations.Bleeding,
            [DamageType.Crushed] = DeathTranslations.Crushed,
            [DamageType.FemurBreaker] = DeathTranslations.UsedAs106Bait,
            [DamageType.PocketDimension] = DeathTranslations.PocketDecay,
            [DamageType.FriendlyFireDetector] = DeathTranslations.FriendlyFireDetector,
            [DamageType.SeveredHands] = DeathTranslations.SeveredHands,
            [DamageType.Decontamination] = DeathTranslations.Decontamination,
            [DamageType.Tesla] = DeathTranslations.Tesla,
            [DamageType.Scp] = DeathTranslations.Unknown,
            [DamageType.Scp207] = DeathTranslations.Scp207,
            [DamageType.Scp049] = DeathTranslations.Scp049,
            [DamageType.Scp173] = DeathTranslations.Scp173,
            [DamageType.Scp0492] = DeathTranslations.Zombie,
            [DamageType.Scp106] = DeathTranslations.PocketDecay,
            [DamageType.Scp3114] = DeathTranslations.Scp3114Slap,
        };

        private static readonly Dictionary<DamageType, ItemType> DamageToItemType = new()
        {
            [DamageType.Crossvec] = ItemType.GunCrossvec,
            [DamageType.Logicer] = ItemType.GunLogicer,
            [DamageType.Revolver] = ItemType.GunRevolver,
            [DamageType.Shotgun] = ItemType.GunShotgun,
            [DamageType.Com15] = ItemType.GunCOM15,
            [DamageType.Com18] = ItemType.GunCOM18,
            [DamageType.Fsp9] = ItemType.GunFSP9,
            [DamageType.E11Sr] = ItemType.GunE11SR,
            [DamageType.Com45] = ItemType.GunCom45,
            [DamageType.Frmg0] = ItemType.GunFRMG0,
            [DamageType.A7] = ItemType.GunA7,
            [DamageType.AK] = ItemType.GunAK,
            [DamageType.Firearm] = ItemType.GunAK,
        };

        private string genericDamageText;
        private string genericEnvironmentDamageText;
        private Player player;
        private DamageType damageType;
        private DamageHandlerBase.CassieAnnouncement customCassieAnnouncement;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericDamageHandler"/> class.
        /// Transform input data to custom generic handler.
        /// </summary>
        /// <param name="player">Current player (Target).</param>
        /// <param name="attacker">Attacker.</param>
        /// <param name="damage">Damage amount.</param>
        /// <param name="damageType">Damage type.</param>
        /// <param name="cassieAnnouncement">Custom cassie announcement.</param>
        /// <param name="damageText">Text to provide for player death screen.</param>
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
            genericEnvironmentDamageText = $"Environmental damage of type {damageType}";

            if (DamageToTranslations.TryGetValue(damageType, out DeathTranslation translation))
            {
                Base = damageType.IsScp() ?
                    new PlayerStatsSystem.ScpDamageHandler(attacker.ReferenceHub, damage, translation) :
                    new UniversalDamageHandler(damage, translation, cassieAnnouncement);
            }

            if (Base is null && DamageToItemType.TryGetValue(damageType, out ItemType itemType))
                GenericFirearm(player, attacker, damage, damageType, itemType);

            switch (damageType)
            {
                case DamageType.Warhead:
                    Base = new WarheadDamageHandler();
                    break;
                case DamageType.Scp018:
                    Base = new Scp018DamageHandler(Pickup.Create(ItemType.SCP018).Cast<Pickups.Projectiles.Scp018Projectile>().Base, damage, IgnoreFriendlyFire);
                    break;
                case DamageType.Recontainment:
                    Base = new RecontainmentDamageHandler(Attacker);
                    break;
                case DamageType.Jailbird:
                    Base = new JailbirdDamageHandler(Attacker.Hub, damage, Vector3.zero);
                    break;
                case DamageType.MicroHid:
                    MicroHIDItem microHidOwner = new GameObject().AddComponent<MicroHIDItem>();
                    microHidOwner.Owner = attacker.ReferenceHub;
                    Base = new MicroHidDamageHandler(microHidOwner, damage);
                    break;
                case DamageType.Explosion:
                    Base = new ExplosionDamageHandler(attacker.Footprint, UnityEngine.Vector3.zero, damage, 0);
                    break;
                case DamageType.ParticleDisruptor:
                    Base = new DisruptorDamageHandler(Attacker, damage);
                    break;
                case DamageType.Scp096:
                    Scp096Role curr096 = attacker.Role.Is(out Roles.Scp096Role scp096) ? scp096.Base : new GameObject().AddComponent<Scp096Role>();
                    curr096._lastOwner = attacker.ReferenceHub;
                    Base = new Scp096DamageHandler(scp096.Base, damage, Scp096DamageHandler.AttackType.GateKill);
                    break;
                case DamageType.Scp939:
                    Scp939Role curr939 = attacker.Role.Is(out Roles.Scp939Role scp939) ? scp939.Base : new GameObject().AddComponent<Scp939Role>();
                    curr939._lastOwner = attacker.ReferenceHub;
                    Base = new Scp939DamageHandler(curr939, damage, Scp939DamageType.LungeTarget);
                    break;
                case DamageType.Strangled:
                    new Scp3114DamageHandler(attacker.ReferenceHub, damage, Scp3114DamageHandler.HandlerType.Strangulation);
                    break;
                case DamageType.Marshmallow:
                case DamageType.Custom:
                case DamageType.Unknown:
                default:
                    Base = new CustomReasonDamageHandler(damageText ?? genericDamageText, damage, cassieAnnouncement.Announcement);
                    break;
            }
        }

        /// <summary>
        /// Gets or sets a custom base.
        /// </summary>
        public PlayerStatsSystem.DamageHandlerBase Base { get; set; }

        /// <summary>
        /// Gets or sets the current attacker.
        /// </summary>
        public Footprint Attacker { get; set; }

        /// <summary>
        /// Gets a value indicating whether allow self damage.
        /// </summary>
        public bool AllowSelfDamage { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the friendly fire rules should be ignored.
        /// </summary>
        public bool IgnoreFriendlyFire { get; set; }

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
        /// <param name="player">Current player.</param>
        /// <param name="attacker">Current attacker.</param>
        /// <param name="amount">Damage amount.</param>
        /// <param name="damageType">Damage type.</param>
        /// <param name="itemType">ItemType.</param>
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
