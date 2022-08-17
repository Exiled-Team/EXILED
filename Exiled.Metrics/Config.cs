// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Metrics;

using System.ComponentModel;

using Exiled.API.Interfaces;

/// <inheritdoc />
public class Config : IConfig
{
    /// <inheritdoc/>
    [Description("Whether or not this plugin is enabled.")]
    public bool IsEnabled { get; set; } = false;

    /// <summary>
    /// Gets or sets a value indicating whether debug messages should appear.
    /// </summary>
    [Description("Whether or not debug logs should be shown in the console.")]
    public bool Debug { get; set; }
}
