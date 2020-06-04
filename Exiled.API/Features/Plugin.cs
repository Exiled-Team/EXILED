// -----------------------------------------------------------------------
// <copyright file="Plugin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Reflection;
    using Exiled.API.Interfaces;

    /// <summary>
    /// Expose how a plugin has to be made.
    /// </summary>
    public abstract class Plugin
    {
        /// <summary>
        /// Gets the plugin name.
        /// </summary>
        public virtual string Name => Assembly.GetExecutingAssembly().GetName().Name;

        /// <summary>
        /// Gets the plugin version.
        /// </summary>
        public virtual Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// Gets the required version of EXILED to run the plugin without bugs or incompatibilities.
        /// </summary>
        public virtual Version RequiredExiledVersion => new Version(2, 0, 0);

        /// <summary>
        /// Gets the plugin <see cref="IConfig"/>.
        /// </summary>
        public abstract IConfig Config { get; }

        /// <summary>
        /// Fired after enabling the plugin.
        /// </summary>
        public abstract void OnEnabled();

        /// <summary>
        /// Fired after disabling the plugin.
        /// </summary>
        public abstract void OnDisabled();

        /// <summary>
        /// Fired after reloading the plugin.
        /// </summary>
        public abstract void OnReloaded();
    }
}