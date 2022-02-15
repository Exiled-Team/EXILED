// -----------------------------------------------------------------------
// <copyright file="ScpDamageHandler.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Features.DamageHandlers
{
    using System;

    using SEXiled.API.Enums;
    using SEXiled.API.Features.Items;

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
                        return TranslationConversion.ContainsKey(translation) ? TranslationConversion[translation] : DamageType.Scp;
                    }

                    default:
                        return base.Type;
                }
            }
        }
    }
}
