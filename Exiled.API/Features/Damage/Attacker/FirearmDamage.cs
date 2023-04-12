// -----------------------------------------------------------------------
// <copyright file="FirearmDamage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage.Attacker
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;

    using Exiled.API.Enums;
    using PlayerStatsSystem;

    public class FirearmDamage : AttackerDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FirearmDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="FirearmDamageHandler"/> class.</param>
        internal FirearmDamage(FirearmDamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;
            Type = Base.WeaponType switch
            {
                ItemType.GunAK => DamageType.AK,
                ItemType.GunCOM15 => DamageType.Com15,
                ItemType.GunCOM18 => DamageType.Com18,
                ItemType.GunCom45 => DamageType.Com45,
                ItemType.GunCrossvec => DamageType.Crossvec,
                ItemType.GunE11SR => DamageType.E11Sr,
                ItemType.GunFSP9 => DamageType.Fsp9,
                ItemType.GunLogicer => DamageType.Logicer,
                ItemType.GunRevolver => DamageType.Revolver,
                ItemType.GunShotgun => DamageType.Shotgun,
                _ => DamageType.Firearm,
            };
        }

        /// <summary>
        /// Gets the <see cref="FirearmDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new FirearmDamageHandler Base { get; }

        /// <inheritdoc/>
        public override DamageType Type { get; internal set; }
    }
}
