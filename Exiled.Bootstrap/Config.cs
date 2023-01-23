// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Bootstrap
{
    using System.ComponentModel;
    using System.IO;

    using PluginAPI.Helpers;

    /// <summary>
    /// The configs of the loader.
    /// </summary>
    public sealed class Config
    {
        /// <summary>
        /// Gets or sets a value indicating whether the plugin is enabled or not.
        /// </summary>
        [Description("Whether or not EXILED is enabled on this server.")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets the Exiled directory path from which plugins will be loaded.
        /// </summary>
        [Description("The Exiled directory path from which plugins will be loaded")]
        public string ExiledDirectoryPath { get; set; } = Path.Combine(Paths.AppData, "EXILED");
    }
}