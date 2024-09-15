// -----------------------------------------------------------------------
// <copyright file="ModuleTypeEnabledEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events.EventArgs.Modules
{
    using Exiled.CustomModules.API.Features;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    /// Contains all information after enabling a module type.
    /// </summary>
    public class ModuleTypeEnabledEventArgs : IExiledEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleTypeEnabledEventArgs"/> class.
        /// </summary>
        /// <param name="moduleInfo"><inheritdoc cref="ModuleInfo"/></param>
        public ModuleTypeEnabledEventArgs(ModuleInfo moduleInfo) => ModuleInfo = moduleInfo;

        /// <summary>
        /// Gets the <see cref="API.Features.ModuleInfo"/>.
        /// </summary>
        public ModuleInfo ModuleInfo { get; }
    }
}