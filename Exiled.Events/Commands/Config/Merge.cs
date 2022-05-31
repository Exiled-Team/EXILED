// -----------------------------------------------------------------------
// <copyright file="Merge.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Config
{
    using System;
    using System.IO;

    using CommandSystem;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Loader;

    /// <summary>
    /// The config merge command.
    /// </summary>
    public class Merge : ICommand
    {
        /// <summary>
        /// Gets static instance of the <see cref="Merge"/> command.
        /// </summary>
        public static Merge Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "merge";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description { get; } = "Merges your configs into the default config distribution.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (Loader.Config.ConfigType == ConfigType.Default)
            {
                response = "Configs are actually merged.";
                return false;
            }

            var configs = ConfigManager.LoadSorted(ConfigManager.Read());
            Loader.Config.ConfigType = ConfigType.Default;
            var haveBeenSaved = ConfigManager.Save(configs);
            File.WriteAllText(Paths.LoaderConfig, Loader.Serializer.Serialize(Loader.Config));

            response = $"Configs have been merged successfully! Feel free to remove the directory in the following path:\n\"{Paths.IndividualConfigs}\"";
            return haveBeenSaved;
        }
    }
}
