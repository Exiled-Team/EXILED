// -----------------------------------------------------------------------
// <copyright file="ConfigSubsystem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
#nullable enable
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Extensions;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Serialization;
    using Exiled.API.Features.Serialization.CustomConverters;
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NodeDeserializers;

    using ColorConverter = Serialization.CustomConverters.ColorConverter;
    using UnderscoredNamingConvention = Serialization.UnderscoredNamingConvention;

    /// <summary>
    /// The base class that handles the config subsystem.
    /// </summary>
    public sealed class ConfigSubsystem : TypeCastObject<object>
    {
        /// <inheritdoc cref="List"/>
        internal static readonly List<ConfigSubsystem> ConfigsValue = new();

        private static readonly Dictionary<ConfigSubsystem, string> Cache = new();
        private static readonly List<ConfigSubsystem> MainConfigsValue = new();
        private static bool isLoaded = false;

        private readonly HashSet<ConfigSubsystem> data = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="ConfigSubsystem"/> class.
        /// </summary>
        /// <param name="obj">The config object.</param>
        public ConfigSubsystem(object obj) => Base = obj;

        /// <summary>
        /// Gets or sets the serializer for configs and translations.
        /// </summary>
        public static ISerializer Serializer { get; set; } = GetDefaultSerializerBuilder().Build();

        /// <summary>
        /// Gets or sets the deserializer for configs and translations.
        /// </summary>
        public static IDeserializer Deserializer { get; set; } = GetDefaultDeserializerBuilder().Build();

        /// <summary>
        /// Gets or sets the JSON compatible serializer.
        /// </summary>
        public static ISerializer JsonSerializer { get; set; } = new SerializerBuilder()
            .JsonCompatible()
            .Build();

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> containing all the <see cref="ConfigSubsystem"/>.
        /// </summary>
        public static IReadOnlyCollection<ConfigSubsystem> List => ConfigsValue;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> containing all the subconfigs.
        /// </summary>
        public IEnumerable<ConfigSubsystem> Subconfigs => data;

        /// <summary>
        /// Gets the base config instance.
        /// </summary>
        public object? Base { get; private set; }

        /// <summary>
        /// Gets or sets the config's folder.
        /// </summary>
        public string? Folder { get; set; }

        /// <summary>
        /// Gets or sets the config's name.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the config does not rely on any path.
        /// </summary>
        public bool IsStandAlone { get; set; }

        /// <summary>
        /// Gets or sets the individual path.
        /// </summary>
        public string? StandAlonePath { get; set; }

        /// <summary>
        /// Gets the absolute path.
        /// </summary>
        public string? AbsolutePath => Folder is not null && Name is not null ? Path.Combine(Paths.Configs, Path.Combine(Folder, Name)) : null;

        /// <summary>
        /// Gets the default serializer builder.
        /// </summary>
        /// <returns>The default serializer builder.</returns>
        public static SerializerBuilder GetDefaultSerializerBuilder() => new SerializerBuilder()
            .WithTypeConverter(new VectorsConverter())
            .WithTypeConverter(new ColorConverter())
            .WithTypeConverter(new AttachmentIdentifiersConverter())
            .WithTypeConverter(new EnumClassConverter())
            .WithTypeConverter(new PrivateConstructorConverter())
            .WithEventEmitter(eventEmitter => new TypeAssigningEventEmitter(eventEmitter))
            .WithTypeInspector(inner => new CommentGatheringTypeInspector(inner))
            .WithEmissionPhaseObjectGraphVisitor(args => new CommentsObjectGraphVisitor(args.InnerVisitor))
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .DisableAliases();

        /// <summary>
        /// Gets the default deserializer builder.
        /// </summary>
        /// <returns>The default deserializer builder.</returns>
        public static DeserializerBuilder GetDefaultDeserializerBuilder() => new DeserializerBuilder()
            .WithTypeConverter(new VectorsConverter())
            .WithTypeConverter(new ColorConverter())
            .WithTypeConverter(new AttachmentIdentifiersConverter())
            .WithTypeConverter(new EnumClassConverter())
            .WithTypeConverter(new PrivateConstructorConverter())
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .WithNodeDeserializer(inner => new ValidatingNodeDeserializer(inner), deserializer => deserializer.InsteadOf<ObjectNodeDeserializer>())
            .WithDuplicateKeyChecking()
            .IgnoreFields()
            .IgnoreUnmatchedProperties();

        /// <summary>
        /// Gets a <see cref="ConfigSubsystem"/> instance given the specified folder.
        /// </summary>
        /// <param name="folder">The folder of the config to look for.</param>
        /// <returns>The corresponding <see cref="ConfigSubsystem"/> instance or <see langword="null"/> if not found.</returns>
        public static ConfigSubsystem Get(string folder) => List.FirstOrDefault(cfg => cfg.Folder == folder) ?? throw new InvalidOperationException();

        /// <summary>
        /// Gets a <see cref="ConfigSubsystem"/> instance given the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the config to look for.</param>
        /// <param name="generateNew">Whether a new config should be generated, if not found.</param>
        /// <returns>The corresponding <see cref="ConfigSubsystem"/> instance or <see langword="null"/> if not found.</returns>
        public static ConfigSubsystem? Get(Type type, bool generateNew = false)
        {
            ConfigSubsystem? config = ConfigsValue.FirstOrDefault(config => config?.Base?.GetType() == type);

            if (config == null && generateNew)
                config = GenerateNew(type);

            return config;
        }

        /// <summary>
        /// Gets a <see cref="ConfigSubsystem"/> instance given the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the config to look for.</typeparam>
        /// <param name="generateNew">Whether a new config should be generated, if not found.</param>
        /// <returns>The corresponding <see cref="ConfigSubsystem"/> instance or <see langword="null"/> if not found.</returns>
        public static ConfigSubsystem? Get<T>(bool generateNew = false)
            where T : class => Get(typeof(T), generateNew);

        /// <summary>
        /// Generates a new config for the specified <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The <see cref="Type"/> of the config.</param>
        /// <returns>The generated config.</returns>
        public static ConfigSubsystem? GenerateNew(Type type) => Load(type, type.GetCustomAttribute<ConfigAttribute>());

        /// <summary>
        /// Generates a new config of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the config.</typeparam>
        /// <returns>The generated config.</returns>
        public static ConfigSubsystem? GenerateNew<T>()
            where T : class => GenerateNew(typeof(T));

        /// <summary>
        /// Loads all configs.
        /// </summary>
        /// <param name="toLoad">The assemblies to load the configs from.</param>
        public static void LoadAll(IEnumerable<Assembly> toLoad)
        {
            if (!isLoaded)
            {
                isLoaded = true;
                MainConfigsValue.Clear();
                ConfigsValue.Clear();
                Cache.Clear();
            }

            void LoadFromAssembly(Assembly asm) =>
                asm.GetTypes()
                    .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract)
                    .ToList()
                    .ForEach(t =>
                    {
                        ConfigAttribute attribute = t.GetCustomAttribute<ConfigAttribute>();
                        if (attribute is null)
                            return;

                        Load(t, attribute);
                    });

            if (toLoad is not null && !toLoad.IsEmpty())
            {
                toLoad.ForEach(LoadFromAssembly);
                return;
            }

            LoadFromAssembly(Assembly.GetCallingAssembly());
        }

        /// <summary>
        /// Loads a config from a <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The config type.</param>
        /// <param name="attribute">The config data.</param>
        /// <returns>The <see cref="ConfigSubsystem"/> object.</returns>
        public static ConfigSubsystem? Load(Type type, ConfigAttribute? attribute)
        {
            try
            {
                attribute ??= type.GetCustomAttribute<ConfigAttribute>();
                if (attribute is null)
                    return null;

                ConstructorInfo? constructor = type.GetConstructor(Type.EmptyTypes);
                object? target = constructor is not null ?
                    constructor.Invoke(null)!
                    : Array.Find(
                        type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public),
                        property => property.PropertyType == type)?.GetValue(null)!;

                if (target is null)
                {
                    Log.Error($"{type.FullName} is a valid config, but it cannot be instantiated!" +
                        $"It either doesn't have a public default constructor without any arguments or a static property of the {type.FullName} type!");

                    return null;
                }

                ConfigSubsystem wrapper = new(target);

                if (!wrapper.IsStandAlone)
                {
                    if (string.IsNullOrEmpty(wrapper.Folder))
                    {
                        if (string.IsNullOrEmpty(attribute.Folder))
                        {
                            Log.Warn($"The folder of the object of type {target.GetType()} ({wrapper.Name}) has not been set. It's not possible to determine the parent config which it belongs to, hence it won't be read.");

                            return null;
                        }

                        wrapper.Folder = attribute.Folder;
                    }
                }

                if (string.IsNullOrEmpty(wrapper.Name))
                {
                    if (string.IsNullOrEmpty(attribute.Name))
                    {
                        wrapper.Name = target.GetType().Name;
                        Log.Warn($"The config's name of the object of type {target.GetType()} has not been set. The object's type name ({target.GetType().Name}) will be used instead.");
                    }
                    else
                    {
                        wrapper.Name = attribute.Name;
                    }
                }

                ConfigsValue.Add(wrapper);
                if (!wrapper.Name!.Contains(".yml"))
                    wrapper.Name += ".yml";

                if (wrapper.IsStandAlone && !string.IsNullOrEmpty(wrapper.StandAlonePath))
                {
                    Load(wrapper, wrapper.StandAlonePath);
                    wrapper.data.Add(wrapper);
                    MainConfigsValue.Add(wrapper);

                    return wrapper;
                }

                if (wrapper.Folder is not null)
                {
                    string path = Path.Combine(Paths.Configs, wrapper.Folder);
                    if (attribute.IsParent)
                    {
                        Directory.CreateDirectory(path);

                        Load(wrapper, wrapper.AbsolutePath!);
                        wrapper.data.Add(wrapper);
                        MainConfigsValue.Add(wrapper);

                        Dictionary<ConfigSubsystem, string> localCache = new(Cache);
                        foreach (KeyValuePair<ConfigSubsystem, string> elem in localCache)
                            LoadFromCache(elem.Key);

                        return wrapper;
                    }

                    Cache.Add(wrapper, wrapper.AbsolutePath!);
                    if (!Directory.Exists(path) || MainConfigsValue.All(cfg => cfg.Folder != wrapper.Folder))
                        return wrapper;
                }

                LoadFromCache(wrapper);

                if (!ConfigsValue.Contains(wrapper))
                    ConfigsValue.Add(wrapper);

                return wrapper;
            }
            catch (ReflectionTypeLoadException reflectionTypeLoadException)
            {
                Log.Error($"Error while initializing config {Assembly.GetCallingAssembly().GetName().Name} (at {Assembly.GetCallingAssembly().Location})! {reflectionTypeLoadException}");

                foreach (Exception? loaderException in reflectionTypeLoadException.LoaderExceptions)
                {
                    Log.Error(loaderException);
                }
            }
            catch (Exception exception)
            {
                Log.Error($"Error while initializing config {Assembly.GetCallingAssembly().GetName().Name} (at {Assembly.GetCallingAssembly().Location})! {exception}");
            }

            return null;
        }

        /// <summary>
        /// Loads a config from the cached configs.
        /// </summary>
        /// <param name="config">The config to load.</param>
        public static void LoadFromCache(ConfigSubsystem config)
        {
            if (!Cache.TryGetValue(config, out string path))
                return;

            foreach (ConfigSubsystem cfg in MainConfigsValue)
            {
                if (string.IsNullOrEmpty(cfg.Folder) || cfg.Folder != config.Folder)
                    continue;

                cfg.data.Add(config);
            }

            Load(config, path);
            Cache.Remove(config);
        }

        /// <summary>
        /// Loads a config.
        /// </summary>
        /// <param name="config">The config to load.</param>
        /// <param name="path">The config's path.</param>
        public static void Load(ConfigSubsystem config, string? path = null)
        {
            path ??= config.AbsolutePath;
            if (File.Exists(path))
            {
                config.Base = Deserializer.Deserialize(File.ReadAllText(path ?? throw new ArgumentNullException(nameof(path))), config.Base!.GetType())!;
                File.WriteAllText(path, Serializer.Serialize(config.Base!));
                return;
            }

            File.WriteAllText(path ?? throw new ArgumentNullException(nameof(path)), Serializer.Serialize(config.Base!));
        }

        /// <summary>
        /// Loads a dynamic configuration for the specified <see cref="Type"/> from the given path.
        /// This method generates a new type at runtime that mirrors the <paramref name="sourceType"/>,
        /// including all serializable properties, and applies attributes like <see cref="ConfigAttribute"/>
        /// and <see cref="YamlIgnoreAttribute"/> if specified.
        /// </summary>
        /// <param name="sourceType">The <see cref="Type"/> for which to generate the dynamic configuration at runtime.</param>
        /// <param name="path">The path from which to load the configuration.</param>
        /// <returns>A <see cref="ConfigSubsystem"/> instance for the dynamically generated configuration, or <see langword="null"/> if not found.</returns>
        public static ConfigSubsystem? LoadDynamic(Type sourceType, string path) =>

            // TODO: Implement support for specifying a path and file for the dynamic type.
            // TODO: Additionally, add a method to initialize the ConfigAttribute so that the dynamic configuration can function independently.
            Get(
                sourceType.GenerateDynamicTypeWithConstructorFromExistingType(
                    sourceType.Name + "Config", new[] { typeof(ConfigAttribute) }, true, new[] { typeof(YamlIgnoreAttribute) }), true);

        /// <summary>
        /// Gets the path of the specified data object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data to be read.</typeparam>
        /// <returns>The corresponding data's path or <see langword="null"/> if not found.</returns>
        public static string? GetPath<T>()
            where T : class
        {
            object? config = Get<T>();
            return config is null || config is not ConfigSubsystem configBase ? null : configBase.AbsolutePath;
        }

        /// <inheritdoc cref="Serializer.Serialize(object?)"/>
        public static string Serialize(object? graph) => Serializer.Serialize(graph);

        /// <inheritdoc cref="Serializer.Serialize(object?, Type)"/>
        public static string Serialize(object? graph, Type type) => Serializer.Serialize(graph, type);

        /// <inheritdoc cref="Deserializer.Deserialize(string)"/>
        public static object? Deserialize(string input) => Deserializer.Deserialize(input);

        /// <inheritdoc cref="Deserializer.Deserialize(string, Type)"/>
        public static object? Deserialize(string input, Type type) => Deserializer.Deserialize(input, type);

        /// <inheritdoc cref="Deserializer.Deserialize{T}(string)"/>
        public static T Deserialize<T>(string input) => Deserializer.Deserialize<T>(input);

        /// <summary>
        /// Converts a dictionary of key-value pairs into a YAML formatted string.
        /// </summary>
        /// <param name="dictionary">The dictionary containing keys and values to be converted to YAML format.</param>
        /// <returns>A YAML formatted string representing the dictionary contents.</returns>
        /// <remarks>
        /// The keys in the dictionary must be of type <see cref="string"/>, and the values can be of any object type.
        /// This method ensures compatibility with JSON format before converting to YAML.
        /// </remarks>
        public static string ConvertDictionaryToYaml(Dictionary<string, object> dictionary)
        {
            Dictionary<string, object> convertedDictionary = dictionary.ToDictionary(
                kvp => kvp.Key.ToString(),
                kvp => kvp.Value);

            return JsonSerializer.Serialize(convertedDictionary);
        }

        /// <summary>
        /// Reads a data object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data to be read.</typeparam>
        /// <returns>The corresponding <typeparamref name="T"/> instance or <see langword="null"/> if not found.</returns>
        public T? Read<T>()
            where T : class
        {
            ConfigSubsystem? t = Get<T>();

            if (t is null)
                return default;

            return t.data.FirstOrDefault(data => data.Base!.GetType() == typeof(T)) as T ?? default;
        }

        /// <summary>
        /// Reads the base data object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data to be read.</typeparam>
        /// <returns>The corresponding <typeparamref name="T"/> instance or <see langword="null"/> if not found.</returns>
        public T? BaseAs<T>() => Base is not T t ? default : t;

        /// <summary>
        /// Writes a new value contained in the specified config of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data to be written.</typeparam>
        /// <param name="name">The name of the parameter to be modified.</param>
        /// <param name="value">The new value to be written.</param>
        public void Write<T>(string name, object value)
            where T : class
        {
            T? param = Read<T>();
            if (param is null)
                return;

            string? path = GetPath<T>();
            PropertyInfo? propertyInfo = param.GetType().GetProperty(name);

            if (propertyInfo is null)
                return;

            propertyInfo.SetValue(param, value);
            File.WriteAllText(path ?? throw new ArgumentNullException(), Serializer.Serialize(param));
            this.CopyProperties(Deserializer.Deserialize(File.ReadAllText(path), GetType()));
        }
    }
}