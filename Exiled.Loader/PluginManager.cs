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
    using Exiled.API.Interfaces;
    using global::Loader;
    using MEC;

    /// <summary>
    /// Used to handle plugins.
    /// </summary>
    public class PluginManager
    {
        private static string typeOverrides = string.Empty;

        /// <summary>
        /// Gets the plugins list.
        /// </summary>
        public static List<Plugin<IConfig>> Plugins => new List<Plugin<IConfig>>();

        /// <summary>
        /// Gets the dependencies list.
        /// </summary>
        public static List<Assembly> Dependencies => new List<Assembly>();

        /// <summary>
        /// Gets the initialized global random class.
        /// </summary>
        public static Random Random => new Random();

        /// <summary>
        /// Gets the version of the assembly.
        /// </summary>
        public static Version Version => Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// Gets the global configs.
        /// </summary>
        public static YamlConfig YamlConfig => new YamlConfig(Paths.Config);

        /// <summary>
        /// Gets the configs of the plugin manager.
        /// </summary>
        public static Config Config => new Config();

        /// <summary>
        /// Gets a value indicating whether the debug should be shown or not.
        /// </summary>
        public static bool ShouldDebugBeShown => Config.Environment == API.Enums.EnvironmentType.Testing || Config.Environment == API.Enums.EnvironmentType.Development;

        /// <summary>
        /// Loads all plugins.
        /// </summary>
        /// <param name="eachTime">The interval between two loaded plugins.</param>
        public static void LoadAll(float eachTime) => Timing.RunCoroutine(LoadAll());

        /// <summary>
        /// The coroutine which loads all plugins.
        /// </summary>
        /// <returns>Used to wait.</returns>
        public static IEnumerator<float> LoadAll()
        {
            yield return Timing.WaitForSeconds(0.5f);

            try
            {
                if (Directory.Exists(Paths.Dependencies))
                    Directory.Move(Paths.Dependencies, Path.Combine(Paths.Plugins, "dependencies"));

                LoadAllDependencies();
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while loading dependencies! {exception}");
            }

            if (Environment.CurrentDirectory.ToLower().Contains("testing"))
                Paths.Plugins = Path.Combine(Paths.AppData, "PluginsTesting");

            if (!Directory.Exists(Paths.Plugins))
            {
                Log.Warn($"Plugin directory not found - creating: {Paths.Plugins}");
                Directory.CreateDirectory(Paths.Plugins);
            }

            List<string> pluginsList = Directory.GetFiles(Paths.Plugins).Where(plugin => !plugin.EndsWith("overrides.txt")).ToList();

            if (File.Exists($"{Paths.Plugins}/overrides.txt"))
                typeOverrides = File.ReadAllText($"{Paths.Plugins}/overrides.txt");

            if (pluginsList.All(plugin => !plugin.Contains("Exiled.Events.dll")))
            {
                Log.Warn("Exiled.Events.dll is not installed! Plugins that do not handle their own events won't work and may cause errors.");
            }
            else
            {
                string eventsPlugin = pluginsList.FirstOrDefault(plugin => plugin.Contains("Exiled.Events.dll"));

                Load(eventsPlugin);
                pluginsList.Remove(eventsPlugin);
            }

            if (pluginsList.Any(plugin => plugin.Contains("Exiled.Permissions.dll")))
            {
                string exiledPermission = pluginsList.FirstOrDefault(m => m.Contains("Exiled.Permissions.dll"));

                Load(exiledPermission);
                pluginsList.Remove(exiledPermission);
            }

            foreach (string plugin in pluginsList)
            {
                if (plugin.EndsWith("EXILED.dll"))
                    continue;

                Load(plugin);
            }

            EnableAll();
        }

        /// <summary>
        /// Loads a single plugin.
        /// </summary>
        /// <param name="path">The path to load the plugin from.</param>
        public static void Load(string path)
        {
            Log.Info($"Loading {path}");
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
                    else if (type.BaseType != typeof(Plugin<IConfig>))
                    {
                        Log.Debug($"{type.FullName} does not inherit from EXILED.Plugin, skipping.", ShouldDebugBeShown);
                        continue;
                    }

                    Log.Info($"Loading type {type.FullName}");
                    object instance = Activator.CreateInstance(type);
                    Log.Info($"Instantiated type {type.FullName}");

                    if (!(instance is Plugin<IConfig> plugin))
                    {
                        Log.Error($"not plugin error! {type.FullName}");
                        continue;
                    }

                    if (plugin.RequiredExiledVersion > Version)
                    {
                        if (Config.ShouldLoadOutdatedPlugins)
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
                    Log.Info($"Successfully loaded {plugin.Name}");
                }
            }
            catch (Exception exception)
            {
                Log.Error($"Error while initalizing {path}! {exception}");
            }
        }

        /// <summary>
        /// Enables all plugins.
        /// </summary>
        public static void EnableAll()
        {
            foreach (Plugin<IConfig> plugin in Plugins)
            {
                try
                {
                    plugin.Config.IsEnabled = true;
                    plugin.Config.Reload();

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
            foreach (Plugin<IConfig> plugin in Plugins)
            {
                try
                {
                    plugin.Config.IsEnabled = false;

                    plugin.OnDisabled();
                    plugin.OnReloaded();

                    Plugins.Remove(plugin);
                }
                catch (Exception exception)
                {
                    Log.Error($"Plugin \"{plugin.Name}\" threw an exception while reloading {exception}");
                }
            }

            LoadAll(0.5f);
        }

        /// <summary>
        /// Disables all plugins.
        /// </summary>
        public static void DisableAll()
        {
            foreach (Plugin<IConfig> plugin in Plugins)
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
        public static void ReloadAllConfigs(bool onlyExiled = false)
        {
            YamlConfig.Reload();

            foreach (Plugin<IConfig> plugin in Plugins)
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
            Log.Debug($"Searching Directory \"{Paths.LoadedDependencies}\"", ShouldDebugBeShown);

            if (!Directory.Exists(Paths.LoadedDependencies))
                Directory.CreateDirectory(Paths.LoadedDependencies);

            string[] dependencies = Directory.GetFiles(Paths.LoadedDependencies);

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