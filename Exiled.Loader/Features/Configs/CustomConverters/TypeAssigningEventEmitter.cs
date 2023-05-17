// -----------------------------------------------------------------------
// <copyright file="TypeAssigningEventEmitter.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Linq;
using Exiled.API.Features;

namespace Exiled.Loader.Features.Configs.CustomConverters
{
    using System;

    using JetBrains.Annotations;
    using YamlDotNet.Core;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.EventEmitters;

    /// <summary>
    /// Event emitter which wraps all strings in double quotes.
    /// </summary>
    public class TypeAssigningEventEmitter : ChainedEventEmitter
    {
        private long count = 0;

        /// <inheritdoc cref="ChainedEventEmitter"/>
        public TypeAssigningEventEmitter([NotNull] IEventEmitter nextEmitter)
            : base(nextEmitter)
        {
        }

        /// <inheritdoc/>
        public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
        {
            Log.Info(eventInfo.Source.Value?.ToString() + ' ' + count + ' ' + (count % 2 != 0));

            if (eventInfo.Source.StaticType != typeof(object))
                count++;

            if (eventInfo.Source.StaticType != typeof(object) && Type.GetTypeCode(eventInfo.Source.StaticType) is TypeCode.String && count % 2 != 0)
                eventInfo.Style = ScalarStyle.SingleQuoted;

            base.Emit(eventInfo, emitter);
        }
    }
}