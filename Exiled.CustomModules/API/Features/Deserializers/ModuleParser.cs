// -----------------------------------------------------------------------
// <copyright file="ModuleParser.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.Deserializers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using JetBrains.Annotations;

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
            Log.Debug("Registering Custom Module Deserializers:");

            // Get the current assembly
            Assembly assembly = typeof(ModuleParser).Assembly;

            // Get all types that inherit from ModuleParser
            IEnumerable<Type> moduleParserTypes = assembly.GetTypes()
                .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(ModuleParser)));

            // Instantiate each type with no parameters
            foreach (Type type in moduleParserTypes)
            {
                Log.Debug(type.Name);
                Activator.CreateInstance(type);
            }
        }
    }
}