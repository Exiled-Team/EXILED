// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules
{
    using System.ComponentModel;

    using Exiled.API.Interfaces;

    /// <summary>
    /// The plugin's config.
    /// </summary>
    public class Config : IConfig
    {
        /// <inheritdoc/>
        [Description("Whether or not the plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug messages should be printed to the console.
        /// </summary>
        /// <returns><see cref="bool"/>.</returns>
        [Description("Whether or not debug messages should be shown.")]
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the built-in role assigner should be used over the base game one.
        /// </summary>
        [Description("Whether the built-in role assigner should be used over the base game one.")]
        public bool UseDefaultRoleAssigner { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the built-in respawn manager should be used over the base game one.
        /// </summary>
        [Description("Whether the built-in respawn manager should be used over the base game one.")]
        public bool UseDefaultRespawnManager { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether the automatic modules loader should be used.
        /// <para/>
        /// It iterates over all existing plugins trying to enable all the modules for each plugin's assembly.
        /// <br/>
        /// It negatively affects the performance in case of the presence of a big amount of plugins.
        /// </summary>
        [Description("Whether the automatic modules loader should be used.")]
        public bool UseAutomaticModulesLoader { get; set; }

        /// <summary>
        /// Gets or sets all modules to be loaded.
        /// </summary>
        [Description("The modules to be loaded.")]
        public string[] Modules { get; set; } = { };
    }
}