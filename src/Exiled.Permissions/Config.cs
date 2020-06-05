// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions
{
    using System.IO;
    using Exiled.Core.API.Features;
    using Exiled.Core.API.Interfaces;
    using Exiled.Core;

    /// <inheritdoc cref="IConfig"/>
    public sealed class Config : IConfig
    {
        /// <summary>
        /// Gets the plugin folder path.
        /// </summary>
        public static string Folder { get; private set; }

        /// <summary>
        /// Gets the plugin full path.
        /// </summary>
        public static string FullPath { get; private set; }

        /// <inheritdoc/>
        public bool IsEnabled { get; set; }

        /// <inheritdoc/>
        public string Prefix => "exiled_permissions_";

        /// <inheritdoc/>
        public void Reload()
        {
            IsEnabled = PluginManager.YamlConfig.GetBool($"{Prefix}enabled", true);
            Folder = Path.Combine(Paths.Plugins, PluginManager.YamlConfig.GetString($"{Prefix}folder_name", "Exiled Permissions"));
            FullPath = Path.Combine(Folder, PluginManager.YamlConfig.GetString($"{Prefix}file_name", "permissions.yml"));
        }
    }
}
