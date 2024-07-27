// -----------------------------------------------------------------------
// <copyright file="Enable.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Commands.PluginManager
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using API.Interfaces;
    using CommandSystem;
    using Exiled.API.Features;

    /// <summary>
    /// The command to enable a plugin.
    /// </summary>
    public sealed class Enable : ICommand, IPermissioned
    {
        /// <summary>
        /// Gets static instance of the <see cref="Enable"/> command.
        /// </summary>
        public static Enable Instance { get; } = new();

        /// <inheritdoc/>
        public string Command { get; } = "enable";

        /// <inheritdoc/>
        public string[] Aliases { get; } = { "e", "en" };

        /// <inheritdoc/>
        public string Description { get; } = "Enable a plugin";

        /// <inheritdoc />
        public string Permission { get; } = "pm.enable";

        /// <inheritdoc/>
        public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
        {
            if (arguments.Count != 1)
            {
                response = "Please, use: pluginmanager enable <pluginname>";
                return false;
            }

            string assemblyPath = Path.Combine(Paths.Plugins, $"{arguments.At(0)}.dll");
            Assembly assembly = Loader.Loader.LoadAssembly(assemblyPath);
            if (assembly is null)
            {
                response = "Plugin not found.";
                return false;
            }

            if (Loader.Loader.Plugins.Any(pl => pl.Assembly == assembly))
            {
                response = "plugin already enabled";
                return false;
            }

            Loader.Loader.Locations[assembly] = assemblyPath;

            IPlugin<IConfig> plugin = Loader.Loader.CreatePlugin(assembly);
            if (plugin is null)
            {
                response = "Plugin is null, skipping.";
                return false;
            }

            AssemblyInformationalVersionAttribute attribute = plugin.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

            Log.Info($"Loaded plugin {plugin.Name}@{(plugin.Version is not null ? $"{plugin.Version.Major}.{plugin.Version.Minor}.{plugin.Version.Build}" : attribute is not null ? attribute.InformationalVersion : string.Empty)}");

            Server.PluginAssemblies.Add(assembly, plugin);
            if (plugin.Config.Debug)
                Log.DebugEnabled.Add(assembly);
            Loader.Loader.Plugins.Add(plugin);
            plugin.OnEnabled();
            plugin.OnRegisteringCommands();
            response = $"Plugin {plugin.Name} has been enabled!";
            return true;
        }
    }
}
