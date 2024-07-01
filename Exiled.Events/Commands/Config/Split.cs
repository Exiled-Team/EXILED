// -----------------------------------------------------------------------
// <copyright file="Split.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Config
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using API.Enums;
    using API.Features;
    using API.Interfaces;
    using CommandSystem;
    using Loader;

    /// <summary>
    /// The config split command.
    /// </summary>
    public class Split : ICommand
    {
        /// <summary>
        /// Gets static instance of the <see cref="Split"/> command.
        /// </summary>
        public static Split Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "split";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description { get; } = "Splits your configs into the separated config distribution.";

        /// <inheritdoc />
        public bool SanitizeResponse { get; }

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (LoaderPlugin.Config.ConfigType == ConfigType.Separated)
            {
                response = "Configs are already separated.";
                return false;
            }

            SortedDictionary<string, IConfig> configs = ConfigManager.LoadSorted(ConfigManager.Read());
            LoaderPlugin.Config.ConfigType = ConfigType.Separated;
            bool haveBeenSaved = ConfigManager.Save(configs);
            PluginAPI.Loader.AssemblyLoader.InstalledPlugins.FirstOrDefault(x => x.PluginName == "Exiled Loader")?.SaveConfig(new LoaderPlugin(), nameof(LoaderPlugin.Config));

            response = $"Configs have been merged successfully! Feel free to remove the file in the following path:\n\"{Paths.Config}\"";
            return haveBeenSaved;
        }
    }
}