// -----------------------------------------------------------------------
// <copyright file="Disable.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.PluginManager
{
    using System;

    using API.Interfaces;
    using CommandSystem;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;

    /// <summary>
    /// The command to disable a plugin.
    /// </summary>
    public sealed class Disable : ICommand, IPermissioned
    {
        /// <summary>
        /// Gets static instance of the <see cref="Disable"/> command.
        /// </summary>
        public static Disable Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "disable";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "ds", "dis" };

        /// <inheritdoc/>
        public string Description { get; } = "Disable a plugin.";

        /// <inheritdoc cref="SanitizeResponse" />
        public bool SanitizeResponse { get; }

        /// <inheritdoc />
        public string Permission { get; } = "pm.disable";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 1)
            {
                response = "Please, use: pluginmanager disable <pluginname>";
                return false;
            }

            IPlugin<IConfig> plugin = Loader.Loader.GetPlugin(arguments.At(0));
            if (plugin is null)
            {
                response = "Plugin not enabled or not found.";
                return false;
            }

            plugin.OnUnregisteringCommands();
            plugin.OnDisabled();
            Loader.Loader.Plugins.Remove(plugin);
            Loader.Loader.Locations.Remove(plugin.Assembly);
            response = $"Plugin {plugin.Name} has been disabled!";
            return true;
        }
    }
}