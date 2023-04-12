// -----------------------------------------------------------------------
// <copyright file="DamageBase.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Exiled.API.Enums;
    using Exiled.API.Features.Damage.Attacker;
    using PlayerStatsSystem;

    public class DamageBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DamageBase"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="DamageHandlerBase"/> class.</param>
        internal DamageBase(DamageHandlerBase damageHandler)
        {
            Base = damageHandler;
        }

        /// <summary>
        /// Gets the <see cref="DamageHandlerBase"/> of the item.
        /// </summary>
        public DamageHandlerBase Base { get; }

        /// <summary>
        /// Gets the <see cref="DamageType"/> for the damage.
        /// </summary>
        public virtual DamageType Type { get; }

        /// <summary>
        /// .
        /// </summary>
        /// <returns>..</returns>
        public static DamageBase Get(DamageHandlerBase damageHandler)
        {
            if (damageHandler == null)
                return null;

            return damageHandler switch
            {
                StandardDamageHandler standardDamage => standardDamage switch
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
                        ScpDamageHandler scpDamageHandler => new ScpDamage(scpDamageHandler),
                        _ => new AttackerDamage(attackerDamageHandler),
                    },
                    WarheadDamageHandler warheadDamageHandler => new WarheadDamage(warheadDamageHandler),
                    UniversalDamageHandler universalDamageHandler => new UniversalDamage(universalDamageHandler),
                    _ => new StandardDamage(standardDamage),
                },
                _ => new DamageBase(damageHandler),
            };
        }
    }
}
