// -----------------------------------------------------------------------
// <copyright file="ScpDamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DamageHandlers
{
    using System.Diagnostics;

    using Enums;

    using Extensions;

    using PlayerStatsSystem;

    using BaseHandler = PlayerStatsSystem.DamageHandlerBase;
    using BaseScpHandler = PlayerStatsSystem.ScpDamageHandler;

    /// <summary>
    /// A wrapper to easily manipulate the behavior of <see cref="BaseHandler"/>.
    /// </summary>
    [DebuggerDisplay("{" + nameof(DebuggerDisplay) + ",nq}")]
    public sealed class ScpDamageHandler : DamageHandler
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ScpDamageHandler"/> class.
        /// </summary>
        /// <param name="target">The target to be set.</param>
        /// <param name="baseHandler"><inheritdoc cref="DamageHandlerBase.Base"/></param>
        public ScpDamageHandler(Player target, BaseHandler baseHandler)
            : base(target, baseHandler)
        {
        }

        /// <inheritdoc/>
        public override DamageType Type
        {
            get
            {
                switch (Base)
                {
                    case Scp096DamageHandler:
                        return DamageType.Scp096;
                    case Scp049DamageHandler scp049DamageHandler:
                        return scp049DamageHandler.DamageSubType switch
                        {
                            Scp049DamageHandler.AttackType.Scp0492 => DamageType.Scp0492,
                            _ => DamageType.Scp049,
                        };
                    case BaseScpHandler scp:
                        {
                            DeathTranslation translation = DeathTranslations.TranslationsById[scp._translationId];
                            if (translation.Id == DeathTranslations.PocketDecay.Id)
                                return DamageType.Scp106;
                            return DamageTypeExtensions.TranslationIdConversion.ContainsKey(translation.Id) ? DamageTypeExtensions.TranslationIdConversion[translation.Id] : DamageType.Scp;
                        }

                    default:
                        return base.Type;
                }
            }
        }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay
        {
            get
            {
                string debugView = $"ScpDamageHandler Damage = {Damage}";
                if (Attacker is not null)
                    debugView += $" Attacker = {Attacker.Nickname}";
                if (Target is not null)
                    debugView += $" Target = {Target.Nickname}";
                return debugView;
            }
        }
    }
}