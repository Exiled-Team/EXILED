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
    /// <typeparam name="T">The config type.</typeparam>
    public abstract class Plugin<T> : IPlugin<T>
        where T : IConfig, new()
    {
        /// <inheritdoc/>
        public virtual string Name { get; } = Assembly.GetCallingAssembly().GetName().Name;

        /// <inheritdoc/>
        public virtual Version Version { get; } = Assembly.GetCallingAssembly().GetName().Version;

        /// <inheritdoc/>
        public virtual Version RequiredExiledVersion { get; } = new Version(2, 0, 0);

        /// <inheritdoc/>
        public T Config { get; } = new T();

        /// <inheritdoc/>
        public abstract void OnDisabled();

        /// <inheritdoc/>
        public abstract void OnEnabled();

        /// <inheritdoc/>
        public abstract void OnReloaded();
    }
}
