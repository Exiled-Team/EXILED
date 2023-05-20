// -----------------------------------------------------------------------
// <copyright file="TypeAssigningEventEmitter.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Features.Configs
{
    using System;
    using System.Linq;

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
            if (UnderscoredNamingConvention.Instance.Properties.FirstOrDefault() == eventInfo.Source.Value)
            {
                UnderscoredNamingConvention.Instance.Properties.RemoveAt(0);
            }
            else
            {
                if (eventInfo.Source.StaticType != typeof(object) && Type.GetTypeCode(eventInfo.Source.StaticType) == TypeCode.String)
                    eventInfo.Style = LoaderPlugin.Config.ScalarStyle;
            }

            base.Emit(eventInfo, emitter);
        }
    }
}
