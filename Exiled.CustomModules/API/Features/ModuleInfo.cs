// -----------------------------------------------------------------------
// <copyright file="ModuleInfo.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.CustomModules.API.Enums;

    /// <summary>
    /// A struct containing all information about a module.
    /// </summary>
    public struct ModuleInfo
    {
#pragma warning disable SA1310
        /// <summary>
        /// The name of the method which enables all the module instances.
        /// </summary>
        public const string ENABLE_ALL_CALLBACK = "EnableAll";

        /// <summary>
        /// The name of the method which disables all the module instances.
        /// </summary>
        public const string DISABLE_ALL_CALLBACK = "DisableAll";

        /// <summary>
        /// The binding flags for identifying the registration handlers.
        /// </summary>
        public const BindingFlags SIGNATURE_BINDINGS = BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.IgnoreCase;
#pragma warning restore SA1310

#pragma warning disable SA1202
        /// <summary>
        /// All known modules information.
        /// </summary>
        internal static readonly List<ModuleInfo> AllModules = new();

        /// <summary>
        /// The <see cref="Type"/> of the module.
        /// </summary>
        public Type Type;
#pragma warning restore SA1202

        /// <summary>
        /// A value indicating whether the module is currently loaded.
        /// </summary>
        public bool IsCurrentlyLoaded;

        /// <summary>
        /// The module identifier.
        /// </summary>
        public UUModuleType ModuleType;

#pragma warning disable SA1310
        /// <summary>
        /// Callback method for enabling all instances of the module.
        /// </summary>
        public Action<Assembly> EnableAll_Callback;

        /// <summary>
        /// Callback method for disabling all instances of the module.
        /// </summary>
        public Action DisableAll_Callback;
#pragma warning restore SA1310

        /// <summary>
        /// Gets the module <see cref="Type"/>'s name.
        /// </summary>
        public string Name => Type.Name;

        /// <summary>
        /// Gets a <see cref="ModuleInfo"/> instance based on the module type or name.
        /// </summary>
        /// <param name="type">The module type to look for.</param>
        /// <returns>The corresponding <see cref="ModuleInfo"/>.</returns>
        public static ModuleInfo Get(Type type) => AllModules.FirstOrDefault(m => m.Type == type);

        /// <summary>
        /// Gets a <see cref="ModuleInfo"/> instance based on the module type or name.
        /// </summary>
        /// <param name="name">The module type's name to look for.</param>
        /// <returns>The corresponding <see cref="ModuleInfo"/>.</returns>
        public static ModuleInfo Get(string name) => AllModules.FirstOrDefault(m => m.Name == name);

        /// <summary>
        /// Gets a <see cref="ModuleInfo"/> instance based on the module type or name.
        /// </summary>
        /// <param name="moduleType">The module type's id to look for.</param>
        /// <returns>The corresponding <see cref="ModuleInfo"/>.</returns>
        public static ModuleInfo Get(uint moduleType) => AllModules.FirstOrDefault(m => m.ModuleType == moduleType);

        /// <summary>
        /// Invokes a module callback by name, enabling or disabling module instances based on the specified assembly.
        /// </summary>
        /// <param name="name">The name of the callback to invoke ("EnableAll" or "DisableAll").</param>
        /// <param name="assembly">The assembly to pass to the callback method.</param>
        public void InvokeCallback(string name, Assembly assembly = null)
        {
            bool isEnableAllCallback = string.Equals(name, ENABLE_ALL_CALLBACK, StringComparison.CurrentCultureIgnoreCase);
            if (!isEnableAllCallback && !string.Equals(name, DISABLE_ALL_CALLBACK, StringComparison.CurrentCultureIgnoreCase))
                return;

            IsCurrentlyLoaded = isEnableAllCallback && !IsCurrentlyLoaded;
            if (IsCurrentlyLoaded)
            {
                CustomModule.OnEnabled.InvokeAll(this);
                EnableAll_Callback(assembly ?? Assembly.GetCallingAssembly());
                return;
            }

            CustomModule.OnDisabled.InvokeAll(this);
            DisableAll_Callback();
        }
    }
}
