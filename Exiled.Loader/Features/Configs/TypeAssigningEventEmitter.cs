// -----------------------------------------------------------------------
// <copyright file="TypeAssigningEventEmitter.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Exiled.API.Enums;

namespace Exiled.Loader.Features.Configs
{
    using System;

    using Exiled.API.Features;
    using YamlDotNet.Core;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.EventEmitters;

    /// <summary>
    /// Event emitter which wraps all strings in double quotes.
    /// </summary>
    public class TypeAssigningEventEmitter : ChainedEventEmitter
    {
        /// <inheritdoc cref="ChainedEventEmitter"/>
        public TypeAssigningEventEmitter(IEventEmitter nextEmitter)
            : base(nextEmitter)
        {
        }

        /// <inheritdoc/>
        public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
        {
            if (eventInfo.Source.StaticType != typeof(object) && Type.GetTypeCode(eventInfo.Source.StaticType) == TypeCode.String && !UnderscoredNamingConvention.Instance.Properties.Contains(eventInfo.Source.Value))
            {
                eventInfo.Style = LoaderPlugin.Config.QuotesType switch
                {
                    QuotesType.SingleQuoted => ScalarStyle.SingleQuoted,
                    QuotesType.DoubleQuoted => ScalarStyle.DoubleQuoted,
                    _ => ScalarStyle.Any
                };
            }

            base.Emit(eventInfo, emitter);
        }
    }
}
