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
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using CommandSystem.Commands;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Loader.Features;
    using Exiled.Loader.Features.Configs;
    using Exiled.Loader.Features.Configs.CustomConverters;

    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using YamlDotNet.Serialization.NodeDeserializers;

    /// <summary>
    /// Used to handle plugins.
    /// </summary>
    public static class Loader
    {
        static Loader()
        {
            Log.Info($"Initializing at {Environment.CurrentDirectory}");

#if PUBLIC_BETA
            Log.Warn("You are running a public beta build. It is not compatible with another version of the game.");
#endif

            Log.SendRaw($"{Assembly.GetExecutingAssembly().GetName().Name} - Version {Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyInformationalVersionAttribute>().InformationalVersion}", ConsoleColor.DarkRed);

            if (MultiAdminFeatures.MultiAdminUsed)
            {
                Log.SendRaw($"Detected MultiAdmin! Version: {MultiAdminFeatures.MultiAdminVersion} | Features: {MultiAdminFeatures.MultiAdminModFeatures}", ConsoleColor.Cyan);
                MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.SERVER_START);
                MultiAdminFeatures.CallAction(MultiAdminFeatures.ActionType.SET_SUPPORTED_FEATURES, MultiAdminFeatures.ModFeatures.All);
            }

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
        public static SortedSet<IPlugin<IConfig>> Plugins { get; } = new SortedSet<IPlugin<IConfig>>(PluginPriorityComparer.Instance);

        /// <summary>
        /// Gets a dictionary containing the file paths of assemblies.
        /// </summary>
        public static Dictionary<Assembly, string> Locations { get; } = new Dictionary<Assembly, string>();

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
        /// Gets the serializer for configs and translations.
        /// </summary>
        public static ISerializer Serializer { get; } = new SerializerBuilder()
            .WithTypeConverter(new VectorsConverter())
            .WithTypeInspector(inner => new CommentGatheringTypeInspector(inner))
            .WithEmissionPhaseObjectGraphVisitor(args => new CommentsObjectGraphVisitor(args.InnerVisitor))
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .Build();

        /// <summary>
        /// Gets the deserializer for configs and translations.
        /// </summary>
        public static IDeserializer Deserializer { get; } = new DeserializerBuilder()
            .WithTypeConverter(new VectorsConverter())
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), deserializer => deserializer.InsteadOf<ObjectNodeDeserializer>())
            .IgnoreFields()
            .IgnoreUnmatchedProperties()
            .Build();

        /// <summary>
        /// Runs the plugin manager, by loading all dependencies, plugins, configs and then enables all plugins.
        /// </summary>
        /// <param name="dependencies">The dependencies that could have been loaded by Exiled.Bootstrap.</param>
        public static void Run(Assembly[] dependencies = null)
        {
            if (dependencies?.Length > 0)
                Dependencies.AddRange(dependencies);

            LoadDependencies();
            LoadPlugins();

            ConfigManager.Reload();
            TranslationManager.Reload();

            EnablePlugins();

            BuildInfoCommand.ModDescription = string.Join(
                "\n",
                AppDomain.CurrentDomain.GetAssemblies()
                    .Where(a => a.FullName.StartsWith("Exiled.", StringComparison.OrdinalIgnoreCase))
                    .Select(a => $"{a.GetName().Name} - Version {a.GetName().Version.ToString(3)}"));
            ServerConsole.AddLog(
                @"Welcome to
   ▄████████ ▀████    ▐████▀  ▄█   ▄█          ▄████████ ████████▄
  ███    ███   ███▌   ████▀  ███  ███         ███    ███ ███   ▀███
  ███    █▀     ███  ▐███    ███▌ ███         ███    █▀  ███    ███
 ▄███▄▄▄        ▀███▄███▀    ███▌ ███        ▄███▄▄▄     ███    ███
▀▀███▀▀▀        ████▀██▄     ███▌ ███       ▀▀███▀▀▀     ███    ███
  ███    █▄    ▐███  ▀███    ███  ███         ███    █▄  ███    ███
  ███    ███  ▄███     ███▄  ███  ███▌    ▄   ███    ███ ███   ▄███
  ██████████ ████       ███▄ █▀   █████▄▄██   ██████████ ████████▀
                                  ▀                                 ", ConsoleColor.Green);
        }

        /// <summary>
        /// Loads all plugins.
        /// </summary>
        public static void LoadPlugins()
        {
            foreach (string assemblyPath in Directory.GetFiles(Paths.Plugins, "*.dll"))
            {
                Assembly assembly = LoadAssembly(assemblyPath);

                if (assembly == null)
                    continue;

                Locations[assembly] = assemblyPath;

                Log.Info($"Loaded plugin {assembly.GetName().Name}@{assembly.GetName().Version.ToString(3)}");
            }

            foreach (Assembly assembly in Locations.Keys)
            {
                if (Locations[assembly].Contains("dependencies"))
                    continue;

                IPlugin<IConfig> plugin = CreatePlugin(assembly);

                if (plugin == null)
                    continue;

                Plugins.Add(plugin);
            }
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
                return Assembly.Load(File.ReadAllBytes(path));
            }
            catch (Exception exception)
            {
                Log.Error($"Error while loading an assembly at {path}! {exception}");
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
                    if (!type.BaseType.IsGenericType || (type.BaseType.GetGenericTypeDefinition() != typeof(Plugin<>) && type.BaseType.GetGenericTypeDefinition() != typeof(Plugin<,>)))
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

                    if (CheckPluginRequiredExiledVersion(plugin))
                        continue;

                    return plugin;
                }
            }
            catch (Exception exception)
            {
                Log.Error($"Error while initializing plugin {assembly.GetName().Name} (at {assembly.Location})! {exception}");
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

                    plugin.OnUnregisteringCommands();

                    plugin.OnDisabled();
                }
                catch (Exception exception)
                {
                    Log.Error($"Plugin \"{plugin.Name}\" threw an exception while reloading: {exception}");
                }
            }

            Plugins.Clear();

            LoadPlugins();

            ConfigManager.Reload();
            TranslationManager.Reload();

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
                    plugin.OnUnregisteringCommands();
                    plugin.OnDisabled();
                }
                catch (Exception exception)
                {
                    Log.Error($"Plugin \"{plugin.Name}\" threw an exception while disabling: {exception}");
                }
            }
        }

        private static bool CheckPluginRequiredExiledVersion(IPlugin<IConfig> plugin)
        {
            var requiredVersion = plugin.RequiredExiledVersion;
            var actualVersion = Version;

            // Check Major version
            // It's increased when an incompatible API change was made
            if (requiredVersion.Major != actualVersion.Major)
            {
                // Assume that if the Required Major version is greater than the Actual Major version,
                // Exiled is outdated
                if (requiredVersion.Major > actualVersion.Major)
                {
                    Log.Error($"You're running an older version of Exiled ({Version.ToString(3)})! {plugin.Name} won't be loaded! " +
                              $"Required version to load it: {plugin.RequiredExiledVersion.ToString(3)}");

                    return true;
                }
                else if (requiredVersion.Major < actualVersion.Major && !Config.ShouldLoadOutdatedPlugins)
                {
                    Log.Error($"You're running an older version of {plugin.Name} ({plugin.Version.ToString(3)})! " +
                              $"Its Required Major version is {requiredVersion.Major}, but excepted {actualVersion.Major}. ");

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Loads all dependencies.
        /// </summary>
        private static void LoadDependencies()
        {
            try
            {
                Log.Info($"Loading dependencies at {Paths.Dependencies}");

                // Quick dirty patch to fix rebbok putting Exiled.CustomItems in the wrong place
                if (File.Exists(Path.Combine(Paths.Dependencies, "Exiled.CustomItems.dll")))
                    File.Delete(Path.Combine(Paths.Dependencies, "Exiled.CustomItems.dll"));

                foreach (string dependency in Directory.GetFiles(Paths.Dependencies, "*.dll"))
                {
                    Assembly assembly = LoadAssembly(dependency);

                    if (assembly == null)
                        continue;

                    Locations[assembly] = dependency;

                    Dependencies.Add(assembly);

                    Log.Info($"Loaded dependency {assembly.GetName().Name}@{assembly.GetName().Version.ToString(3)}");
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
