// -----------------------------------------------------------------------
// <copyright file="Get.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.ConfigValue
{
    using System;
    using System.Linq;
    using System.Reflection;

    using CommandSystem;
    using Exiled.API.Extensions;
    using Exiled.API.Interfaces;
    using Exiled.Permissions.Extensions;
    using RemoteAdmin;

    /// <summary>
    /// The command to get config value.
    /// </summary>
    public class Get : ICommand
    {
        /// <summary>
        /// Gets a static instance of <see cref="Get"/> class.
        /// </summary>
        public static Get Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "get";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "print" };

        /// <inheritdoc/>
        public string Description { get; } = "Gets a config value";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            const string perm = "cv.get";

            if (!sender.CheckPermission(perm) && sender is PlayerCommandSender playerSender && !playerSender.FullPermissions)
            {
                response = $"You can't get a config value, you don't have \"{perm}\" permissions.";
                return false;
            }

            if (arguments.Count != 1)
            {
                response = "Please, use: cv get PluginName.ValueName";
                return false;
            }

            string[] args = arguments.At(0).Split('.');

            string pluginName = args[0];
            string propertyName = args[1].FromSnakeCase();

            if (Loader.Loader.Plugins.All(x => x.Name != pluginName))
            {
                response = $"Plugin not found: {pluginName}";
                return false;
            }

            IPlugin<IConfig> plugin = Loader.Loader.Plugins.First(x => x.Name == pluginName);

            if (plugin.Config == null)
            {
                response = "Plugin config is null!";
                return false;
            }

            if (plugin.Config.GetType().GetProperty(propertyName) is not PropertyInfo property)
            {
                response = $"Config value not found: {propertyName} ({propertyName.ToSnakeCase()})";
                return false;
            }

            response = $"Value: {property.GetValue(plugin.Config)}";
            return true;
        }
    }
}