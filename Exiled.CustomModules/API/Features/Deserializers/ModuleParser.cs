// -----------------------------------------------------------------------
// <copyright file="ModuleParser.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.Deserializers
{
    // ReSharper disable VirtualMemberCallInConstructor
    using System;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using JetBrains.Annotations;
    using YamlDotNet.Core;

    /// <summary>
    /// An inheritable class that declares an additional deserializer.
    /// </summary>
    public abstract class ModuleParser
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleParser"/> class.
        /// </summary>
        protected ModuleParser()
        {
            ParserContext.Delegates.Add(Delegate);
        }

        /// <summary>
        /// Gets or sets the delegate for parsing.
        /// </summary>
        [NotNull]
        public abstract ParserContext.ModuleDelegate Delegate { get; set; }

        /// <summary>
        /// Registers all module parsers.
        /// </summary>
        public static void InstantiateModuleParsers()
        {
            Log.Info("Registering Custom Module Deserializers:");

            // Get the current assembly
            Assembly assembly = typeof(ModuleParser).Assembly;

            // Get all types that inherit from ModuleParser
            IEnumerable<Type> moduleParserTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ModuleParser)));

            // Instantiate each type with no parameters
            foreach (Type type in moduleParserTypes)
            {
                Log.Info(type.Name);
                Activator.CreateInstance(type);
            }
        }
    }

    /// <summary>
    /// A context for deserializer parsing.
    /// </summary>
#pragma warning disable SA1402
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:Fields should be private", Justification = "Need to be public for deserializer.")]
    public class ParserContext
#pragma warning restore SA1402
    {
        /// <summary>
        /// A list of functions that should be checked when deserializing a module.
        /// </summary>
        public static List<ModuleDelegate> Delegates = new();

        /// <summary>
        /// The Parser.
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