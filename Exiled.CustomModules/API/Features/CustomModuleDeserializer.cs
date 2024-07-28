// -----------------------------------------------------------------------
// <copyright file="CustomModuleDeserializer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;
    using System.Linq;

    using Exiled.CustomModules.API.Features.Deserializers;
    using YamlDotNet.Core;
    using YamlDotNet.Serialization;

    /// <inheritdoc />
    public class CustomModuleDeserializer : INodeDeserializer
    {
        /// <inheritdoc />
        public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            value = null;

            if (ParserContext.Delegates.IsEmpty())
                ModuleParser.InstantiateModuleParsers();

            bool parserStatus = false;
            foreach (ParserContext.ModuleDelegate moduleDelegate in ParserContext.Delegates.TakeWhile(_ => !parserStatus))
                parserStatus = moduleDelegate.Invoke(new ParserContext(parser, expectedType, nestedObjectDeserializer), out value);

            return parserStatus;
        }
    }
}