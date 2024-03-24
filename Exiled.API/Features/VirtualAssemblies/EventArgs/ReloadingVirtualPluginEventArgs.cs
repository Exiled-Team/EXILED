// -----------------------------------------------------------------------
// <copyright file="ReloadingVirtualPluginEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.VirtualAssemblies.EventArgs
{
    using System;

    /// <summary>
    /// Contains all information before reloading a <see cref="VirtualPlugin"/>.
    /// </summary>
    public class ReloadingVirtualPluginEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReloadingVirtualPluginEventArgs"/> class.
        /// </summary>
        /// <param name="plugin"><inheritdoc cref="Plugin"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ReloadingVirtualPluginEventArgs(VirtualPlugin plugin, bool isAllowed = true)
        {
            Plugin = plugin;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="VirtualPlugin"/> to be reloaded.
        /// </summary>
        public VirtualPlugin Plugin { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="VirtualPlugin"/> can be reloaded.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}