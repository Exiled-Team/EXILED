// -----------------------------------------------------------------------
// <copyright file="DamageBase.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DamageHandlers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using PlayerStatsSystem;

    public class DamageBase
    {
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
                        DisruptorDamageHandler disruptorDamageHandler => new CustomReasonDamage(disruptorDamageHandler),
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
                    _ => new StandardDamageHandler(standardDamage),
                },
                _ => new DamageBase(damageHandler),
            };
        }
    }
}
