// -----------------------------------------------------------------------
// <copyright file="ScpDamageHandler.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.DamageHandlers
{
    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    using PlayerStatsSystem;

    using BaseHandler = PlayerStatsSystem.DamageHandlerBase;

    using BaseScpHandler = PlayerStatsSystem.ScpDamageHandler;

    /// <summary>
    /// A wrapper to easily manipulate the behavior of <see cref="BaseHandler"/>.
    /// </summary>
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
                    case Scp096DamageHandler _:
                        return DamageType.Scp096;
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
    }
}
