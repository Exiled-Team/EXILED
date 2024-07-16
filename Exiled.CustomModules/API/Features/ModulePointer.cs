// -----------------------------------------------------------------------
// <copyright file="ModulePointer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;
    using System.Reflection;

    using Exiled.API.Features.Core;
    using Exiled.CustomModules.API.Features.Attributes;

    /// <summary>
    /// Represents a marker class for a module's pointer.
    /// </summary>
    public abstract class ModulePointer : TypeCastObject<ModulePointer>
    {
        /// <summary>
        /// Gets or sets the module's id the <see cref="ModulePointer"/> is pointing to.
        /// </summary>
        public abstract uint Id { get; set; }

        /// <summary>
        /// Gets the module pointer for the specified custom module and assembly.
        /// </summary>
        /// <param name="customModule">The custom module to get the pointer for.</param>
        /// <param name="assembly">The assembly to search for the module pointer. If null, the calling assembly is used.</param>
        /// <returns>The module pointer for the specified custom module, or null if not found.</returns>
        public static ModulePointer Get(CustomModule customModule, Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();
            Type customModuleType = customModule.GetType();

            foreach (Type type in assembly.GetTypes())
            {
                ModuleIdentifierAttribute moduleIdentifier = type.GetCustomAttribute<ModuleIdentifierAttribute>();
                if (moduleIdentifier == null)
                    continue;

                bool isPointing = moduleIdentifier.Id > 0 && moduleIdentifier.Id == customModule.Id;
                if (typeof(ModulePointer).IsAssignableFrom(type))
                {
                    ModulePointer modulePointer;
                    if (type.IsGenericTypeDefinition)
                    {
                        Type constructedType = type.MakeGenericType(customModuleType);
                        modulePointer = Activator.CreateInstance(constructedType) as ModulePointer;
                    }
                    else
                    {
                        modulePointer = Activator.CreateInstance(type) as ModulePointer;
                    }

                    if (isPointing)
                    {
                        modulePointer.Id = moduleIdentifier.Id;
                        return modulePointer;
                    }

                    if (modulePointer.Id != customModule.Id)
                        continue;

                    return modulePointer;
                }
            }

            return null;
        }

        /// <summary>
        /// Gets the module pointer for the specified custom module from the calling assembly.
        /// </summary>
        /// <param name="customModule">The custom module to get the pointer for.</param>
        /// <returns>The module pointer for the specified custom module, or null if not found.</returns>
        public static ModulePointer Get(CustomModule customModule) => Get(customModule, Assembly.GetCallingAssembly());
    }
}
