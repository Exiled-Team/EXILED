// -----------------------------------------------------------------------
// <copyright file="ScpDamage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Damage.Attacker
{
    using System.Linq;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using PlayerStatsSystem;

    /// <summary>
    /// A wrapper class for ScpDamageHandler.
    /// </summary>
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

        /// <summary>
        /// Initializes a new instance of the <see cref="ScpDamage"/> class.
        /// </summary>
        /// <param name="type">The <see cref="DamageType"/> to give.</param>
        /// <param name="damage">The ammount of damage to dealt.</param>
        /// <param name="attacker">The player who attack.</param>
        /// <returns>.</returns>
        public static new ScpDamage Create(DamageType type, float damage, Player attacker)
        {
            attacker ??= Server.Host;
            if (!CustomDamage.DamageTypeToCustomDamage.TryGetValue(type, out CustomDamage customDamage))
                customDamage = new();
            return new(new(attacker.ReferenceHub, damage, DamageTypeExtensions.TranslationConversion.FirstOrDefault(x => x.Value == type).Key));
        }
    }
}
