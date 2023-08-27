// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader
{
    using System.ComponentModel;
    using System.IO;

    using API.Enums;
    using API.Interfaces;
    using Exiled.API.Features;
    using YamlDotNet.Core;

    /// <summary>
    /// The configs of the loader.
    /// </summary>
    public sealed class Config : IConfig
    {
        /// <inheritdoc />
        [Description("Whether or not EXILED is enabled on this server.")]
        public bool IsEnabled { get; set; } = true;

        /// <inheritdoc />
        [Description("Whether or not debug messages should be shown.")]
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether outdated exiled should be loaded or not.
        /// </summary>
        [Description("Indicates whether outdated Exiled should be loaded or not. It could cause issues when it's true")]
        public bool ShouldLoadOutdatedExiled { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether outdated plugins should be loaded or not.
        /// </summary>
        [Description("Indicates whether outdated plugins should be loaded or not")]
        public bool ShouldLoadOutdatedPlugins { get; set; } = true;

        /// <summary>
        /// Gets or sets the Exiled directory path from which plugins will be loaded.
        /// </summary>
        [Description("The Exiled directory path from which plugins will be loaded")]
        public string ExiledDirectoryPath { get; set; } = Path.Combine(Paths.AppData, "EXILED");

        /// <summary>
        /// Gets or sets the environment type.
        /// </summary>
        [Description("The working environment type (Development, Testing, Production, Ptb, ProductionDebug)")]
        public EnvironmentType Environment { get; set; } = EnvironmentType.Production;

        /// <summary>
        /// Gets or sets the config files distribution type.
        /// </summary>
        [Description("The config files distribution type (Default, Separated)")]
        public ConfigType ConfigType { get; set; } = ConfigType.Default;

        /// <summary>
        /// Gets or sets the quotes wrapper type.
        /// </summary>
        [Description("Indicates in which quoted strings in configs will be wrapped (Any, SingleQuoted, DoubleQuoted, Folded, Literal)")]
        public ScalarStyle ScalarStyle { get; set; } = ScalarStyle.SingleQuoted;

        /// <summary>
        /// Gets or sets the quotes wrapper type.
        /// </summary>
        [Description("Indicates in which quoted strings with multiline in configs will be wrapped (Any, SingleQuoted, DoubleQuoted, Folded, Literal)")]
        public ScalarStyle MultiLineScalarStyle { get; set; } = ScalarStyle.Literal;
    }
}