// -----------------------------------------------------------------------
// <copyright file="Paths.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.IO;
    using System.Linq;

    /// <summary>
    /// A set of useful paths.
    /// </summary>
    public static class Paths
    {
        static Paths() => Reload();

        /// <summary>
        /// Gets AppData path.
        /// </summary>
        public static string AppData { get; } = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);

        /// <summary>
        /// Gets managed assemblies directory path.
        /// </summary>
        public static string ManagedAssemblies { get; } = Path.Combine(Path.Combine(Environment.CurrentDirectory, "SCPSL_Data"), "Managed");

        /// <summary>
        /// Gets the server's config path.
        /// </summary>
        public static string ServerConfig { get; } = FileManager.GetAppFolder(serverConfig: true);

        /// <summary>
        /// Gets or sets exiled directory path.
        /// </summary>
        public static string Exiled { get; set; }

        /// <summary>
        /// Gets or sets plugins path.
        /// </summary>
        public static string Plugins { get; set; }

        /// <summary>
        /// Gets or sets Dependencies directory path.
        /// </summary>
        public static string Dependencies { get; set; }

        /// <summary>
        /// Gets or sets the configuration folder path.
        /// </summary>
        public static string Configs { get; set; }

        /// <summary>
        /// Gets or sets individual configs directory path.
        /// </summary>
        public static string IndividualConfigs { get; set; }

        /// <summary>
        /// Gets or sets the loader configuration file path.
        /// </summary>
        public static string LoaderConfig { get; set; }

        /// <summary>
        /// Gets or sets the configuration file path.
        /// </summary>
        public static string Config { get; set; }

        /// <summary>
        /// Gets or sets the backup configs path.
        /// </summary>
        public static string BackupConfig { get; set; }

        /// <summary>
        /// Gets or sets translations path.
        /// </summary>
        public static string Translations { get; set; }

        /// <summary>
        /// Gets or sets individual translations directory path.
        /// </summary>
        public static string IndividualTranslations { get; set; }

        /// <summary>
        /// Gets or sets backup translations path.
        /// </summary>
        public static string BackupTranslations { get; set; }

        /// <summary>
        /// Gets or sets logs path.
        /// </summary>
        public static string Log { get; set; }

        /// <summary>
        /// Reloads all paths.
        /// </summary>
        /// <param name="rootDirectory">The new root directory.</param>
        public static void Reload(string rootDirectory = null)
        {
            rootDirectory ??= Path.Combine(AppData, "EXILED");

            Exiled = rootDirectory;
            Plugins = Path.Combine(Exiled, "Plugins");
            Dependencies = Path.Combine(Plugins, "dependencies");
            Configs = Path.Combine(Exiled, "Configs");
            IndividualConfigs = Path.Combine(Configs, "Plugins");
            LoaderConfig = PluginAPI.Loader.AssemblyLoader.InstalledPlugins.FirstOrDefault(x => x.PluginName == "Exiled Loader")?.MainConfigPath;
            Config = Path.Combine(Configs, $"{Server.Port}-config.yml");
            BackupConfig = Path.Combine(Configs, $"{Server.Port}-config.yml.old");
            IndividualTranslations = Path.Combine(Configs, "Translations");
            Translations = Path.Combine(Configs, $"{Server.Port}-translations.yml");
            BackupTranslations = Path.Combine(Configs, $"{Server.Port}-translations.yml.old");
            Log = Path.Combine(Exiled, $"{Server.Port}-RemoteAdminLog.txt");
        }

        /// <summary>
        /// Gets the config path of a plugin.
        /// </summary>
        /// <param name="pluginPrefix">The prefix of the plugin.</param>
        /// <returns>The config path of the plugin.</returns>
        public static string GetConfigPath(string pluginPrefix) => Path.Combine(IndividualConfigs, pluginPrefix, $"{Server.Port}.yml");

        /// <summary>
        /// Gets the translation path of a plugin.
        /// </summary>
        /// <param name="pluginPrefix">The prefix of the plugin.</param>
        /// <returns>The translation path of the plugin.</returns>
        public static string GetTranslationPath(string pluginPrefix) => Path.Combine(IndividualTranslations, pluginPrefix, $"{Server.Port}.yml");
    }
}