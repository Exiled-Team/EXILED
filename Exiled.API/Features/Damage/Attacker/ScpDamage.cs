// -----------------------------------------------------------------------
// <copyright file="ScpDamage.cs" company="Exiled Team">
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
    using Exiled.API.Extensions;
    using PlayerStatsSystem;

    public class ScpDamage : AttackerDamage
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScpDamage"/> class.
        /// </summary>
        /// <param name="damageHandler">The base <see cref="ScpDamageHandler"/> class.</param>
        internal ScpDamage(ScpDamageHandler damageHandler)
            : base(damageHandler)
        {
            Base = damageHandler;

            DeathTranslation translation = DeathTranslations.TranslationsById[Base._translationId];
            if (translation.Id == DeathTranslations.PocketDecay.Id)
                Type = DamageType.Scp106;
            Type = DamageTypeExtensions.TranslationIdConversion.ContainsKey(translation.Id) ? DamageTypeExtensions.TranslationIdConversion[translation.Id] : DamageType.Scp;

        }

        /// <summary>
        /// Gets the <see cref="ScpDamageHandler"/> that this class is encapsulating.
        /// </summary>
        public new ScpDamageHandler Base { get; }

        /// <inheritdoc/>
        public override DamageType Type { get; internal set; }
    }
}
