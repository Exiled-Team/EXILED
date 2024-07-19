// -----------------------------------------------------------------------
// <copyright file="Configs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Reload
{
    using System;

    using API.Interfaces;

    using CommandSystem;

    using Exiled.Permissions.Extensions;

    using Loader;

    /// <summary>
    /// The reload configs command.
    /// </summary>
    public class Configs : ICommand, IPermissioned
    {
        /// <summary>
        /// Gets static instance of the <see cref="Configs"/> command.
        /// </summary>
        public static Configs Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "configs";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "cfgs" };

        /// <inheritdoc/>
        public string Description { get; } = "Reload plugin configs.";

        /// <inheritdoc cref="SanitizeResponse" />
        public bool SanitizeResponse { get; }

        /// <inheritdoc />
        public string Permission { get; } = "ee.reloadconfigs";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            bool haveBeenReloaded = ConfigManager.Reload();

            Handlers.Server.OnReloadedConfigs();
            API.Features.Log.DebugEnabled.Clear();

            foreach (IPlugin<IConfig> plugin in Loader.Plugins)
            {
                plugin.OnUnregisteringCommands();
                plugin.OnRegisteringCommands();
                if (plugin.Config.Debug)
                    API.Features.Log.DebugEnabled.Add(plugin.Assembly);
            }

            response = "Plugin configs have been reloaded successfully!";
            return haveBeenReloaded;
        }
    }
}
