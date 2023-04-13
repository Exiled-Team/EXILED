// -----------------------------------------------------------------------
// <copyright file="DamageBase.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features.Damage.Attacker;
    using PlayerRoles.PlayableScps.Scp939;
    using PlayerStatsSystem;

    /// <summary>
    /// A wrapper class for DamageHandlerBase.
    /// </summary>
    public class DamageBase
    {
        /// <summary>
        /// .
        /// </summary>
        public const float KillValue = StandardDamageHandler.KillValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="DamageBase"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="DamageHandlerBase"/> class.</param>
        internal DamageBase(DamageHandlerBase damageHandler) => Base = damageHandler;

        /// <summary>
        /// Gets the <see cref="DamageHandlerBase"/> of the item.
        /// </summary>
        public DamageHandlerBase Base { get; }

        /// <summary>
        /// Gets the <see cref="DamageType"/> for the damage.
        /// </summary>
        public virtual DamageType Type { get; internal set; }

        /// <summary>
        /// Gets .
        /// </summary>
        public string ServerLogsText => Base.ServerLogsText;

        /// <summary>
        /// Gets the <see cref="Name"/> for the damage.
        /// </summary>
        public string Name => Enum.IsDefined(typeof(DamageType), Type) ? Type.ToString() : CustomDamage.DamageTypeToCustomDamage.TryGetValue(Type, out CustomDamage customDamage) ? customDamage.DamageName : "Unknown";

        /// <summary>
        /// Gets .
        /// </summary>
        public DamageHandlerBase.CassieAnnouncement CassieAnnouncement => Base.CassieDeathAnnouncement;

        /// <summary>
        /// Gets an existing <see cref="StandardDamage"/> or creates a new instance of one.
        /// </summary>
        /// <param name="damageHandler">The <see cref="DamageHandlerBase"/> to convert into a <see cref="StandardDamage"/>.</param>
        /// <returns>The <see cref="StandardDamage"/> wrapper for the given <see cref="DamageHandlerBase"/>.</returns>
        public static StandardDamage Get(DamageHandlerBase damageHandler) => damageHandler switch
        {
            CustomReasonDamageHandler customReasonDamageHandler => new CustomReasonDamage(customReasonDamageHandler),
            AttackerDamageHandler attackerDamageHandler => attackerDamageHandler switch
            {
                DisruptorDamageHandler disruptorDamageHandler => new DisruptorDamage(disruptorDamageHandler),
                ExplosionDamageHandler explosionDamageHandler => new ExplosionDamage(explosionDamageHandler),
                FirearmDamageHandler firearmDamageHandler => new FirearmDamage(firearmDamageHandler),
                JailbirdDamageHandler jailbirdDamageHandler => new JailbirdDamage(jailbirdDamageHandler),
                MicroHidDamageHandler microHidDamageHandler => new MicroHidDamage(microHidDamageHandler),
                RecontainmentDamageHandler recontainmentDamageHandler => new RecontainmentDamage(recontainmentDamageHandler),
                Scp018DamageHandler scp018DamageHandler => new Scp018Damage(scp018DamageHandler),
                Scp049DamageHandler scp049DamageHandler => new Scp049Damage(scp049DamageHandler),
                Scp096DamageHandler scp096DamageHandler => new Scp096Damage(scp096DamageHandler),
                Scp939DamageHandler scp939DamageHandler => new Scp939Damage(scp939DamageHandler),
                ScpDamageHandler scpDamageHandler => new ScpDamage(scpDamageHandler),
                _ => new AttackerDamage(attackerDamageHandler),
            },
            WarheadDamageHandler warheadDamageHandler => new WarheadDamage(warheadDamageHandler),
            UniversalDamageHandler universalDamageHandler => new UniversalDamage(universalDamageHandler),
            StandardDamageHandler standardDamageHandler => new StandardDamage(standardDamageHandler),
            _ => null,
        };

        /// <summary>
        /// Creates and returns a new <see cref="DamageBase"/> with the proper inherited subclass.
        /// </summary>
        /// <param name="type">The <see cref="DamageType"/> of the pickup.</param>
        /// <param name="damage">The <see cref="StandardDamage.Damage"/> of the pickup.</param>
        /// <param name="attacker">The <see cref="Player"/> who make the attack.</param>
        /// <returns>The created <see cref="StandardDamage"/>.</returns>
        public static StandardDamage Create(DamageType type, float damage = KillValue, Player attacker = null) => type switch
        {
            DamageType.Firearm or DamageType.AK or DamageType.Crossvec or DamageType.Logicer or DamageType.E11Sr or DamageType.Shotgun or DamageType.Fsp9 or DamageType.Com15 or DamageType.Com18 or DamageType.Com45 or DamageType.MicroHid => FirearmDamage.Create(type, damage, attacker),
            DamageType.ParticleDisruptor => DisruptorDamage.Create(damage, attacker),
            DamageType.Warhead => WarheadDamage.Create(damage),
            DamageType.Explosion => ExplosionDamage.Create(damage, attacker),
            DamageType.Scp or DamageType.Scp173 or DamageType.Scp106 or DamageType.Scp096Charge or DamageType.Scp096Gate or DamageType.Scp096SlapLeft or DamageType.Scp096SlapRight => ScpDamage.Create(type, damage, attacker),
            DamageType.Scp049 or DamageType.Scp0492 or DamageType.CardiacArrest => Scp049Damage.Create(type, damage, attacker),
            DamageType.Scp939Claw or DamageType.Scp939LungeTarget or DamageType.Scp939LungeSecondary => Scp939Damage.Create(type, damage, attacker),
            DamageType.Scp018 => UniversalDamage.Create(type, damage),
            DamageType.Custom or _ => CustomReasonDamage.Create(type, damage),
        };

        /// <summary>
        /// .
        /// </summary>
        /// <param name="player">..</param>
        /// <returns>...</returns>
        public DamageHandlerBase.HandlerOutput ApplyDamage(Player player) => Base.ApplyDamage(player.ReferenceHub);

        /// <inheritdoc/>
        public override string ToString() => $"{Type} {Name}";
    }
}
