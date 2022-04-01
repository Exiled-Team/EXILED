// -----------------------------------------------------------------------
// <copyright file="TranslationManager.cs" company="Exiled Team">
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
    /// Used to handle plugin translations.
    /// </summary>
    public static class TranslationManager {
        /// <summary>
        /// Loads all plugin translations.
        /// </summary>
        /// <param name="rawTranslations">The raw translations to be loaded.</param>
        /// <returns>Returns a dictionary of loaded translations.</returns>
        public static SortedDictionary<string, ITranslation> Load(string rawTranslations) {
            try {
                Log.Info("Loading plugin translations...");

                Dictionary<string, object> rawDeserializedTranslations = Loader.Deserializer.Deserialize<Dictionary<string, object>>(rawTranslations) ?? new Dictionary<string, object>();
                SortedDictionary<string, ITranslation> deserializedTranslations = new SortedDictionary<string, ITranslation>(StringComparer.Ordinal);

                foreach (IPlugin<IConfig> plugin in Loader.Plugins) {
                    if (plugin.InternalTranslation == null)
                        continue;

                    if (!rawDeserializedTranslations.TryGetValue(plugin.Prefix, out object rawDeserializedTranslation)) {
                        Log.Warn($"{plugin.Name} doesn't have default translations, generating...");

                        deserializedTranslations.Add(plugin.Prefix, plugin.InternalTranslation);
                    }
                    else {
                        try {
                            deserializedTranslations.Add(plugin.Prefix, (ITranslation)Loader.Deserializer.Deserialize(Loader.Serializer.Serialize(rawDeserializedTranslation), plugin.InternalTranslation.GetType()));

                            plugin.InternalTranslation.CopyProperties(deserializedTranslations[plugin.Prefix]);
                        }
                        catch (YamlException yamlException) {
                            Log.Error($"{plugin.Name} translations could not be loaded, some of them are in a wrong format, default translations will be loaded instead!\n{yamlException}");

                            deserializedTranslations.Add(plugin.Prefix, plugin.InternalTranslation);
                        }
                    }
                }

                // Make sure that no keys in the config file were discarded.
                if (!rawDeserializedTranslations.Keys.All(deserializedTranslations.ContainsKey)) {
                    Log.Warn("Missing plugins have been detected in the translations. A backup translations file will be created at \"" + Paths.BackupTranslations + "\".");
                    File.WriteAllText(Paths.BackupTranslations, rawTranslations);
                }

                Log.Info("Plugin translations loaded successfully!");

                return deserializedTranslations;
            }
            catch (Exception exception) {
                Log.Error($"An error has occurred while loading translations!\n{exception}");

                return null;
            }
        }

        /// <summary>
        /// Reads, Loads and Saves plugin translations.
        /// </summary>
        /// <returns>Returns a value indicating if the reloading process has been completed successfully or not.</returns>
        public static bool Reload() => Save(Load(Read()));

        /// <summary>
        /// Saves plugin translations.
        /// </summary>
        /// <param name="translations">The translations to be saved, already serialized in yaml format.</param>
        /// <returns>Returns a value indicating whether the translations have been saved successfully or not.</returns>
        public static bool Save(string translations) {
            try {
                File.WriteAllText(Paths.Translations, translations ?? string.Empty);

                return true;
            }
            catch (Exception exception) {
                Log.Error($"An error has occurred while saving translations to {Paths.Translations} path:\n{exception}");

                return false;
            }
        }

        /// <summary>
        /// Saves plugin translations.
        /// </summary>
        /// <param name="translations">The translations to be saved.</param>
        /// <returns>Returns a value indicating whether the translations have been saved successfully or not.</returns>
        public static bool Save(SortedDictionary<string, ITranslation> translations) {
            try {
                if (translations == null || translations.Count == 0)
                    return false;

                return Save(Loader.Serializer.Serialize(translations));
            }
            catch (YamlException yamlException) {
                Log.Error($"An error has occurred while serializing translations:\n{yamlException}");

                return false;
            }
        }

        /// <summary>
        /// Read all plugin translations.
        /// </summary>
        /// <returns>Returns the read translations.</returns>
        public static string Read() {
            try {
                if (File.Exists(Paths.Translations))
                    return File.ReadAllText(Paths.Translations);
            }
            catch (Exception exception) {
                Log.Error($"An error has occurred while reading translations from {Paths.Translations} path:\n{exception}");
            }

            return string.Empty;
        }
    }
}
