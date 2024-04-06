// -----------------------------------------------------------------------
// <copyright file="TypeAssigningEventEmitter.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Serialization
{
    using System;

    using YamlDotNet.Core;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.EventEmitters;

    /// <summary>
    /// Event emitter which wraps all strings in double quotes.
    /// </summary>
    public class TypeAssigningEventEmitter : ChainedEventEmitter
    {
        private readonly char[] multiline = new char[] { '\r', '\n', '\x85', '\x2028', '\x2029' };

        /// <inheritdoc cref="ChainedEventEmitter"/>
        public TypeAssigningEventEmitter(IEventEmitter nextEmitter)
            : base(nextEmitter)
        {
        }

        /// <inheritdoc/>
        public override void Emit(ScalarEventInfo eventInfo, IEmitter emitter)
        {
            if (eventInfo.Source.StaticType != typeof(object) && Type.GetTypeCode(eventInfo.Source.StaticType) == TypeCode.String && !UnderscoredNamingConvention.Instance.Properties.Contains(eventInfo.Source.Value))
                eventInfo.Style = eventInfo.Source.Value == null || eventInfo.Source.Value.ToString().IndexOfAny(multiline) is -1 ? ScalarStyle.SingleQuoted : ScalarStyle.Literal;

            base.Emit(eventInfo, emitter);
        }
    }
}
