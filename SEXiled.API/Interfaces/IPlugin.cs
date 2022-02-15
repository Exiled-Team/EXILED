// -----------------------------------------------------------------------
// <copyright file="IPlugin.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Interfaces
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using CommandSystem;
    using SEXiled.API.Enums;
    using SEXiled.API.Features;

    /// <summary>
    /// Defines the contract for basic plugin features.
    /// </summary>
    /// <typeparam name="TConfig">The config type.</typeparam>
    public interface IPlugin<out TConfig> : IComparable<IPlugin<IConfig>>
        where TConfig : IConfig
    {
        /// <summary>
        /// Gets the plugin assembly.
        /// </summary>
        Assembly Assembly { get; }

        /// <summary>
        /// Gets the plugin name.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the plugin prefix.
        /// </summary>
        string Prefix { get; }

        /// <summary>
        /// Gets the plugin author.
        /// </summary>
        string Author { get; }

        /// <summary>
        /// Gets the plugin commands.
        /// </summary>
        Dictionary<Type, Dictionary<Type, ICommand>> Commands { get; }

        /// <summary>
        /// Gets the plugin priority.
        /// Higher values mean higher priority and vice versa.
        /// </summary>
        PluginPriority Priority { get; }

        /// <summary>
        /// Gets the plugin version.
        /// </summary>
        Version Version { get; }

        /// <summary>
        /// Gets the required version of SEXiled to run the plugin without bugs or incompatibilities.
        /// </summary>
        Version RequiredExiledVersion { get; }

        /// <summary>
        /// Gets the plugin config.
        /// </summary>
        TConfig Config { get; }

        /// <summary>
        /// Gets the internally used translations. Plugins should implement <see cref="Plugin{TConfig, TTranslation}"/> and use <see cref="Plugin{TConfig, TTranslation}.Translation"/>.
        /// </summary>
        ITranslation InternalTranslation { get; }

        /// <summary>
        /// Gets the plugin config path.
        /// </summary>
        string ConfigPath { get; }

        /// <summary>
        /// Gets the plugin translation path.
        /// </summary>
        string TranslationPath { get; }

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

        /// <summary>
        /// Fired before registering commands.
        /// </summary>
        void OnRegisteringCommands();

        /// <summary>
        /// Fired before unregistering configs.
        /// </summary>
        void OnUnregisteringCommands();
    }
}
