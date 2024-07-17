// -----------------------------------------------------------------------
// <copyright file="EConfig.cs" company="Exiled Team">
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
    public sealed class EConfig : TypeCastObject<object>
    {
        /// <inheritdoc cref="List"/>
        internal static readonly List<EConfig> ConfigsValue = new();

        private static readonly Dictionary<EConfig, string> Cache = new();
        private static readonly List<EConfig> MainConfigsValue = new();

        private readonly HashSet<EConfig> data = new();

        /// <summary>
        /// Initializes a new instance of the <see cref="EConfig"/> class.
        /// </summary>
        /// <param name="obj">The config object.</param>
        public EConfig(object obj) => Base = obj;

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
            .WithDuplicateKeyChecking()
            .IgnoreFields()
            .IgnoreUnmatchedProperties()
            .Build();

        /// <summary>
        /// Gets a <see cref="IReadOnlyCollection{T}"/> containing all the <see cref="EConfig"/>.
        /// </summary>
        public static IReadOnlyCollection<EConfig> List => ConfigsValue;

        /// <summary>
        /// Gets a <see cref="IEnumerable{T}"/> containing all the subconfigs.
        /// </summary>
        public IEnumerable<EConfig> Subconfigs => data;

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
        /// Gets the absolute path.
        /// </summary>
        public string? AbsolutePath => Folder is not null && Name is not null ? Path.Combine(Paths.Configs, Path.Combine(Folder, Name)) : null;

        /// <summary>
        /// Gets a <see cref="EConfig"/> instance given the specified type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the config to look for.</typeparam>
        /// <param name="generateNew">Whether a new config should be generated, if not found.</param>
        /// <returns>The corresponding <see cref="EConfig"/> instance or <see langword="null"/> if not found.</returns>
        public static EConfig? Get<T>(bool generateNew = false)
            where T : class
        {
            EConfig? config = ConfigsValue.FirstOrDefault(config => config?.Base?.GetType() == typeof(T));

            if (!config && generateNew)
                config = GenerateNew<T>();

            return config;
        }

        /// <summary>
        /// Gets a <see cref="EConfig"/> instance given the specified folder.
        /// </summary>
        /// <param name="folder">The folder of the config to look for.</param>
        /// <returns>The corresponding <see cref="EConfig"/> instance or <see langword="null"/> if not found.</returns>
        public static EConfig Get(string folder) => List.FirstOrDefault(cfg => cfg.Folder == folder) ?? throw new InvalidOperationException();

        /// <summary>
        /// Generates a new config of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the config.</typeparam>
        /// <returns>The generated config.</returns>
        public static EConfig? GenerateNew<T>()
            where T : class
        {
            EConfig? config = Get<T>();
            if (config is not null)
                return config;

            return Load(typeof(T), typeof(T).GetCustomAttribute<ConfigAttribute>());
        }

        /// <summary>
        /// Loads all configs.
        /// </summary>
        public static void LoadAll()
        {
            MainConfigsValue.Clear();
            ConfigsValue.Clear();
            Cache.Clear();

            Assembly.GetCallingAssembly()
                .GetTypes()
                .Where(t => t.IsClass && !t.IsInterface && !t.IsAbstract)
                .ToList()
                .ForEach(t => Load(t));
        }

        /// <summary>
        /// Loads a config from a <see cref="Type"/>.
        /// </summary>
        /// <param name="type">The config type.</param>
        /// <param name="attribute">The config data.</param>
        /// <returns>The <see cref="EConfig"/> object.</returns>
        public static EConfig? Load(Type type, ConfigAttribute? attribute = null)
        {
            try
            {
                attribute ??= type.GetCustomAttribute<ConfigAttribute>();
                if (attribute is null)
                    return null;

                ConstructorInfo? constructor = type.GetConstructor(Type.EmptyTypes);
                object? config = constructor is not null ?
                    constructor.Invoke(null)!
                    : Array.Find(
                        type.GetProperties(BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public),
                        property => property.PropertyType == type)?.GetValue(null)!;

                if (config is null)
                {
                    Log.Error($"{type.FullName} is a valid config, but it cannot be instantiated!" +
                        $"It either doesn't have a public default constructor without any arguments or a static property of the {type.FullName} type!");

                    return null;
                }

                EConfig wrapper = new(config);
                if (string.IsNullOrEmpty(wrapper.Folder))
                {
                    if (string.IsNullOrEmpty(attribute.Folder))
                    {
                        Log.Warn($"The folder of the object of type {config.GetType()} ({wrapper.Name}) has not been set. It's not possible to determine the parent config which it belongs to, hence it won't be read.");

                        return null;
                    }

                    wrapper.Folder = attribute.Folder;
                }

                if (string.IsNullOrEmpty(wrapper.Name))
                {
                    if (string.IsNullOrEmpty(attribute.Name))
                    {
                        wrapper.Name = config.GetType().Name;
                        Log.Warn($"The config's name of the object of type {config.GetType()} has not been set. The object's type name ({config.GetType().Name}) will be used instead.");
                    }
                    else
                    {
                        wrapper.Name = attribute.Name;
                    }
                }

                ConfigsValue.Add(wrapper);
                if (!wrapper.Name!.Contains(".yml"))
                    wrapper.Name += ".yml";

                if (wrapper.Folder is not null)
                {
                    string path = Path.Combine(Paths.Configs, wrapper.Folder);
                    if (attribute.IsParent)
                    {
                        if (!Directory.Exists(Path.Combine(path)))
                            Directory.CreateDirectory(path);

                        Load(wrapper, wrapper.AbsolutePath!);
                        wrapper.data.Add(wrapper);
                        MainConfigsValue.Add(wrapper);

                        Dictionary<EConfig, string> localCache = new(Cache);
                        foreach (KeyValuePair<EConfig, string> elem in localCache)
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
        public static void LoadFromCache(EConfig config)
        {
            if (!Cache.TryGetValue(config, out string path))
                return;

            foreach (EConfig cfg in MainConfigsValue)
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
        public static void Load(EConfig config, string? path = null)
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
        /// Gets the path of the specified data object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data to be read.</typeparam>
        /// <returns>The corresponding data's path or <see langword="null"/> if not found.</returns>
        public static string? GetPath<T>()
            where T : class
        {
            object? config = Get<T>();
            return config is null || config is not EConfig configBase ? null : configBase.AbsolutePath;
        }

        /// <summary>
        /// Reads a data object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">The type of the data to be read.</typeparam>
        /// <returns>The corresponding <typeparamref name="T"/> instance or <see langword="null"/> if not found.</returns>
        public T? Read<T>()
            where T : class
        {
            EConfig? t = Get<T>();

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
            File.WriteAllText(path ?? throw new InvalidOperationException(), Serializer.Serialize(param));
            this.CopyProperties(Deserializer.Deserialize(File.ReadAllText(path), GetType()));
        }
    }
}