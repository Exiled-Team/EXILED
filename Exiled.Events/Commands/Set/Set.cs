// -----------------------------------------------------------------------
// <copyright file="Set.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.Set
{
    using System;

    using CommandSystem;

    using Exiled.API.Interfaces;
    using Exiled.Loader;
    using Exiled.Permissions.Extensions;

    /// <summary>
    /// Enable or disable a plugin.
    /// </summary>
    public class Set : ICommand
    {
        /// <inheritdoc/>
        public string Command { get; } = "pluginset";

        /// <inheritdoc/>
        public string[] Aliases { get; } = new[] { "pset" };

        /// <inheritdoc/>
        public string Description { get; } = "Disable or enable plugin.";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (!sender.CheckPermission("ee.setpluginstatus"))
            {
                response = "You can't disable or enable a plugin, you don't have \"ee.setpluginstatus\" permission.";
                return false;
            }

            if (arguments.Count != 2 || (arguments.At(0) != "on" && arguments.At(0) != "off"))
            {
                response = "Please, use: pluginset on/off pluginName";
                return false;
            }

            IPlugin<IConfig> plugin = Loader.GetPlugin(arguments.At(1));
            if (plugin is null)
            {
                response = "Plugin not found.";
                return false;
            }

            try
            {
                if (arguments.At(0) == "on")
                {
                    plugin.OnEnabled();
                    plugin.OnRegisteringCommands();
                    response = "Plugin loaded correctly";
                    return true;
                }

                plugin.OnUnregisteringCommands();
                plugin.OnDisabled();
                response = "Plugin disable correctly";
                return true;
            }
            catch (Exception e)
            {
                response = $"Error {(arguments.At(0) == "on" ? "enabling" : "disabling")} the plugin: {e}";
                return false;
            }
        }
    }
}
