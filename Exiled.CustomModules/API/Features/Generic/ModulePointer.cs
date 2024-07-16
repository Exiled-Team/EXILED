// -----------------------------------------------------------------------
// <copyright file="ModulePointer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.Generic
{
    using System;
    using System.Reflection;

    using Exiled.CustomModules.API.Features;
    using Exiled.CustomModules.API.Features.Attributes;

    /// <summary>
    /// Represents a marker class for a module's pointer.
    /// </summary>
    /// <typeparam name="TModule">The type of the module this configuration belongs to.</typeparam>
    public abstract class ModulePointer<TModule> : ModulePointer
        where TModule : CustomModule
    {
        /// <summary>
        /// Gets the module pointer for the specified custom module and assembly with a target module type.
        /// </summary>
        /// <typeparam name="TTargetModule">The type of the target module.</typeparam>
        /// <param name="customModule">The custom module to get the pointer for.</param>
        /// <param name="assembly">The assembly to search for the module pointer. If null, the calling assembly is used.</param>
        /// <returns>The module pointer for the specified custom module and target module type, or null if not found.</returns>
        public static ModulePointer<TTargetModule> Get<TTargetModule>(CustomModule customModule, Assembly assembly = null)
            where TTargetModule : TModule
        {
            assembly ??= Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                ModuleIdentifierAttribute moduleIdentifier = type.GetCustomAttribute<ModuleIdentifierAttribute>();
                if (moduleIdentifier == null || moduleIdentifier.Id != customModule.Id || !typeof(ModulePointer<TTargetModule>).IsAssignableFrom(type))
                    continue;

                return Activator.CreateInstance(type) as ModulePointer<TTargetModule>;
            }

            return null;
        }

        /// <summary>
        /// Gets the module pointer for the specified custom module from the calling assembly with a target module type.
        /// </summary>
        /// <typeparam name="TTargetModule">The type of the target module.</typeparam>
        /// <param name="customModule">The custom module to get the pointer for.</param>
        /// <returns>The module pointer for the specified custom module and target module type, or null if not found.</returns>
        public static ModulePointer<TTargetModule> Get<TTargetModule>(CustomModule customModule)
            where TTargetModule : TModule => Get<TTargetModule>(customModule, Assembly.GetCallingAssembly());
    }
}
