// -----------------------------------------------------------------------
// <copyright file="Configs.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Commands.Reload
{
    using System;

    using CommandSystem;

    using SEXiled.API.Interfaces;
    using SEXiled.Loader;
    using SEXiled.Permissions.Extensions;

    /// <summary>
    /// The reload configs command.
    /// </summary>
    public class Configs : ICommand
    {
        /// <summary>
        /// Gets static instance of the <see cref="Configs"/> command.
        /// </summary>
        public static Configs Instance { get; } = new Configs();

        /// <inheritdoc/>
        public string Command { get; } = "configs";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new string[] { "cfgs" };

        /// <inheritdoc/>
        public string Description { get; } = "Reload plugin configs.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ee.reloadconfigs"))
            {
                response = "You can't reload configs, you don't have \"ee.reloadconfigs\" permission.";
                return false;
            }

            bool haveBeenReloaded = ConfigManager.Reload();

            Handlers.Server.OnReloadedConfigs();

            foreach (IPlugin<IConfig> plugin in Loader.Plugins)
            {
                plugin.OnUnregisteringCommands();
                plugin.OnRegisteringCommands();
            }

            response = "Plugin configs have been reloaded successfully!";
            return haveBeenReloaded;
        }
    }
}
