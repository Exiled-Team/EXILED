// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example
{
    using Exiled.API.Interfaces;
    using Exiled.Loader;

    /// <inheritdoc cref="IConfig"/>
    public class Config : IConfig
    {
        /// <inheritdoc/>
        public bool IsEnabled { get; set; }

        /// <summary>
        /// Gets the test config.
        /// </summary>
        public string Test { get; private set; }

        /// <inheritdoc/>
        public string Prefix => "exiled_example_";

        /// <inheritdoc/>
        public void Reload()
        {
            IsEnabled = PluginManager.YamlConfig.GetBool($"{Prefix}enabled", true);
            Test = PluginManager.YamlConfig.GetString($"{Prefix}test", "This is an example of a string config!");
        }
    }
}
