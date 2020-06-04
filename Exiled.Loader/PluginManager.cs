// -----------------------------------------------------------------------
// <copyright file="PluginManager.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using Exiled.API.Features;
    using global::Loader;

    /// <summary>
    /// Used to handle plugins.
    /// </summary>
    public class PluginManager
    {
        private static string typeOverrides = string.Empty;

        /// <summary>
        /// Gets the plugins list.
        /// </summary>
        public static List<Plugin> Plugins { get; } = new List<Plugin>();

        /// <summary>
        /// Gets the dependencies list.
        /// </summary>
        public static List<Assembly> Dependencies { get; } = new List<Assembly>();

        /// <summary>
        /// Gets the initialized global random class.
        /// </summary>
        public static Random Random { get; } = new Random();

        /// <summary>
        /// Gets the version of the assembly.
        /// </summary>
        public static Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// Gets the global configs.
        /// </summary>
        public static YamlConfig YamlConfig { get; } = new YamlConfig(Paths.Config);

        /// <summary>
        /// Gets the configs of the plugin manager.
        /// </summary>
        public static Config Config { get; } = new Config();

        /// <summary>
        /// Gets a value indicating whether the debug should be shown or not.
        /// </summary>
        public static bool ShouldDebugBeShown => Config.Environment == API.Enums.EnvironmentType.Testing || Config.Environment == API.Enums.EnvironmentType.Development;

        /// <summary>
        /// The coroutine which loads all plugins.
        /// </summary>
        public static void LoadAll()
        {
            try
            {
                LoadAllDependencies();
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while loading dependencies! {exception}");
            }

            if (!Directory.Exists(Paths.Plugins))
            {
                Log.Warn($"Plugin directory not found - creating: {Paths.Plugins}");
                Directory.CreateDirectory(Paths.Plugins);
            }

            List<string> plugins = Directory.GetFiles(Paths.Plugins).Where(plugin => !plugin.EndsWith("overrides.txt")).ToList();

            if (File.Exists($"{Paths.Plugins}/overrides.txt"))
                typeOverrides = File.ReadAllText($"{Paths.Plugins}/overrides.txt");

            if (plugins.All(plugin => !plugin.Contains("Exiled.Events.dll")))
            {
                Log.Warn("Exiled.Events.dll is not installed! Plugins that do not handle their own events won't work and may cause errors.");
            }
            else
            {
                string eventsPlugin = plugins.FirstOrDefault(plugin => plugin.Contains("Exiled.Events.dll"));

                Load(eventsPlugin);
                plugins.Remove(eventsPlugin);
            }

            if (plugins.Any(plugin => plugin.Contains("Exiled.Permissions.dll")))
            {
                string exiledPermission = plugins.FirstOrDefault(m => m.Contains("Exiled.Permissions.dll"));

                Load(exiledPermission);
                plugins.Remove(exiledPermission);
            }

            if (plugins.Any(plugin => plugin.Contains("Exiled.Updater.dll")))
            {
                string exiledUpdater = plugins.FirstOrDefault(m => m.Contains("Exiled.Updater.dll"));

                Load(exiledUpdater);
                plugins.Remove(exiledUpdater);
            }

            foreach (string plugin in plugins)
                Load(plugin);

            EnableAll();
        }

        /// <summary>
        /// Loads a single plugin.
        /// </summary>
        /// <param name="path">The path to load the plugin from.</param>
        public static void Load(string path)
        {
            try
            {
                byte[] file = ModLoader.ReadFile(path);

                Assembly assembly = Assembly.Load(file);

                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsAbstract)
                    {
                        Log.Debug($"{type.FullName} is abstract, skipping.", ShouldDebugBeShown);
                        continue;
                    }

                    if (type.FullName != null && typeOverrides.Contains(type.FullName))
                    {
                        Log.Debug($"Overriding type check for {type.FullName}", ShouldDebugBeShown);
                    }
                    else if (type.BaseType != typeof(Plugin))
                    {
                        Log.Debug($"{type.FullName} does not inherit from EXILED.Plugin, skipping.", ShouldDebugBeShown);
                        continue;
                    }

                    Log.Error($"Checks passed.");

                    Log.Info($"Loading type {type.FullName}");

                    Plugin plugin = (Plugin)Activator.CreateInstance(type);

                    Log.Info($"Instantiated type {type.FullName}");

                    if (plugin.RequiredExiledVersion > Version)
                    {
                        if (!Config.ShouldLoadOutdatedPlugins)
                        {
                            Log.Error($"You're running an older version of Exiled ({Version.Major}.{Version.Minor}.{Version.Build})! This plugin won't be loaded!" +
                            $"Required version to load it: {plugin.RequiredExiledVersion.Major}.{plugin.RequiredExiledVersion.Minor}.{plugin.RequiredExiledVersion.Build}");

                            continue;
                        }
                        else
                        {
                            Log.Warn($"You're running an older version of Exiled ({Version.Major}.{Version.Minor}.{Version.Build})! " +
                            $"You may encounter some bugs! Update Exiled to at least " +
                            $"{plugin.RequiredExiledVersion.Major}.{plugin.RequiredExiledVersion.Minor}.{plugin.RequiredExiledVersion.Build}");
                        }
                    }

                    Plugins.Add(plugin);

                    Log.Info($"Successfully loaded {plugin.Name} v{plugin.Version.Major}.{plugin.Version.Minor}.{plugin.Version.Build}");
                }
            }
            catch (Exception exception)
            {
                Log.Error($"Error while initializing {path}! {exception}");
                Log.Error($"{exception.InnerException}");
                Log.Error($"{exception.StackTrace}");
            }
        }

        /// <summary>
        /// Enables all plugins.
        /// </summary>
        public static void EnableAll()
        {
            foreach (Plugin plugin in Plugins)
            {
                try
                {
                    plugin.Config.IsEnabled = true;
                    plugin.Config.Reload();

                    if (plugin.Config.IsEnabled)
                        plugin.OnEnabled();
                }
                catch (Exception exception)
                {
                    Log.Error($"Plugin \"{plugin.Name}\" threw an exception while enabling: {exception}");
                }
            }
        }

        /// <summary>
        /// Reloads all plugins.
        /// </summary>
        public static void ReloadAll()
        {
            foreach (Plugin plugin in Plugins)
            {
                try
                {
                    plugin.Config.IsEnabled = false;

                    plugin.OnReloaded();
                    plugin.OnDisabled();

                    Plugins.Remove(plugin);
                }
                catch (Exception exception)
                {
                    Log.Error($"Plugin \"{plugin.Name}\" threw an exception while reloading {exception}");
                }
            }

            LoadAll();
        }

        /// <summary>
        /// Disables all plugins.
        /// </summary>
        public static void DisableAll()
        {
            foreach (Plugin plugin in Plugins)
            {
                try
                {
                    plugin.OnDisabled();

                    plugin.Config.IsEnabled = false;
                }
                catch (Exception exception)
                {
                    Log.Error($"Plugin {plugin.Name} threw an exception while disabling {exception}");
                }
            }
        }

        /// <summary>
        /// Reloads all plugin configs.
        /// </summary>
        /// <param name="onlyExiled">Returns whether Exiled configs must be reloaded or not.</param>
        public static void ReloadPluginConfigs(bool onlyExiled = false)
        {
            YamlConfig.Reload();

            foreach (Plugin plugin in Plugins)
            {
                if (onlyExiled && plugin.Name != "EXILED")
                    continue;

                plugin.Config.Reload();
            }
        }

        /// <summary>
        /// Reloads RemoteAdmin configs.
        /// </summary>
        public static void ReloadRemoteAdminConfig()
        {
            ServerStatic.RolesConfig = new YamlConfig(ServerStatic.RolesConfigPath);
            ServerStatic.SharedGroupsConfig = (GameCore.ConfigSharing.Paths[4] == null) ? null : new YamlConfig(GameCore.ConfigSharing.Paths[4] + "shared_groups.txt");
            ServerStatic.SharedGroupsMembersConfig = (GameCore.ConfigSharing.Paths[5] == null) ? null : new YamlConfig(GameCore.ConfigSharing.Paths[5] + "shared_groups_members.txt");
            ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
            ServerStatic.GetPermissionsHandler().RefreshPermissions();
        }

        /// <summary>
        /// Check if a dependency is loaded.
        /// </summary>
        /// <param name="path">The path to check from.</param>
        /// <returns>Returns whether the dependency is loaded or not.</returns>
        public static bool IsDependencyLoaded(string path)
        {
            foreach (Assembly assembly in Dependencies)
            {
                if (assembly.Location == path)
                    return true;
            }

            return false;
        }

        /// <summary>
        /// Loads all dependencies.
        /// </summary>
        private static void LoadAllDependencies()
        {
            Log.Info("Loading dependencies...");
            Log.Debug($"Searching Directory \"{Paths.Dependencies}\"", ShouldDebugBeShown);

            if (!Directory.Exists(Paths.Dependencies))
                Directory.CreateDirectory(Paths.Dependencies);

            string[] dependencies = Directory.GetFiles(Paths.Dependencies);

            foreach (string dll in dependencies)
            {
                if (!dll.EndsWith(".dll"))
                    continue;

                if (IsDependencyLoaded(dll))
                    return;

                Assembly assembly = Assembly.LoadFrom(dll);
                Dependencies.Add(assembly);
                Log.Info("Loaded dependency " + assembly.FullName);
            }

            Log.Debug("Complete!", ShouldDebugBeShown);
        }
    }
}