// -----------------------------------------------------------------------
// <copyright file="TranslationManager.cs" company="Exiled Team">
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

    using API.Enums;
    using API.Extensions;
    using API.Interfaces;

    using Exiled.API.Features;
    using Exiled.API.Features.Core.Generic.Pools;

    using YamlDotNet.Core;

    using Serialization = API.Features.EConfig;

    /// <summary>
    /// Used to handle plugin translations.
    /// </summary>
    public static class TranslationManager
    {
        /// <summary>
        /// Loads all of the plugin's translations.
        /// </summary>
        /// <param name="rawTranslations">The raw translations to be loaded.</param>
        /// <returns>Returns a dictionary of loaded translations.</returns>
        public static SortedDictionary<string, ITranslation> Load(string rawTranslations)
        {
            try
            {
                Log.Info($"Loading plugin translations... ({LoaderPlugin.Config.ConfigType})");

                Dictionary<string, object> rawDeserializedTranslations = Serialization.Deserializer.Deserialize<Dictionary<string, object>>(rawTranslations) ?? DictionaryPool<string, object>.Pool.Get();
                SortedDictionary<string, ITranslation> deserializedTranslations = new(StringComparer.Ordinal);

                foreach (IPlugin<IConfig> plugin in Loader.Plugins)
                {
                    if (plugin.InternalTranslation is null)
                        continue;

                    deserializedTranslations.Add(plugin.Prefix, plugin.LoadTranslation(rawDeserializedTranslations));
                }

                // Make sure that no keys in the translation file were discarded. (Individual can ignore this since rawDeserializedTranslations is null)
                if (!rawDeserializedTranslations.Keys.All(deserializedTranslations.ContainsKey))
                {
                    Log.Warn("Missing plugins have been detected in the translations. A backup translations file will be created at \"" + Paths.BackupTranslations + "\".");
                    File.WriteAllText(Paths.BackupTranslations, rawTranslations);
                }

                Log.Info("Plugin translations loaded successfully!");

                DictionaryPool<string, object>.Pool.Return(rawDeserializedTranslations);
                return deserializedTranslations;
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while loading translations!\n{exception}");

                return null;
            }
        }

        /// <summary>
        /// Loads the translations of a plugin based on the actual distribution.
        /// </summary>
        /// <param name="plugin">The plugin which its translation has to be loaded.</param>
        /// <param name="rawTranslations">The raw translations to check whether or not the plugin already has a translation config.</param>
        /// <returns>The <see cref="ITranslation"/> of the desired plugin.</returns>
        public static ITranslation LoadTranslation(this IPlugin<IConfig> plugin, Dictionary<string, object> rawTranslations = null) => LoaderPlugin.Config.ConfigType switch
        {
            ConfigType.Separated => plugin.LoadSeparatedTranslation(),
            _ => plugin.LoadDefaultTranslation(rawTranslations),
        };

        /// <summary>
        /// Reads, loads, and saves plugin translations.
        /// </summary>
        /// <returns>Returns a value indicating if the reloading process has been completed successfully or not.</returns>
        public static bool Reload() => Save(Load(Read()));

        /// <summary>
        /// Saves default distribution translations.
        /// </summary>
        /// <param name="translations">The translations to be saved, already serialized in yaml format.</param>
        /// <returns>Returns a value indicating whether the translations have been saved successfully or not.</returns>
        public static bool SaveDefaultTranslation(string translations)
        {
            try
            {
                File.WriteAllText(Paths.Translations, translations ?? string.Empty);

                return true;
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while saving translations to {Paths.Translations} path:\n{exception}");

                return false;
            }
        }

        /// <summary>
        /// Saves plugin translations based on the separated distribution.
        /// </summary>
        /// <param name="pluginPrefix">The prefix of the plugin which its translation is going to be saved.</param>
        /// <param name="translations">The translations to be saved, already serialized in yaml format.</param>
        /// <returns>Returns a value indicating whether the translations have been saved successfully or not.</returns>
        public static bool SaveSeparatedTranslation(this string pluginPrefix, string translations)
        {
            string translationsPath = Paths.GetTranslationPath(pluginPrefix);

            try
            {
                Directory.CreateDirectory(Path.Combine(Paths.IndividualTranslations, pluginPrefix));
                File.WriteAllText(translationsPath, translations ?? string.Empty);

                return true;
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while saving translations to {translationsPath} path: {exception}");

                return false;
            }
        }

        /// <summary>
        /// Saves plugin translations.
        /// </summary>
        /// <param name="translations">The translations to be saved.</param>
        /// <returns>Returns a value indicating whether the translations have been saved successfully or not.</returns>
        public static bool Save(SortedDictionary<string, ITranslation> translations)
        {
            try
            {
                if (translations is null || translations.Count == 0)
                    return false;

                if (LoaderPlugin.Config.ConfigType == ConfigType.Merged)
                {
                    return SaveDefaultTranslation(Serialization.Serializer.Serialize(translations));
                }

                return translations.All(plugin => SaveSeparatedTranslation(plugin.Key, Serialization.Serializer.Serialize(plugin.Value)));
            }
            catch (YamlException yamlException)
            {
                Log.Error($"An error has occurred while serializing translations:\n{yamlException}");

                return false;
            }
        }

        /// <summary>
        /// Read all plugin translations.
        /// </summary>
        /// <returns>Returns the read translations.</returns>
        public static string Read()
        {
            if (LoaderPlugin.Config.ConfigType != ConfigType.Merged)
                return string.Empty;

            try
            {
                if (File.Exists(Paths.Translations))
                    return File.ReadAllText(Paths.Translations);
            }
            catch (Exception exception)
            {
                Log.Error($"An error has occurred while reading translations from {Paths.Translations} path:\n{exception}");
            }

            return string.Empty;
        }

        /// <summary>
        /// Clears the translations.
        /// </summary>
        /// <returns>Returns a value indicating whether translations have been cleared successfully or not.</returns>
        public static bool Clear()
        {
            try
            {
                if (LoaderPlugin.Config.ConfigType == ConfigType.Merged)
                {
                    SaveDefaultTranslation(string.Empty);
                    return true;
                }

                return Loader.Plugins.All(plugin => SaveSeparatedTranslation(plugin.Prefix, string.Empty));
            }
            catch (Exception e)
            {
                Log.Error("An error has occurred while clearing translations:\n" + e);
                return false;
            }
        }

        /// <summary>
        /// Loads the translations of a plugin based on the default distribution.
        /// </summary>
        /// <param name="plugin">The plugin which its translation has to be loaded.</param>
        /// <param name="rawTranslations">The raw translations to check whether or not the plugin already has a translation config.</param>
        /// <returns>The <see cref="ITranslation"/> of the desired plugin.</returns>
        private static ITranslation LoadDefaultTranslation(this IPlugin<IConfig> plugin, Dictionary<string, object> rawTranslations)
        {
            if (rawTranslations is null)
            {
                rawTranslations = Serialization.Deserializer.Deserialize<Dictionary<string, object>>(Read()) ?? DictionaryPool<string, object>.Pool.Get();
            }

            if (!rawTranslations.TryGetValue(plugin.Prefix, out object rawDeserializedTranslation))
            {
                Log.Warn($"{plugin.Name} doesn't have default translations, generating...");
                return plugin.InternalTranslation;
            }

            ITranslation translation;

            try
            {
                translation = (ITranslation)Serialization.Deserializer.Deserialize(Serialization.Serializer.Serialize(rawDeserializedTranslation), plugin.InternalTranslation.GetType());
                plugin.InternalTranslation.CopyProperties(translation);
            }
            catch (YamlException yamlException)
            {
                Log.Error($"{plugin.Name} translations could not be loaded, some of them are in a wrong format, default translations will be loaded instead!\n{yamlException}");
                translation = plugin.InternalTranslation;
            }

            DictionaryPool<string, object>.Pool.Return(rawTranslations);
            return translation;
        }

        /// <summary>
        /// Loads the translations of a plugin based in the separated distribution.
        /// </summary>
        /// <param name="plugin">The plugin which its translations will be loaded.</param>
        /// <returns>The translation of the desired plugin.</returns>
        private static ITranslation LoadSeparatedTranslation(this IPlugin<IConfig> plugin)
        {
            if (!File.Exists(plugin.TranslationPath))
            {
                Log.Warn($"{plugin.Name} doesn't have default translations, generating...");
                return plugin.InternalTranslation;
            }

            ITranslation translation;

            try
            {
                translation = (ITranslation)Serialization.Deserializer.Deserialize(File.ReadAllText(plugin.TranslationPath), plugin.InternalTranslation.GetType());
                plugin.InternalTranslation.CopyProperties(translation);
            }
            catch (YamlException yamlException)
            {
                Log.Error($"{plugin.Name} translations could not be loaded, some of them are in a wrong format, default translations will be loaded instead!\n{yamlException}");
                translation = plugin.InternalTranslation;
            }

            return translation;
        }
    }
}