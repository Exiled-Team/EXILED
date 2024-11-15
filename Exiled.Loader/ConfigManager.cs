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
    using System.Linq;
    using System.Reflection;

    using API.Enums;
    using API.Extensions;
    using API.Interfaces;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core.Generic.Pools;
    using YamlDotNet.Core;

    using Serialization = API.Features.ConfigSubsystem;

    /// <summary>
    /// Used to handle plugin configs.
    /// </summary>
    public static class ConfigManager
    {
        /// <summary>
        /// Implements the <see cref="ConfigSubsystem"/>.
        /// </summary>
        public static void LoadConfigSubsystem() => Serialization.LoadAll(Loader.Plugins.Select(asm => asm.Assembly));

        /// <summary>
        /// Loads all the plugin configs.
        /// </summary>
        /// <param name="rawConfigs">The raw configs to be loaded.</param>
        /// <returns>Returns a dictionary of loaded configs.</returns>
        public static SortedDictionary<string, IConfig> LoadSorted(string rawConfigs)
        {
            try
            {
                Log.Info($"Loading plugin configs... ({LoaderPlugin.Config.ConfigType})");

                Dictionary<string, object> rawDeserializedConfigs = Serialization.Deserializer.Deserialize<Dictionary<string, object>>(rawConfigs) ?? DictionaryPool<string, object>.Pool.Get();
                SortedDictionary<string, IConfig> deserializedConfigs = new(StringComparer.Ordinal);

                foreach (IPlugin<IConfig> plugin in Loader.Plugins)
                {
                    if (plugin.Config.GetType().GetCustomAttribute<ConfigAttribute>() is not null)
                        continue;

                    deserializedConfigs.Add(plugin.Prefix, plugin.LoadConfig(rawDeserializedConfigs));
                }

                // Make sure that no keys in the config file were discarded. (Individual can ignore this since rawDeserializedConfigs is null)
                if (!rawDeserializedConfigs.Keys.All(deserializedConfigs.ContainsKey))
                {
                    Log.Warn("Missing plugins have been detected in the config. A backup config file will be created at \"" + Paths.BackupConfig + "\".");
                    File.WriteAllText(Paths.BackupConfig, rawConfigs);
                }

                Log.Info("Plugin configs loaded successfully!");

                DictionaryPool<string, object>.Pool.Return(rawDeserializedConfigs);
                return deserializedConfigs;
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while loading configs! {exception}");

                return null;
            }
        }

        /// <summary>
        /// Loads the config of a plugin using the distribution.
        /// </summary>
        /// <param name="plugin">The plugin which config will be loaded.</param>
        /// <param name="rawConfigs">The raw configs to detect if the plugin already has generated configs.</param>
        /// <returns>The <see cref="IConfig"/> of the plugin.</returns>
        public static IConfig LoadConfig(this IPlugin<IConfig> plugin, Dictionary<string, object> rawConfigs = null) =>
            plugin.Config.GetType().GetCustomAttribute<ConfigAttribute>() is not null ? null : LoaderPlugin.Config.ConfigType switch
            {
                ConfigType.Separated => LoadSeparatedConfig(plugin),
                _ => LoadDefaultConfig(plugin, rawConfigs),
            };

        /// <summary>
        /// Loads the config of a plugin using the default distribution.
        /// </summary>
        /// <param name="plugin">The plugin which config will be loaded.</param>
        /// <param name="rawConfigs">The raw configs to detect if the plugin already has generated configs.</param>
        /// <returns>The <see cref="IConfig"/> of the plugin.</returns>
        public static IConfig LoadDefaultConfig(this IPlugin<IConfig> plugin, Dictionary<string, object> rawConfigs)
        {
            if (plugin.Config.GetType().GetCustomAttribute<ConfigAttribute>() is not null)
                return null;

            rawConfigs ??= Serialization.Deserializer.Deserialize<Dictionary<string, object>>(Read()) ?? new Dictionary<string, object>();

            if (!rawConfigs.TryGetValue(plugin.Prefix, out object rawDeserializedConfig) && plugin.Config.GetType().GetCustomAttribute<ConfigAttribute>() is null)
            {
                Log.Warn($"{plugin.Name} doesn't have default configs, generating...");

                return plugin.Config;
            }

            IConfig config;

            try
            {
                string rawConfigString = Serialization.Serializer.Serialize(rawDeserializedConfig);
                config = ValidateConfig(plugin, (IConfig)Serialization.Deserializer.Deserialize(rawConfigString, plugin.Config.GetType()));
                plugin.Config.CopyProperties(config);
            }
            catch (YamlException yamlException)
            {
                Log.Error($"{plugin.Name} configs could not be loaded, some of them are in a wrong format, default configs will be loaded instead!\n{yamlException}");
                config = plugin.Config;
            }

            return config;
        }

        /// <summary>
        /// Loads the config of a plugin using the separated distribution.
        /// </summary>
        /// <param name="plugin">The plugin which its config will be loaded.</param>
        /// <returns>The <see cref="IConfig"/> of the plugin.</returns>
        public static IConfig LoadSeparatedConfig(this IPlugin<IConfig> plugin)
        {
            if (plugin.Config.GetType().GetCustomAttribute<ConfigAttribute>() is not null)
                return null;

            if (!File.Exists(plugin.ConfigPath))
            {
                Log.Warn($"{plugin.Name} doesn't have default configs, generating...");
                return plugin.Config;
            }

            IConfig config;

            try
            {
                config = ValidateConfig(plugin, (IConfig)Serialization.Deserializer.Deserialize(File.ReadAllText(plugin.ConfigPath), plugin.Config.GetType()));
                plugin.Config.CopyProperties(config);
            }
            catch (YamlException yamlException)
            {
                Log.Error($"{plugin.Name} configs could not be loaded, some of them are in a wrong format, default configs will be loaded instead!\n{yamlException}");
                return plugin.Config;
            }

            return config;
        }

        /// <summary>
        /// Validates a config.
        /// </summary>
        /// <param name="plugin">The plugin which its config will be validated.</param>
        /// <param name="config">A <see cref="IConfig"/> to validate.</param>
        /// <returns>Returns a validated config.</returns>
        public static IConfig ValidateConfig(this IPlugin<IConfig> plugin, IConfig config)
        {
            try
            {
                Type configType = config.GetType();
                Type pluginConfigType = plugin.Config.GetType();

                int validations = 0;

                foreach (PropertyInfo propertyInfo in configType.GetProperties())
                {
                    foreach (Attribute attribute in propertyInfo.GetCustomAttributes())
                    {
                        if (attribute is not IValidator validator)
                            continue;

                        object value = propertyInfo.GetValue(config, null);
                        object defaultValue = pluginConfigType.GetProperty(propertyInfo.Name)?.GetValue(plugin.Config, null);

                        if (!validator.Validate(value))
                        {
                            Log.Error($"{plugin.Name} config value {propertyInfo.Name} ({value}) hasn't passed the validation (attribute ({attribute.GetType().Name})). Default value ({defaultValue}) will be used instead.");
                            propertyInfo.SetValue(config, defaultValue);
                        }
                        else
                        {
                            validations++;
                        }
                    }
                }

                if (validations > 0)
                {
                    Log.Info($"Successfully passed {validations} validations");
                }

                return config;
            }
            catch (Exception e)
            {
                Log.Error($"An error occured while validation the config of the plugin {plugin.Name}: {e}");
                return config;
            }
        }

        /// <summary>
        /// Reads, loads, and saves plugin configs.
        /// </summary>
        /// <returns>Returns a value indicating if the reloading process has been completed successfully or not.</returns>
        public static bool Reload() => Save(LoadSorted(Read()));

        /// <summary>
        /// Saves default distribution configs.
        /// </summary>
        /// <param name="configs">The configs to be saved, already serialized in yaml format.</param>
        /// <returns>Returns a value indicating whether the configs have been saved successfully or not.</returns>
        public static bool SaveDefaultConfig(string configs)
        {
            try
            {
                File.WriteAllText(Paths.Config, configs ?? string.Empty);

                return true;
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while saving configs to {Paths.Config} path: {exception}");

                return false;
            }
        }

        /// <summary>
        /// Saves separated distribution plugin configs.
        /// </summary>
        /// <param name="pluginPrefix">The prefix of the plugin which its config is going to be saved.</param>
        /// <param name="configs">The configs to be saved, already serialized in yaml format.</param>
        /// <returns>Returns a value indicating whether the configs have been saved successfully or not.</returns>
        public static bool SaveSeparatedConfig(this string pluginPrefix, string configs)
        {
            string configPath = Paths.GetConfigPath(pluginPrefix);

            try
            {
                Directory.CreateDirectory(Path.Combine(Paths.IndividualConfigs, pluginPrefix));
                File.WriteAllText(configPath, configs ?? string.Empty);

                return true;
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while saving configs to {configPath} path: {exception}");

                return false;
            }
        }

        /// <summary>
        /// Saves plugin configs.
        /// </summary>
        /// <param name="configs">The configs to be saved.</param>
        /// <returns>Returns a value indicating whether the configs have been saved successfully or not.</returns>
        public static bool Save(SortedDictionary<string, IConfig> configs)
        {
            try
            {
                if (configs is null || configs.Count == 0)
                    return false;

                if (LoaderPlugin.Config.ConfigType == ConfigType.Merged)
                {
                    return SaveDefaultConfig(Serialization.Serializer.Serialize(configs));
                }

                return configs.All(config => SaveSeparatedConfig(config.Key, Serialization.Serializer.Serialize(config.Value)));
            }
            catch (YamlException yamlException)
            {
                Log.Error($"An error has occurred while serializing configs:\n{yamlException}");

                return false;
            }
        }

        /// <summary>
        /// Read all plugin configs.
        /// </summary>
        /// <returns>Returns the read configs.</returns>
        public static string Read()
        {
            if (LoaderPlugin.Config.ConfigType != ConfigType.Merged)
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
        public static bool Clear()
        {
            try
            {
                if (LoaderPlugin.Config.ConfigType == ConfigType.Merged)
                {
                    SaveDefaultConfig(string.Empty);
                    return true;
                }

                return Loader.Plugins.All(plugin => SaveSeparatedConfig(plugin.Prefix, string.Empty));
            }
            catch (Exception e)
            {
                Log.Error("An error has occurred while clearing configs:\n" + e);
                return false;
            }
        }

        /// <summary>
        /// Reloads RemoteAdmin configs.
        /// </summary>
        public static void ReloadRemoteAdmin()
        {
            ServerStatic.RolesConfig = new YamlConfig(ServerStatic.RolesConfigPath);
            ServerStatic.SharedGroupsConfig = GameCore.ConfigSharing.Paths[4] is null ? null : new YamlConfig(GameCore.ConfigSharing.Paths[4] + "shared_groups.txt");
            ServerStatic.SharedGroupsMembersConfig = GameCore.ConfigSharing.Paths[5] is null ? null : new YamlConfig(GameCore.ConfigSharing.Paths[5] + "shared_groups_members.txt");
            ServerStatic.PermissionsHandler = new PermissionsHandler(ref ServerStatic.RolesConfig, ref ServerStatic.SharedGroupsConfig, ref ServerStatic.SharedGroupsMembersConfig);
            ServerStatic.GetPermissionsHandler().RefreshPermissions();

            foreach (Player player in Player.List)
            {
                player.ReferenceHub.serverRoles.SetGroup(null, false);
                player.ReferenceHub.serverRoles.RefreshPermissions();
            }
        }
    }
}
