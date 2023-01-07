// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Permissions
{
    using System.ComponentModel;
    using System.IO;

    using API.Interfaces;

    using Exiled.API.Features;

    /// <inheritdoc cref="IConfig"/>
    public sealed class Config : IConfig
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Config"/> class.
        /// </summary>
        public Config()
        {
            Folder = Paths.Configs;
            FullPath = Path.Combine(Folder, "permissions.yml");
        }

        /// <summary>
        /// Gets a value indicating whether the debug should be shown or not.
        /// </summary>
        [Description("Indicates whether the debug should be shown or not")]
        public bool ShouldDebugBeShown { get; private set; }

        /// <summary>
        /// Gets the permissions folder path.
        /// </summary>
        [Description("The permissions folder path")]
        public string Folder { get; private set; }

        /// <summary>
        /// Gets the permissions full path.
        /// </summary>
        [Description("The permissions full path")]
        public string FullPath { get; private set; }

        /// <inheritdoc/>
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc/>
        public bool Debug { get; set; }
    }
}