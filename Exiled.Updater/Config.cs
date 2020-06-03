// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Updater
{
    using Exiled.API.Interfaces;
    using Exiled.Loader;

    /// <inheritdoc cref="IConfig"/>
    public sealed class Config : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; }

        /// <inheritdoc/>
        public string Prefix => "exiled_updater_";

        /// <summary>
        /// Gets a value indicating whether testing releases have to be downloaded or not.
        /// </summary>
        public bool AllowTestingReleases { get; internal set; }

        /// <inheritdoc/>
        public void Reload()
        {
            IsEnabled = PluginManager.YamlConfig.GetBool($"{Prefix}enabled");
            AllowTestingReleases = PluginManager.YamlConfig.GetBool($"{Prefix}isAllowed_testing_releases");
        }
    }
}
