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
        /// Gets or sets the module type which the module pointer is pointing to.
        /// </summary>
        public virtual string ModuleTypeIndicator { get; set; }

        /// <summary>
        /// Gets the module pointer for the specified custom module and assembly.
        /// </summary>
        /// <param name="customModule">The custom module to get the pointer for.</param>
        /// <param name="assembly">The assembly to search for the module pointer. If null, the calling assembly is used.</param>
        /// <returns>The module pointer for the specified custom module, or null if not found.</returns>
        public static ModulePointer Get(CustomModule customModule, Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            Type customModuleType = customModule.GetType().BaseType;

            Type baseModuleType = customModuleType.IsGenericType ? customModuleType.BaseType : customModuleType;
            if (baseModuleType == typeof(CustomModule))
                baseModuleType = customModuleType;

            foreach (Type type in assembly.GetTypes())
            {
                ModuleIdentifierAttribute moduleIdentifier = type.GetCustomAttribute<ModuleIdentifierAttribute>();
                if (moduleIdentifier == null)
                    continue;

                if (typeof(ModulePointer).IsAssignableFrom(type))
                {
                    ModulePointer modulePointer;
                    Type constructedType = null;

                    if (type.BaseType.IsGenericType)
                        constructedType = type.BaseType.GetGenericArguments()[0];

                    if (constructedType != baseModuleType)
                        continue;

                    modulePointer = Activator.CreateInstance(type) as ModulePointer;

                    if (modulePointer.Id != customModule.Id)
                        continue;

                    if (string.IsNullOrEmpty(modulePointer.ModuleTypeIndicator))
                        modulePointer.ModuleTypeIndicator = constructedType.Name;

                    return modulePointer;
                }
            }

            return null;
        }
    }
}
