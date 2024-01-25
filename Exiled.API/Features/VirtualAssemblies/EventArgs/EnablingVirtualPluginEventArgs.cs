// -----------------------------------------------------------------------
// <copyright file="EnablingVirtualPluginEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.VirtualAssemblies.EventArgs
{
    using System;

    /// <summary>
    /// Contains all information before enabling a <see cref="VirtualPlugin"/>.
    /// </summary>
    public class EnablingVirtualPluginEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EnablingVirtualPluginEventArgs"/> class.
        /// </summary>
        /// <param name="plugin"><inheritdoc cref="Plugin"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public EnablingVirtualPluginEventArgs(VirtualPlugin plugin, bool isAllowed = true)
        {
            Plugin = plugin;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the <see cref="VirtualPlugin"/> to be enabled.
        /// </summary>
        public VirtualPlugin Plugin { get; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="VirtualPlugin"/> can be enabled.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}