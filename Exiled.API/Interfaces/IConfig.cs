// -----------------------------------------------------------------------
// <copyright file="IConfig.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    using System.ComponentModel;

    /// <summary>
    /// Defines the contract for basic config features.
    /// </summary>
    public interface IConfig
    {
        /// <summary>
        /// Gets or sets a value indicating whether the plugin is enabled or not.
        /// </summary>
        [Description("Whether or not this plugin is enabled.")]
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether debug messages should be displayed in the console or not.
        /// </summary>
        [Description("Whether or not debug messages should be shown in the console.")]
        bool Debug { get; set; }
    }
}