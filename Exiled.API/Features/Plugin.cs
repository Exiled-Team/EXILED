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

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Interfaces;

    /// <summary>
    /// Expose how a plugin has to be made.
    /// </summary>
    /// <typeparam name="TConfig">The config type.</typeparam>
    public abstract class Plugin<TConfig> : IPlugin<TConfig>
        where TConfig : IConfig, new()
    {
        /// <inheritdoc/>
        public virtual string Name { get; } = Assembly.GetCallingAssembly().GetName().Name;

        /// <inheritdoc/>
        public virtual string Prefix { get; } = Assembly.GetCallingAssembly().GetName().Name.ToSnakeCase();

        /// <inheritdoc/>
        public virtual PluginPriority Priority { get; }

        /// <inheritdoc/>
        public virtual Version Version { get; } = Assembly.GetCallingAssembly().GetName().Version;

        /// <inheritdoc/>
        public virtual Version RequiredExiledVersion { get; } = new Version(2, 0, 0);

        /// <inheritdoc/>
        public TConfig Config { get; } = new TConfig();

        /// <inheritdoc/>
        public abstract void OnDisabled();

        /// <inheritdoc/>
        public abstract void OnEnabled();

        /// <inheritdoc/>
        public abstract void OnReloaded();
    }
}
