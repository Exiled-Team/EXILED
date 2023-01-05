// -----------------------------------------------------------------------
// <copyright file="PathExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader
{
    using System;
    using System.Reflection;

    using API.Interfaces;

    /// <summary>
    /// Contains the extensions to get a path.
    /// </summary>
    public static class PathExtensions
    {
        /// <summary>
        /// Gets a path of an assembly.
        /// </summary>
        /// <param name="assembly">The <see cref="Assembly"/>.</param>
        /// <exception cref="ArgumentNullException">The provided assembly is <see langword="null"/>.</exception>
        /// <returns>The path of the assembly or <see langword="null"/>.</returns>
        public static string GetPath(this Assembly assembly)
        {
            if (assembly is null)
                throw new ArgumentNullException(nameof(assembly));

            Loader.Locations.TryGetValue(assembly, out string path);
            return path;
        }

        /// <summary>
        /// Gets a path of a plugin.
        /// </summary>
        /// <param name="plugin">The <see cref="IPlugin{IConfig}"/>.</param>
        /// <exception cref="ArgumentNullException">The provided plugin is <see langword="null"/>.</exception>
        /// <returns>The path of the plugin or <see langword="null"/>.</returns>
        public static string GetPath(this IPlugin<IConfig> plugin)
        {
            if (plugin is null)
                throw new ArgumentNullException(nameof(plugin));

            return plugin.Assembly.GetPath();
        }
    }
}