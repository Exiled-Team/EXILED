// -----------------------------------------------------------------------
// <copyright file="ParserContext.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.Deserializers
{
    using System;
    using System.Collections.Generic;

    using YamlDotNet.Core;

#pragma warning disable SA1401

    /// <summary>
    /// A context for deserializer parsing.
    /// </summary>
    public class ParserContext
    {
        /// <summary>
        /// A list of functions that should be checked when deserializing a module.
        /// </summary>
        public static readonly List<ModuleDelegate> Delegates = new();

        /// <summary>
        /// The parser.
        /// </summary>
        public readonly IParser Parser;

        /// <summary>
        /// The expected type.
        /// </summary>
        public readonly Type ExpectedType;

        /// <summary>
        /// The fallback deserializer.
        /// </summary>
        public readonly Func<IParser, Type, object> NestedObjectDeserializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ParserContext"/> class.
        /// </summary>
        /// <param name="parser">The Parser.</param>
        /// <param name="expectedType">The type expected.</param>
        /// <param name="nestedObjectDeserializer">The fallback deserializer.</param>
        public ParserContext(
            IParser parser,
            Type expectedType,
            Func<IParser, Type, object> nestedObjectDeserializer)
        {
            Parser = parser;
            ExpectedType = expectedType;
            NestedObjectDeserializer = nestedObjectDeserializer;
        }

        /// <summary>
        /// A delegate returning bool retaining to a module.
        /// </summary>
        /// <param name="input">The parser context.</param>
        /// <param name="output">The output object if successful.</param>
        /// <returns>A bool stating if parsing was successful or not.</returns>
        public delegate bool ModuleDelegate(in ParserContext input, out object output);
    }
}