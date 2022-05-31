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
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using System.Threading;

    using CommandSystem.Commands.Shared;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Loader.Features;
    using Exiled.Loader.Features.Configs;
    using Exiled.Loader.Features.Configs.CustomConverters;

    using NorthwoodLib;

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

            ConfigManager.LoadLoaderConfigs();

            if (Config.Environment != EnvironmentType.Production && Config.Environment != EnvironmentType.ProductionDebug)
                Paths.Reload($"EXILED-{Config.Environment.ToString().ToUpper()}");
            if (Environment.CurrentDirectory.Contains("testing", StringComparison.OrdinalIgnoreCase))
                Paths.Reload($"EXILED-Testing");

            Directory.CreateDirectory(Paths.Configs);
            Directory.CreateDirectory(Paths.Plugins);
            Directory.CreateDirectory(Paths.Dependencies);

            if (Config.ConfigType == ConfigType.Separated)
                Directory.CreateDirectory(Paths.IndividualConfigs);
        }

        /// <summary>
        /// Gets the plugins list.
        /// </summary>
        public static SortedSet<IPlugin<IConfig>> Plugins { get; } = new(PluginPriorityComparer.Instance);

        /// <summary>
        /// Gets a dictionary that pairs assemblies with their associated plugins.
        /// </summary>
        public static Dictionary<Assembly, IPlugin<IConfig>> PluginAssemblies { get; } = new();

        /// <summary>
        /// Gets a dictionary containing the file paths of assemblies.
        /// </summary>
        public static Dictionary<Assembly, string> Locations { get; } = new();

        /// <summary>
        /// Gets the initialized global random class.
        /// </summary>
        public static Random Random { get; } = new();

        /// <summary>
        /// Gets the version of the assembly.
        /// </summary>
        public static Version Version { get; } = Assembly.GetExecutingAssembly().GetName().Version;

        /// <summary>
        /// Gets the configs of the plugin manager.
        /// </summary>
        public static Config Config { get; } = new();

        /// <summary>
        /// Gets a value indicating whether the debug should be shown or not.
        /// </summary>
        public static bool ShouldDebugBeShown => Config.Environment == EnvironmentType.Testing || Config.Environment == EnvironmentType.Development || Config.Environment == EnvironmentType.ProductionDebug;

        /// <summary>
        /// Gets plugin dependencies.
        /// </summary>
        public static List<Assembly> Dependencies { get; } = new();

        /// <summary>
        /// Gets or sets the serializer for configs and translations.
        /// </summary>
        public static ISerializer Serializer { get; set; } = new SerializerBuilder()
            .WithTypeConverter(new VectorsConverter())
            .WithTypeConverter(new ColorConverter())
            .WithTypeConverter(new AttachmentIdentifiersConverter())
            .WithTypeInspector(inner => new CommentGatheringTypeInspector(inner))
            .WithEmissionPhaseObjectGraphVisitor(args => new CommentsObjectGraphVisitor(args.InnerVisitor))
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .Build();

        /// <summary>
        /// Gets or sets the deserializer for configs and translations.
        /// </summary>
        public static IDeserializer Deserializer { get; set; } = new DeserializerBuilder()
            .WithTypeConverter(new VectorsConverter())
            .WithTypeConverter(new ColorConverter())
            .WithTypeConverter(new AttachmentIdentifiersConverter())
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
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? CheckUAC() : geteuid() == 0)
            {
                ServerConsole.AddLog("YOU ARE RUNNING THE SERVER AS ROOT / ADMINISTRATOR. THIS IS HIGHLY UNRECOMMENDED. PLEASE INSTALL YOUR SERVER AS A NON-ROOT/ADMIN USER.", ConsoleColor.Red);
                Thread.Sleep(5000);
            }

            if (dependencies?.Length > 0)
                Dependencies.AddRange(dependencies);

            if (!Config.IsEnabled)
            {
                Log.Warn("Exiled Loader is disabled. No plugins will be loaded.");
                return;
            }

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

            ServerConsole.AddLog($"Welcome to {LoaderMessages.GetMessage()}", ConsoleColor.Green);
        }

        /// <summary>
        /// Loads all plugins.
        /// </summary>
        public static void LoadPlugins()
        {
            foreach (string assemblyPath in Directory.GetFiles(Paths.Plugins, "*.dll"))
            {
                Assembly assembly = LoadAssembly(assemblyPath);

                if (assembly is null)
                    continue;

                Locations[assembly] = assemblyPath;

                Log.Info($"Loaded plugin {assembly.GetName().Name}@{assembly.GetName().Version.ToString(3)}");
            }

            foreach (Assembly assembly in Locations.Keys)
            {
                if (Locations[assembly].Contains("dependencies"))
                    continue;

                IPlugin<IConfig> plugin = CreatePlugin(assembly);

                if (plugin is null)
                    continue;

                PluginAssemblies.Add(assembly, plugin);
                Plugins.Add(plugin);
            }
        }

        /// <summary>
        /// Loads an assembly.
        /// </summary>
        /// <param name="path">The path to load the assembly from.</param>
        /// <returns>Returns the loaded assembly or <see langword="null"/>.</returns>
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
        /// <returns>Returns the created plugin instance or <see langword="null"/>.</returns>
        public static IPlugin<IConfig> CreatePlugin(Assembly assembly)
        {
            try
            {
                foreach (Type type in assembly.GetTypes())
                {
                    if (type.IsAbstract || type.IsInterface)
                    {
                        Log.Debug($"\"{type.FullName}\" is an interface or abstract class, skipping.", ShouldDebugBeShown);
                        continue;
                    }

                    if (!IsDerivedFromPlugin(type))
                    {
                        Log.Debug($"\"{type.FullName}\" does not inherit from Plugin<TConfig>, skipping.", ShouldDebugBeShown);
                        continue;
                    }

                    Log.Debug($"Loading type {type.FullName}", ShouldDebugBeShown);

                    IPlugin<IConfig> plugin = null;

                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor is not null)
                    {
                        Log.Debug("Public default constructor found, creating instance...", ShouldDebugBeShown);

                        plugin = constructor.Invoke(null) as IPlugin<IConfig>;
                    }
                    else
                    {
                        Log.Debug($"Constructor wasn't found, searching for a property with the {type.FullName} type...", ShouldDebugBeShown);

                        object value = Array.Find(type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public), property => property.PropertyType == type)?.GetValue(null);

                        if (value is not null)
                            plugin = value as IPlugin<IConfig>;
                    }

                    if (plugin is null)
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
            catch (ReflectionTypeLoadException reflectionTypeLoadException)
            {
                Log.Error($"Error while initializing plugin {assembly.GetName().Name} (at {assembly.Location})! {reflectionTypeLoadException}");

                foreach (var loaderException in reflectionTypeLoadException.LoaderExceptions)
                {
                    Log.Error(loaderException);
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
            List<IPlugin<IConfig>> toLoad = Plugins.ToList();

            foreach (IPlugin<IConfig> plugin in toLoad.ToList())
            {
                try
                {
                    if (plugin.Name.StartsWith("Exiled") && plugin.Config.IsEnabled)
                    {
                        plugin.OnEnabled();
                        plugin.OnRegisteringCommands();
                        toLoad.Remove(plugin);
                    }
                }
                catch (Exception e)
                {
                    Log.Error($"Plugin \"{plugin.Name}\" thew an exeption while enabling: {e}");
                }
            }

            foreach (IPlugin<IConfig> plugin in toLoad)
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
            PluginAssemblies.Clear();

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

        /// <summary>
        /// Gets a plugin with its prefix or name.
        /// </summary>
        /// <param name="args">The name or prefix of the plugin (Using the prefix is recommended).</param>
        /// <returns>The desired plugin, null if not found.</returns>
        public static IPlugin<IConfig> GetPlugin(string args) => Plugins.FirstOrDefault(x => x.Name == args || x.Prefix == args);

        /// <summary>
        /// Indicates that the passed type is derived from the plugin type.
        /// </summary>
        /// <param name="type">Type.</param>
        /// <returns><see langword="true"/> if passed type is derived from <see cref="Plugin{TConfig}"/> or <see cref="Plugin{TConfig, TTranslation}"/>, otherwise <see langword="false"/>.</returns>
        private static bool IsDerivedFromPlugin(Type type)
        {
            while (type is not null)
            {
                type = type.BaseType;

                if (type is { IsGenericType: true })
                {
                    var genericTypeDef = type.GetGenericTypeDefinition();

                    if (genericTypeDef == typeof(Plugin<>) || genericTypeDef == typeof(Plugin<,>))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool CheckPluginRequiredExiledVersion(IPlugin<IConfig> plugin)
        {
            Version requiredVersion = plugin.RequiredExiledVersion;
            Version actualVersion = Version;

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
                              $"Its Required Major version is {requiredVersion.Major}, but the actual version is: {actualVersion.Major}. This plugin will not be loaded!");

                    return true;
                }
            }

            return false;
        }

#pragma warning disable SA1201
#pragma warning disable SA1300
#pragma warning disable SA1313
#pragma warning disable SA1600
#pragma warning disable SA1602
#pragma warning disable CS1591
        [DllImport("libc")]
        private static extern uint geteuid();

        [DllImport("advapi32.dll", SetLastError = true)]
        private static extern bool GetTokenInformation(
            IntPtr TokenHandle,
            TOKEN_INFORMATION_CLASS TokenInformationClass,
            IntPtr TokenInformation,
            uint TokenInformationLength,
            out uint ReturnLength);

        public enum TOKEN_INFORMATION_CLASS
        {
            TokenUser = 1,
            TokenGroups,
            TokenPrivileges,
            TokenOwner,
            TokenPrimaryGroup,
            TokenDefaultDacl,
            TokenSource,
            TokenType,
            TokenImpersonationLevel,
            TokenStatistics,
            TokenRestrictedSids,
            TokenSessionId,
            TokenGroupsAndPrivileges,
            TokenSessionReference,
            TokenSandBoxInert,
            TokenAuditPolicy,
            TokenOrigin,
            TokenElevationType,
            TokenLinkedToken,
            TokenElevation,
            TokenHasRestrictions,
            TokenAccessInformation,
            TokenVirtualizationAllowed,
            TokenVirtualizationEnabled,
            TokenIntegrityLevel,
            TokenUIAccess,
            TokenMandatoryPolicy,
            TokenLogonSid,
            MaxTokenInfoClass,
        }

        public enum TOKEN_ELEVATION_TYPE
        {
            TokenElevationTypeDefault = 1,
            TokenElevationTypeFull,
            TokenElevationTypeLimited,
        }
#pragma warning restore

        /// <summary>
        /// Check UAC elevated (for Windows).
        /// </summary>
        private static bool CheckUAC()
        {
            TOKEN_ELEVATION_TYPE tet = TOKEN_ELEVATION_TYPE.TokenElevationTypeDefault;
            uint tetSize = (uint)Marshal.SizeOf((int)tet);
            IntPtr tetPtr = Marshal.AllocHGlobal((int)tetSize);
            try
            {
                if (GetTokenInformation(WindowsIdentity.GetCurrent().Token, TOKEN_INFORMATION_CLASS.TokenElevationType, tetPtr, tetSize, out _))
                    tet = (TOKEN_ELEVATION_TYPE)Marshal.ReadInt32(tetPtr);
            }
            finally
            {
                Marshal.FreeHGlobal(tetPtr);
            }

            return tet == TOKEN_ELEVATION_TYPE.TokenElevationTypeFull;
        }

        /// <summary>
        /// Loads all dependencies.
        /// </summary>
        private static void LoadDependencies()
        {
            try
            {
                Log.Info($"Loading dependencies at {Paths.Dependencies}");

                foreach (string dependency in Directory.GetFiles(Paths.Dependencies, "*.dll"))
                {
                    Assembly assembly = LoadAssembly(dependency);

                    if (assembly is null)
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
