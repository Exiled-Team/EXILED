// -----------------------------------------------------------------------
// <copyright file="ConfigManager.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader {
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;

    using YamlDotNet.Core;

    /// <summary>
    /// Used to handle plugin configs.
    /// </summary>
    public static class ConfigManager {
        /// <summary>
        /// Loads all plugin configs.
        /// </summary>
        /// <param name="rawConfigs">The raw configs to be loaded.</param>
        /// <returns>Returns a dictionary of loaded configs.</returns>
        public static SortedDictionary<string, IConfig> LoadSorted(string rawConfigs) {
            try {
                Log.Info("Loading plugin configs...");

                Dictionary<string, object> rawDeserializedConfigs = Loader.Deserializer.Deserialize<Dictionary<string, object>>(rawConfigs) ?? new Dictionary<string, object>();
                SortedDictionary<string, IConfig> deserializedConfigs = new SortedDictionary<string, IConfig>(StringComparer.Ordinal);

                if (!rawDeserializedConfigs.TryGetValue("exiled_loader", out object rawDeserializedConfig)) {
                    Log.Warn($"Exiled.Loader doesn't have default configs, generating...");

                    deserializedConfigs.Add("exiled_loader", Loader.Config);
                }
                else {
                    deserializedConfigs.Add("exiled_loader", Loader.Deserializer.Deserialize<Config>(Loader.Serializer.Serialize(rawDeserializedConfig)));

                    Loader.Config.CopyProperties(deserializedConfigs["exiled_loader"]);
                }

                foreach (IPlugin<IConfig> plugin in Loader.Plugins) {
                    if (!rawDeserializedConfigs.TryGetValue(plugin.Prefix, out rawDeserializedConfig)) {
                        Log.Warn($"{plugin.Name} doesn't have default configs, generating...");

                        deserializedConfigs.Add(plugin.Prefix, plugin.Config);
                    }
                    else {
                        try {
                            deserializedConfigs.Add(plugin.Prefix, (IConfig)Loader.Deserializer.Deserialize(Loader.Serializer.Serialize(rawDeserializedConfig), plugin.Config.GetType()));

                            plugin.Config.CopyProperties(deserializedConfigs[plugin.Prefix]);
                        }
                        catch (YamlException yamlException) {
                            Log.Error($"{plugin.Name} configs could not be loaded, some of them are in a wrong format, default configs will be loaded instead! {yamlException}");

                            deserializedConfigs.Add(plugin.Prefix, plugin.Config);
                        }
                    }
                }

                // Make sure that no keys in the config file were discarded.
                if (!rawDeserializedConfigs.Keys.All(deserializedConfigs.ContainsKey)) {
                    Log.Warn("Missing plugins have been detected in the config. A backup config file will be created at \"" + Paths.BackupConfig + "\".");
                    File.WriteAllText(Paths.BackupConfig, rawConfigs);
                }

                Log.Info("Plugin configs loaded successfully!");

                return deserializedConfigs;
            }
            catch (Exception exception) {
                Log.Error($"An error has occurred while loading configs! {exception}");

                return null;
            }
        }

        /// <summary>
        /// Reads, Loads and Saves plugin configs.
        /// </summary>
        /// <returns>Returns a value indicating if the reloading process has been completed successfully or not.</returns>
        public static bool Reload() => Save(LoadSorted(Read()));

        /// <summary>
        /// Saves plugin configs.
        /// </summary>
        /// <param name="configs">The configs to be saved, already serialized in yaml format.</param>
        /// <returns>Returns a value indicating whether the configs have been saved successfully or not.</returns>
        public static bool Save(string configs) {
            try {
                File.WriteAllText(Paths.Config, configs ?? string.Empty);

                return true;
            }
            catch (Exception exception) {
                Log.Error($"An error has occurred while saving configs to {Paths.Config} path: {exception}");

                return false;
            }
        }

        /// <summary>
        /// Saves plugin configs.
        /// </summary>
        /// <param name="configs">The configs to be saved.</param>
        /// <returns>Returns a value indicating whether the configs have been saved successfully or not.</returns>
        public static bool Save(SortedDictionary<string, IConfig> configs) {
            try {
                if (configs == null || configs.Count == 0)
                    return false;

                return Save(Loader.Serializer.Serialize(configs));
            }
            catch (YamlException yamlException) {
                Log.Error($"An error has occurred while serializing configs: {yamlException}");

                return false;
            }
        }

        /// <summary>
        /// Read all plugin configs.
        /// </summary>
        /// <returns>Returns the read configs.</returns>
        public static string Read() {
            try {
                if (File.Exists(Paths.Config))
                    return File.ReadAllText(Paths.Config);
            }
            catch (Exception exception) {
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
        public static void ReloadRemoteAdmin() {
            ServerStatic.RolesConfig = new YamlConfig(ServerStatic.RolesConfigPath);
            ServerStatic.SharedGroupsConfig = (GameCore.ConfigSharing.Paths[4] == null) ? null : new YamlConfig(GameCore.ConfigSharing.Paths[4] + "shared_groups.txt");
            ServerStatic.SharedGroupsMembersConfig = (GameCore.ConfigSharing.Paths[5] == null) ? null : new YamlConfig(GameCore.ConfigSharing.Paths[5] + "shared_groups_members.txt");
            ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
            ServerStatic.GetPermissionsHandler().RefreshPermissions();

            foreach (Player p in Player.List) {
                p.ReferenceHub.serverRoles.SetGroup(null, false, false, false);
                p.ReferenceHub.serverRoles.RefreshPermissions();
            }
        }
    }
}
