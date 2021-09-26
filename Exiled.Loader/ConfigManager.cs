// -----------------------------------------------------------------------
// <copyright file="ConfigManager.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using YamlDotNet.Core;

    /// <summary>
    /// Used to handle plugin configs.
    /// </summary>
    public static class ConfigManager
    {
        /// <summary>
        /// Loads the loader configs.
        /// </summary>
        public static void LoadLoaderConfigs()
        {
            if (!File.Exists(Paths.LoaderConfig))
            {
                Log.Warn($"Exiled.Loader doesn't have default configs, generating...");
                File.WriteAllText(Paths.LoaderConfig, Loader.Serializer.Serialize(Loader.Config));
                return;
            }

            try
            {
                Loader.Config.CopyProperties(Loader.Deserializer.Deserialize<Config>(File.ReadAllText(Paths.LoaderConfig)));
            }
            catch (YamlException yamlException)
            {
                Log.Error($"Exiled.Loader configs could not be loaded, some of them are in a wrong format, default configs will be loaded instead! {yamlException}");
            }
        }

        /// <summary>
        /// Loads all plugin configs in raw text.
        /// </summary>
        /// <param name="rawConfigs">The raw configs to be loaded.</param>
        /// <returns>Returns a dictionary of loaded configs.</returns>
        public static SortedDictionary<string, IConfig> LoadConfigs(string rawConfigs)
        {
            try
            {
                Log.Info($"Loading plugin configs... ({Loader.Config.ConfigType})");

                SortedDictionary<string, IConfig> deserializedConfigs = new SortedDictionary<string, IConfig>(StringComparer.Ordinal);

                if (Loader.Config.ConfigType == ConfigType.Default)
                {
                    Dictionary<string, object> rawDeserializedConfigs = Loader.Deserializer.Deserialize<Dictionary<string, object>>(rawConfigs) ?? new Dictionary<string, object>();

                    foreach (IPlugin<IConfig> plugin in Loader.Plugins)
                    {
                        if (!rawDeserializedConfigs.TryGetValue(plugin.Prefix, out object rawDeserializedConfig))
                        {
                            Log.Warn($"{plugin.Name} doesn't have default configs, generating...");

                            deserializedConfigs.Add(plugin.Prefix, plugin.Config);
                        }
                        else
                        {
                            try
                            {
                                deserializedConfigs.Add(plugin.Prefix, (IConfig)Loader.Deserializer.Deserialize(Loader.Serializer.Serialize(rawDeserializedConfig), plugin.Config.GetType()));

                                plugin.Config.CopyProperties(deserializedConfigs[plugin.Prefix]);
                            }
                            catch (YamlException yamlException)
                            {
                                Log.Error($"{plugin.Name} configs could not be loaded, some of them are in a wrong format, default configs will be loaded instead! {yamlException}");

                                deserializedConfigs.Add(plugin.Prefix, plugin.Config);
                            }
                        }
                    }
                }
                else
                {
                    foreach (IPlugin<IConfig> plugin in Loader.Plugins)
                    {
                        if (File.Exists(plugin.ConfigPath))
                        {
                            try
                            {
                                deserializedConfigs.Add(plugin.Prefix, (IConfig)Loader.Deserializer.Deserialize(File.ReadAllText(plugin.ConfigPath), plugin.Config.GetType()));
                                plugin.Config.CopyProperties(deserializedConfigs[plugin.Prefix]);
                            }
                            catch (YamlException yamlException)
                            {
                                Log.Error($"{plugin.Name} configs could not be loaded, some of them are in a wrong format, default configs will be loaded instead! {yamlException}");

                                deserializedConfigs.Add(plugin.Prefix, plugin.Config);
                            }
                        }
                        else
                        {
                            Log.Warn($"{plugin.Name} doesn't have default configs, generating...");
                            deserializedConfigs.Add(plugin.Prefix, plugin.Config);
                        }
                    }
                }

                Log.Info("Plugin configs loaded successfully!");

                return deserializedConfigs;
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while loading configs! {exception}");

                return null;
            }
        }

        /// <summary>
        /// Reads, Loads and Saves plugin configs.
        /// </summary>
        /// <returns>Returns a value indicating if the reloading process has been completed successfully or not.</returns>
        public static bool Reload() => SaveAll(LoadConfigs(Read()));

        /// <summary>
        /// Saves plugin configs.
        /// </summary>
        /// <param name="configs">The configs to be saved, already serialized in yaml format.</param>
        /// <param name="pluginPrefix">The prefix of the plugin which configs are being saved. Null for the default config file.</param>
        /// <returns>Returns a value indicating whether the configs have been saved successfully or not.</returns>
        public static bool Save(string configs, string pluginPrefix = null)
        {
            try
            {
                if (pluginPrefix != null && Loader.Config.ConfigType == ConfigType.Separated && !Directory.Exists(Path.Combine(Paths.IndividualConfigs, pluginPrefix)))
                    Directory.CreateDirectory(Path.Combine(Paths.IndividualConfigs, pluginPrefix));

                File.WriteAllText(Paths.GetConfigPath(pluginPrefix), configs ?? string.Empty);

                return true;
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while saving configs to {Paths.GetConfigPath(pluginPrefix)} path: {exception}");

                return false;
            }
        }

        /// <summary>
        /// Saves plugin configs.
        /// </summary>
        /// <param name="configs">The configs to be saved.</param>
        /// <returns>Returns a value indicating whether the configs have been saved successfully or not.</returns>
        public static bool SaveAll(SortedDictionary<string, IConfig> configs)
        {
            try
            {
                if (configs == null || configs.Count == 0)
                    return false;

                if (Loader.Config.ConfigType == ConfigType.Default)
                {
                    return Save(Loader.Serializer.Serialize(configs));
                }

                foreach (var config in configs)
                {
                    Save(Loader.Serializer.Serialize(config.Value), config.Key);
                }

                return true;
            }
            catch (YamlException yamlException)
            {
                Log.Error($"An error has occurred while serializing configs: {yamlException}");

                return false;
            }
        }

        /// <summary>
        /// Read all plugin configs.
        /// </summary>
        /// <returns>Returns the read configs.</returns>
        public static string Read()
        {
            if (Loader.Config.ConfigType != ConfigType.Default)
                return string.Empty;

            try
            {
                if (File.Exists(Paths.Config))
                    return File.ReadAllText(Paths.Config);
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while reading configs from {Paths.Config} path: {exception}");
            }

            return string.Empty;
        }

        /// <summary>
        /// Clears the configs.
        /// </summary>
        /// <returns>Returns a value indicating whether configs have been cleared successfully or not.</returns>
        public static bool Clear() => Save(string.Empty);

        /// <summary>
        /// Reloads RemoteAdmin configs.
        /// </summary>
        public static void ReloadRemoteAdmin()
        {
            ServerStatic.RolesConfig = new YamlConfig(ServerStatic.RolesConfigPath);
            ServerStatic.SharedGroupsConfig = (GameCore.ConfigSharing.Paths[4] == null) ? null : new YamlConfig(GameCore.ConfigSharing.Paths[4] + "shared_groups.txt");
            ServerStatic.SharedGroupsMembersConfig = (GameCore.ConfigSharing.Paths[5] == null) ? null : new YamlConfig(GameCore.ConfigSharing.Paths[5] + "shared_groups_members.txt");
            ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
            ServerStatic.GetPermissionsHandler().RefreshPermissions();

            foreach (Player p in Player.List)
            {
                p.ReferenceHub.serverRoles.SetGroup(null, false, false, false);
                p.ReferenceHub.serverRoles.RefreshPermissions();
            }
        }
    }
}
