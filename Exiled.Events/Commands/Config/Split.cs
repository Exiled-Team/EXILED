// -----------------------------------------------------------------------
// <copyright file="Split.cs" company="Exiled Team">
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
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// The config split command.
    /// </summary>
    public class Split : ICommand
    {
        /// <summary>
        /// Gets static instance of the <see cref="Split"/> command.
        /// </summary>
        public static Split Instance { get; } = new Split();

        /// <inheritdoc/>
        public string Command { get; } = "split";

        /// <inheritdoc/>
        public string[] Aliases { get; } = Array.Empty<string>();

        /// <inheritdoc/>
        public string Description { get; } = "Splits your configs into the separated config distribution.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("el.split"))
            {
                response = "You can't use the command, you don't have the \"el.split\" permission.";
                return false;
            }

            if (Loader.Config.ConfigType == ConfigType.Separated)
            {
                response = "Configs are actually separated.";
                return false;
            }

            var configs = ConfigManager.LoadConfigs(ConfigManager.Read());
            Loader.Config.ConfigType = ConfigType.Separated;
            var haveBeenSaved = ConfigManager.SaveAll(configs);
            File.WriteAllText(Paths.LoaderConfig, Loader.Serializer.Serialize(Loader.Config));

            response = $"Configs have been split successfully! Feel free to remove the {Paths.Config} file.";
            return haveBeenSaved;
        }
    }
}
