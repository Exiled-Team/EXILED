// -----------------------------------------------------------------------
// <copyright file="IPlugin.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    using System;

    /// <summary>
    /// Defines the contract for basic plugin features.
    /// </summary>
    /// <typeparam name="T">The config type.</typeparam>
    public interface IPlugin<out T>
        where T : IConfig
    {
        /// <summary>
        /// Gets the plugin name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the plugin version.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Gets the required version of Exiled to run the plugin without bugs or incompatibilities.
        /// </summary>
        Version RequiredExiledVersion { get; }

        /// <summary>
        /// Gets the plugin config.
        /// </summary>
        T Config { get; }

        /// <summary>
        /// Fired after enabling the plugin.
        /// </summary>
        void OnEnabled();

        /// <summary>
        /// Fired after disabling the plugin.
        /// </summary>
        void OnDisabled();

        /// <summary>
        /// Fired after reloading the plugin.
        /// </summary>
        void OnReloaded();
    }
}
