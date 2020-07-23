// -----------------------------------------------------------------------
// <copyright file="Loader.cs" company="Exiled Team">
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

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;

    /// <summary>
    /// Used to handle plugins.
    /// </summary>
    public static class Loader
    {
        static Loader()
        {
            Log.Info($"Initializing at {Environment.CurrentDirectory}");
            Log.SendRaw($"{Assembly.GetExecutingAssembly().GetName().Name} - Version {Version.Major}.{Version.Minor}.{Version.Build}", ConsoleColor.DarkRed);

            CustomNetworkManager.Modded = true;

            // "Useless" check for now, since configs will be loaded after loading all plugins.
            if (Config.Environment != EnvironmentType.Production)
                Paths.Reload($"EXILED-{Config.Environment.ToString().ToUpper()}");

            if (!Directory.Exists(Paths.Configs))
                Directory.CreateDirectory(Paths.Configs);

            if (!Directory.Exists(Paths.Plugins))
                Directory.CreateDirectory(Paths.Plugins);

            if (!Directory.Exists(Paths.Dependencies))
                Directory.CreateDirectory(Paths.Dependencies);
        }

        /// <summary>
        /// Gets the plugins list.
        /// </summary>
        public static List<IPlugin<IConfig>> Plugins { get; } = new List<IPlugin<IConfig>>();

        /// <summary>
        /// Gets the initialized global random class.
        /// </summary>
        public static Random Random { get; } = new Random();

        /// <summary>
        /// Gets the version of the assembly.
        /// </summary>
        public static Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// Gets the configs of the plugin manager.
        /// </summary>
        public static Config Config { get; } = new Config();

        /// <summary>
        /// Gets a value indicating whether the debug should be shown or not.
        /// </summary>
        public static bool ShouldDebugBeShown => Config.Environment == EnvironmentType.Testing || Config.Environment == EnvironmentType.Development;

        /// <summary>
        /// Gets plugin dependencies.
        /// </summary>
        public static List<Assembly> Dependencies { get; } = new List<Assembly>();

        /// <summary>
        /// Runs the plugin manager, by loading all dependencies, plugins, configs and then enables all plugins.
        /// </summary>
        /// <param name="dependencies">The dependencies that could have been loaded by Exiled.Bootstrap.</param>
        public static void Run(Assembly[] dependencies = null)
        {
            if (dependencies != null && dependencies.Length > 0)
                Dependencies.AddRange(dependencies);

            LoadDependencies();
            LoadPlugins();

            ConfigManager.Reload();

            EnablePlugins();
        }

        /// <summary>
        /// Loads all plugins.
        /// </summary>
        public static void LoadPlugins()
        {
            foreach (string pluginPath in Directory.GetFiles(Paths.Plugins).Where(path => (path.EndsWith(".dll") || path.EndsWith(".exe")) && !IsAssemblyLoaded(path)))
            {
                Assembly assembly = LoadAssembly(pluginPath);

                if (assembly == null)
                    continue;

                IPlugin<IConfig> plugin = CreatePlugin(assembly);

                if (plugin == null)
                    continue;

                Plugins.Add(plugin);
            }

            Plugins.Sort((left, right) => -left.Priority.CompareTo(right.Priority));
        }

        /// <summary>
        /// Loads an assembly.
        /// </summary>
        /// <param name="path">The path to load the assembly from.</param>
        /// <returns>Returns the loaded assembly or null.</returns>
        public static Assembly LoadAssembly(string path)
        {
            try
            {
                return Assembly.LoadFrom(path);
            }
            catch (Exception exception)
            {
                Log.Error($"Error while loading a plugin at {path}! {exception}");
            }

            return null;
        }

        /// <summary>
        /// Create a plugin instance.
        /// </summary>
        /// <param name="assembly">The plugin assembly.</param>
        /// <returns>Returns the created plugin instance or null.</returns>
        public static IPlugin<IConfig> CreatePlugin(Assembly assembly)
        {
            try
            {
                foreach (Type type in assembly.GetTypes().Where(type => !type.IsAbstract && !type.IsInterface))
                {
                    if (!type.BaseType.IsGenericType || type.BaseType.GetGenericTypeDefinition() != typeof(Plugin<>))
                    {
                        Log.Debug($"\"{type.FullName}\" does not inherit from Plugin<TConfig>, skipping.", ShouldDebugBeShown);
                        continue;
                    }

                    Log.Debug($"Loading type {type.FullName}", ShouldDebugBeShown);

                    IPlugin<IConfig> plugin = null;

                    var constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor != null)
                    {
                        Log.Debug("Public default constructor found, creating instance...", ShouldDebugBeShown);

                        plugin = constructor.Invoke(null) as IPlugin<IConfig>;
                    }
                    else
                    {
                        Log.Debug($"Constructor wasn't found, searching for a property with the {type.FullName} type...", ShouldDebugBeShown);

                        var value = Array.Find(type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public), property => property.PropertyType == type)?.GetValue(null);

                        if (value != null)
                            plugin = value as IPlugin<IConfig>;
                    }

                    if (plugin == null)
                    {
                        Log.Error($"{type.FullName} is a valid plugin, but it cannot be instantiated! It either doesn't have a public default constructor without any arguments or a static property of the {type.FullName} type!");

                        continue;
                    }

                    Log.Debug($"Instantiated type {type.FullName}", ShouldDebugBeShown);

                    if (plugin.RequiredExiledVersion > Version)
                    {
                        if (!Config.ShouldLoadOutdatedPlugins)
                        {
                            Log.Error($"You're running an older version of Exiled ({Version.ToString(3)})! This plugin won't be loaded! " +
                            $"Required version to load it: {plugin.RequiredExiledVersion.ToString(3)}");

                            continue;
                        }
                        else
                        {
                            Log.Warn($"You're running an older version of Exiled ({Version.ToString(3)})! " +
                            $"You may encounter some bugs! Update Exiled to at least {plugin.RequiredExiledVersion.ToString(3)}");
                        }
                    }

                    return plugin;
                }
            }
            catch (Exception exception)
            {
                Log.Error($"Error while initializing plugin at {assembly.Location}! {exception}");
            }

            return null;
        }

        /// <summary>
        /// Enables all plugins.
        /// </summary>
        public static void EnablePlugins()
        {
            foreach (IPlugin<IConfig> plugin in Plugins)
            {
                try
                {
                    if (plugin.Config.IsEnabled)
                    {
                        plugin.OnEnabled();
                        plugin.OnRegisteringCommands();
                    }
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
        public static void ReloadPlugins()
        {
            foreach (IPlugin<IConfig> plugin in Plugins)
            {
                try
                {
                    plugin.OnReloaded();

                    plugin.Config.IsEnabled = false;

                    plugin.OnDisabled();
                }
                catch (Exception exception)
                {
                    Log.Error($"Plugin \"{plugin.Name}\" threw an exception while reloading: {exception}");
                }
            }

            LoadPlugins();
            EnablePlugins();
        }

        /// <summary>
        /// Disables all plugins.
        /// </summary>
        public static void DisablePlugins()
        {
            foreach (IPlugin<IConfig> plugin in Plugins)
            {
                try
                {
                    plugin.Config.IsEnabled = false;
                    plugin.OnDisabled();
                    plugin.OnUnregisteringCommands();
                }
                catch (Exception exception)
                {
                    Log.Error($"Plugin \"{plugin.Name}\" threw an exception while disabling: {exception}");
                }
            }
        }

        /// <summary>
        /// Check if a dependency is loaded.
        /// </summary>
        /// <param name="path">The path to check from.</param>
        /// <returns>Returns whether the dependency is loaded or not.</returns>
        public static bool IsDependencyLoaded(string path) => Dependencies.Exists(assembly => assembly.Location == path);

        /// <summary>
        /// Check if an assembly is loaded.
        /// </summary>
        /// <param name="path">The path to check from.</param>
        /// <returns>Returns whether the assembly is loaded or not.</returns>
        public static bool IsAssemblyLoaded(string path) => Plugins.Any(plugin => plugin.Assembly.Location == path);

        /// <summary>
        /// Loads all dependencies.
        /// </summary>
        private static void LoadDependencies()
        {
            try
            {
                Log.Info($"Loading dependencies at {Paths.Dependencies}");

                foreach (string dependency in Directory.GetFiles(Paths.Dependencies).Where(path => path.EndsWith(".dll") && !IsDependencyLoaded(path)))
                {
                    Assembly assembly = LoadAssembly(dependency);

                    if (assembly == null)
                        continue;

                    Dependencies.Add(assembly);

                    Log.Info($"Loaded dependency {assembly.FullName}");
                }

                Log.Info("Dependencies loaded successfully!");
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while loading dependencies! {exception}");
            }
        }
    }
}
