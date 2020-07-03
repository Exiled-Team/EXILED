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
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Loader.Features.Configs;
    using YamlDotNet.Core;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;
    using YamlDotNet.Serialization.NodeDeserializers;

    /// <summary>
    /// Used to handle plugin configs.
    /// </summary>
    public static class ConfigManager
    {
        /// <summary>
        /// Gets the serializer builder instance.
        /// </summary>
        internal static SerializerBuilder SerializerBuilder { get; } = new SerializerBuilder()
            .WithTypeInspector(inner => new CommentGatheringTypeInspector(inner))
            .WithEmissionPhaseObjectGraphVisitor(args => new CommentsObjectGraphVisitor(args.InnerVisitor))
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithTagMapping("!Dictionary[string,IConfig]", typeof(Dictionary<string, IConfig>))
            .WithTagMapping("!Exiled.Loader.Config", typeof(Config))
            .IgnoreFields()
            .EnsureRoundtrip();

        /// <summary>
        /// Gets the deserializer builder instance.
        /// </summary>
        internal static DeserializerBuilder DeserializerBuilder { get; } = new DeserializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), deserializer => deserializer.InsteadOf<ObjectNodeDeserializer>())
            .WithTagMapping("!Dictionary[string,IConfig]", typeof(Dictionary<string, IConfig>))
            .WithTagMapping("!Exiled.Loader.Config", typeof(Config))
            .IgnoreFields()
            .IgnoreUnmatchedProperties();

        /// <summary>
        /// Gets the config serializer.
        /// </summary>
        private static ISerializer Serializer => SerializerBuilder.Build();

        /// <summary>
        /// Gets the config serializer.
        /// </summary>
        private static IDeserializer Deserializer => DeserializerBuilder.Build();

        /// <summary>
        /// Loads all plugin configs.
        /// </summary>
        /// <param name="rawConfigs">The raw configs to be loaded.</param>
        /// <returns>Returns a dictionary of loaded configs.</returns>
        public static Dictionary<string, IConfig> Load(string rawConfigs)
        {
            try
            {
                Log.Info("Loading plugin configs...");

                Dictionary<string, IConfig> deserializedConfigs = Deserializer.Deserialize<Dictionary<string, IConfig>>(rawConfigs) ?? new Dictionary<string, IConfig>();

                if (!deserializedConfigs.TryGetValue("exiled_loader", out IConfig deserializedConfig))
                {
                    Log.Warn($"Exiled.Loader doesn't have default configs, generating...");

                    deserializedConfigs.Add("exiled_loader", Loader.Config);
                }
                else
                {
                    Loader.Config.CopyProperties(deserializedConfigs["exiled_loader"]);
                }

                foreach (IPlugin<IConfig> plugin in Loader.Plugins)
                {
                    if (!deserializedConfigs.TryGetValue(plugin.Prefix, out deserializedConfig))
                    {
                        Log.Warn($"{plugin.Name} doesn't have default configs, generating...");

                        deserializedConfigs.Add(plugin.Prefix, plugin.Config);
                    }
                    else
                    {
                        plugin.Config.CopyProperties(deserializedConfig);
                    }
                }

                Log.Info("Plugin configs loaded successfully!");

                return deserializedConfigs;
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while loading configs, default configs will be loaded instead! {exception}");

                return null;
            }
        }

        /// <summary>
        /// Reads, Loads and Saves plugin configs.
        /// </summary>
        /// <returns>Returns a value indicating if the reloading process has been completed successfully or not.</returns>
        public static bool Reload() => Save(Load(Read()));

        /// <summary>
        /// Saves plugin configs.
        /// </summary>
        /// <param name="configs">The configs to be saved, already serialized in yaml format.</param>
        /// <returns>Returns a value indicating whether the configs have been saved successfully or not.</returns>
        public static bool Save(string configs)
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
        /// Saves plugin configs.
        /// </summary>
        /// <param name="configs">The configs to be saved.</param>
        /// <returns>Returns a value indicating whether the configs have been saved successfully or not.</returns>
        public static bool Save(Dictionary<string, IConfig> configs)
        {
            try
            {
                if (configs == null || configs.Count == 0)
                    return false;

                return Save(Serializer.Serialize(configs));
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
        }

        /// <summary>
        /// Adds a new config type tag to the serializer and deserializer.
        /// </summary>
        /// <param name="obj">The object to get the tag from.</param>
        public static void AddTag(object obj) => AddTag(obj.GetType());

        /// <summary>
        /// Adds a new config type tag to the serializer and deserializer.
        /// </summary>
        /// <param name="type">The tag type.</param>
        public static void AddTag(Type type)
        {
            try
            {
                SerializerBuilder.WithTagMapping($"!{type.FullName}", type);
                DeserializerBuilder.WithTagMapping($"!{type.FullName}", type);
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while trying to add a tag to the serializer/deserializer: {exception}");
            }
        }
    }
}
