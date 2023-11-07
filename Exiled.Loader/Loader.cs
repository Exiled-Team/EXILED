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
    using System.IO.Compression;
    using System.Linq;
    using System.Reflection;
    using System.Runtime.InteropServices;
    using System.Security.Principal;
    using System.Threading;

    using API.Enums;
    using API.Interfaces;

    using CommandSystem.Commands.Shared;

    using Exiled.API.Features;
    using Features;
    using Features.Configs;
    using Features.Configs.CustomConverters;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NodeDeserializers;

    /// <summary>
    /// Used to handle plugins.
    /// </summary>
    public class Loader
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Loader"/> class.
        /// </summary>
        public Loader()
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

            if (LoaderPlugin.Config.ConfigType == ConfigType.Separated)
                Directory.CreateDirectory(Paths.IndividualConfigs);
        }

        /// <summary>
        /// Gets the plugins list.
        /// </summary>
        public static SortedSet<IPlugin<IConfig>> Plugins { get; } = new(PluginPriorityComparer.Instance);

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
            .WithEventEmitter(eventEmitter => new TypeAssigningEventEmitter(eventEmitter))
            .WithTypeInspector(inner => new CommentGatheringTypeInspector(inner))
            .WithEmissionPhaseObjectGraphVisitor(args => new CommentsObjectGraphVisitor(args.InnerVisitor))
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .DisableAliases()
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
        /// Loads all plugins.
        /// </summary>
        public static void LoadPlugins()
        {
            File.Delete(Path.Combine(Paths.Plugins, "Exiled.Updater.dll"));

            foreach (string assemblyPath in Directory.GetFiles(Paths.Plugins, "*.dll"))
            {
                Assembly assembly = LoadAssembly(assemblyPath);

                if (assembly is null)
                    continue;

                Locations[assembly] = assemblyPath;
            }

            foreach (Assembly assembly in Locations.Keys)
            {
                if (Locations[assembly].Contains("dependencies"))
                    continue;

                IPlugin<IConfig> plugin = CreatePlugin(assembly);

                if (plugin is null)
                    continue;

                AssemblyInformationalVersionAttribute attribute = plugin.Assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>();

                Log.Info($"Loaded plugin {plugin.Name}@{(plugin.Version is not null ? $"{plugin.Version.Major}.{plugin.Version.Minor}.{plugin.Version.Build}" : attribute is not null ? attribute.InformationalVersion : string.Empty)}");

                Server.PluginAssemblies.Add(assembly, plugin);
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
                Assembly assembly = Assembly.Load(File.ReadAllBytes(path));

                ResolveAssemblyEmbeddedResources(assembly);

                return assembly;
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
                        Log.Debug($"\"{type.FullName}\" is an interface or abstract class, skipping.");
                        continue;
                    }

                    if (!IsDerivedFromPlugin(type))
                    {
                        Log.Debug($"\"{type.FullName}\" does not inherit from Plugin<TConfig>, skipping.");
                        continue;
                    }

                    Log.Debug($"Loading type {type.FullName}");

                    IPlugin<IConfig> plugin = null;

                    ConstructorInfo constructor = type.GetConstructor(Type.EmptyTypes);
                    if (constructor is not null)
                    {
                        Log.Debug("Public default constructor found, creating instance...");

                        plugin = constructor.Invoke(null) as IPlugin<IConfig>;
                    }
                    else
                    {
                        Log.Debug($"Constructor wasn't found, searching for a property with the {type.FullName} type...");

                        object value = Array.Find(type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public), property => property.PropertyType == type)?.GetValue(null);

                        if (value is not null)
                            plugin = value as IPlugin<IConfig>;
                    }

                    if (plugin is null)
                    {
                        Log.Error($"{type.FullName} is a valid plugin, but it cannot be instantiated! It either doesn't have a public default constructor without any arguments or a static property of the {type.FullName} type!");

                        continue;
                    }

                    Log.Debug($"Instantiated type {type.FullName}");

                    if (CheckPluginRequiredExiledVersion(plugin))
                        continue;

                    return plugin;
                }
            }
            catch (ReflectionTypeLoadException reflectionTypeLoadException)
            {
                Log.Error($"Error while initializing plugin {assembly.GetName().Name} (at {assembly.Location})! {reflectionTypeLoadException}");

                foreach (Exception loaderException in reflectionTypeLoadException.LoaderExceptions)
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

                    if (plugin.Config.Debug)
                        Log.DebugEnabled.Add(plugin.Assembly);
                }
                catch (Exception exception)
                {
                    Log.Error($"Plugin \"{plugin.Name}\" threw an exeption while enabling: {exception}");
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
            Server.PluginAssemblies.Clear();
            Locations.Clear();

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
        /// Runs the plugin manager, by loading all dependencies, plugins, configs and then enables all plugins.
        /// </summary>
        /// <param name="dependencies">The dependencies that could have been loaded by Exiled.Bootstrap.</param>
        /// <returns>A MEC <see cref="IEnumerator{T}"/>.</returns>
        public IEnumerator<float> Run(Assembly[] dependencies = null)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? CheckUAC() : geteuid() == 0)
            {
                ServerConsole.AddLog("YOU ARE RUNNING THE SERVER AS ROOT / ADMINISTRATOR. THIS IS HIGHLY UNRECOMMENDED. PLEASE INSTALL YOUR SERVER AS A NON-ROOT/ADMIN USER.", ConsoleColor.DarkRed);
                Thread.Sleep(5000);
            }

            if (LoaderPlugin.Config.EnableAutoUpdates)
            {
                Thread thread = new(() =>
                {
                    Updater updater = Updater.Initialize(LoaderPlugin.Config);
                    updater.CheckUpdate();
                })
                {
                    Name = "Exiled Updater",
                    Priority = ThreadPriority.AboveNormal,
                };

                thread.Start();
            }

            if (!LoaderPlugin.Config.ShouldLoadOutdatedExiled &&
                !GameCore.Version.CompatibilityCheck(
                (byte)AutoUpdateFiles.RequiredSCPSLVersion.Major,
                (byte)AutoUpdateFiles.RequiredSCPSLVersion.Minor,
                (byte)AutoUpdateFiles.RequiredSCPSLVersion.Revision,
                GameCore.Version.Major,
                GameCore.Version.Minor,
                GameCore.Version.Revision,
                GameCore.Version.BackwardCompatibility,
                GameCore.Version.BackwardRevision))
            {
                ServerConsole.AddLog($"Exiled is outdated, a new version will be installed automatically as soon as it's available.\nSCP:SL: {GameCore.Version.VersionString} Exiled Supported Version: {AutoUpdateFiles.RequiredSCPSLVersion}", ConsoleColor.DarkRed);
                yield break;
            }

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

            ServerConsole.AddLog($"Welcome to {LoaderMessages.GetMessage()}", ConsoleColor.Green);
        }

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
                    Type genericTypeDef = type.GetGenericTypeDefinition();

                    if (genericTypeDef == typeof(Plugin<>) || genericTypeDef == typeof(Plugin<,>))
                        return true;
                }
            }

            return false;
        }

        private static bool CheckPluginRequiredExiledVersion(IPlugin<IConfig> plugin)
        {
            if (plugin.IgnoreRequiredVersionCheck)
                return false;

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
                    Log.Error(
                        $"You're running an older version of Exiled ({Version.ToString(3)})! {plugin.Name} won't be loaded! " +
                        $"Required version to load it: {plugin.RequiredExiledVersion.ToString(3)}");

                    return true;
                }
                else if ((requiredVersion.Major < actualVersion.Major) && !LoaderPlugin.Config.ShouldLoadOutdatedPlugins)
                {
                    Log.Error(
                        $"You're running an older version of {plugin.Name} ({plugin.Version.ToString(3)})! " +
                        $"Its Required Major version is {requiredVersion.Major}, but the actual version is: {actualVersion.Major}. This plugin will not be loaded!");

                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Attempts to load Embedded (compressed) assemblies from specified Assembly.
        /// </summary>
        /// <param name="target">Assembly to check for embedded assemblies.</param>
        private static void ResolveAssemblyEmbeddedResources(Assembly target)
        {
            try
            {
                Log.Debug($"Attempting to load embedded resources for {target.FullName}");

                string[] resourceNames = target.GetManifestResourceNames();

                foreach (string name in resourceNames)
                {
                    Log.Debug($"Found resource {name}");

                    if (name.EndsWith(".dll", StringComparison.OrdinalIgnoreCase))
                    {
                        using MemoryStream stream = new();

                        Log.Debug($"Loading resource {name}");

                        Stream dataStream = target.GetManifestResourceStream(name);

                        if (dataStream == null)
                        {
                            Log.Error($"Unable to resolve resource {name} Stream was null");
                            continue;
                        }

                        dataStream.CopyTo(stream);

                        Dependencies.Add(Assembly.Load(stream.ToArray()));

                        Log.Debug($"Loaded resource {name}");
                    }
                    else if (name.EndsWith(".dll.compressed", StringComparison.OrdinalIgnoreCase))
                    {
                        Stream dataStream = target.GetManifestResourceStream(name);

                        if (dataStream == null)
                        {
                            Log.Error($"Unable to resolve resource {name} Stream was null");
                            continue;
                        }

                        using DeflateStream stream = new(dataStream, CompressionMode.Decompress);
                        using MemoryStream memStream = new();

                        Log.Debug($"Loading resource {name}");

                        stream.CopyTo(memStream);

                        Dependencies.Add(Assembly.Load(memStream.ToArray()));

                        Log.Debug($"Loaded resource {name}");
                    }
                }
            }
            catch (Exception exception)
            {
                Log.Error($"Failed to load embedded resources from {target.FullName}: {exception}");
            }
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
