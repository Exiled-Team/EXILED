// -----------------------------------------------------------------------
// <copyright file="CustomModule.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Attributes;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.DynamicEvents;
    using Exiled.API.Interfaces;
    using Exiled.CustomModules.API.Enums;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Represents a marker class for custom modules.
    /// </summary>
    public abstract class CustomModule : TypeCastObject<CustomModule>, IEquatable<CustomModule>, IEquatable<uint>
    {
#pragma warning disable SA1310
        /// <summary>
        /// The folder where custom modules are stored.
        /// </summary>
        public const string CUSTOM_MODULES_FOLDER = "CustomModules";
#pragma warning restore SA1310

        private static readonly List<ModuleInfo> Loader = new();

        private string serializableParentPath;
        private string serializableChildPath;
        private string serializableFilePath;
        private string modulePointerPath;
        private string serializableParentName;
        private string serializableChildName;
        private string serializableFileName;
        private string modulePointerName;

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all delegates to be fired when a module gets enabled.
        /// </summary>
        [YamlIgnore]
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<ModuleInfo> OnEnabled { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="TDynamicEventDispatcher{T}"/> which handles all delegates to be fired when a module gets disabled.
        /// </summary>
        [YamlIgnore]
        [DynamicEventDispatcher]
        public static TDynamicEventDispatcher<ModuleInfo> OnDisabled { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="CustomModule"/>'s name.
        /// </summary>
        public abstract string Name { get; set; }

        /// <summary>
        /// Gets or sets or sets the <see cref="CustomModule"/>'s id.
        /// </summary>
        public abstract uint Id { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the <see cref="CustomModule"/> is enabled.
        /// </summary>
        public abstract bool IsEnabled { get; set; }

        /// <summary>
        /// Gets or sets the module's configuration.
        /// </summary>
        [YamlIgnore]
        public abstract ModulePointer Config { get; set; }

        /// <summary>
        /// Gets the name of the serialized parent folder.
        /// </summary>
        [YamlIgnore]
        internal string ParentName
        {
            get
            {
                // Type-Module
                if (!string.IsNullOrEmpty(serializableParentName))
                    serializableParentName = $"{GetType().Name}-Module";

                return serializableParentName;
            }
        }

        /// <summary>
        /// Gets the name of the child folder.
        /// </summary>
        [YamlIgnore]
        internal string ChildName
        {
            get
            {
                // Name
                if (string.IsNullOrEmpty(serializableChildName))
                    serializableChildName = Name;

                return serializableChildName;
            }
        }

        /// <summary>
        /// Gets the name of the serialized child file.
        /// </summary>
        [YamlIgnore]
        internal string FileName
        {
            get
            {
                // Name.yml
                if (string.IsNullOrEmpty(serializableFileName))
                    serializableFileName = $"{Name}.yml";

                return serializableFileName;
            }
        }

        /// <summary>
        /// Gets the name of the serialized module pointer.
        /// </summary>
        [YamlIgnore]
        internal string PointerName
        {
            get
            {
                // Name-Ptr.yml
                if (string.IsNullOrEmpty(modulePointerName))
                    modulePointerName = $"{Name}-Ptr.yml";

                return modulePointerName;
            }
        }

        /// <summary>
        /// Gets the path to the parent folder.
        /// </summary>
        [YamlIgnore]
        internal string ParentPath
        {
            get
            {
                // /Configs/CustomModules/ParentName/
                if (string.IsNullOrEmpty(serializableParentPath))
                    serializableParentPath = Path.Combine(Paths.Configs, CUSTOM_MODULES_FOLDER, ParentName);

                return serializableParentPath;
            }
        }

        /// <summary>
        /// Gets the path to the serialized module child folder.
        /// </summary>
        [YamlIgnore]
        internal string ChildPath
        {
            get
            {
                // /Configs/CustomModules/ParentName/ChildName/
                if (string.IsNullOrEmpty(serializableChildPath))
                    serializableChildPath = Path.Combine(Paths.Configs, CUSTOM_MODULES_FOLDER, ParentName, ChildName);

                return serializableChildPath;
            }
        }

        /// <summary>
        /// Gets the path to the serialized module file.
        /// </summary>
        [YamlIgnore]
        internal string FilePath
        {
            get
            {
                // /Configs/CustomModules/ParentName/ChildName/FileName
                if (string.IsNullOrEmpty(serializableFilePath))
                    serializableFilePath = Path.Combine(Paths.Configs, CUSTOM_MODULES_FOLDER, ParentName, ChildName, FileName);

                return serializableFilePath;
            }
        }

        /// <summary>
        /// Gets the path to the serialized module pointer.
        /// </summary>
        [YamlIgnore]
        internal string PointerPath
        {
            get
            {
                // /Configs/CustomModules/ParentName/ChildName/PointerName
                if (string.IsNullOrEmpty(modulePointerPath))
                    modulePointerPath = Path.Combine(Paths.Configs, CUSTOM_MODULES_FOLDER, ParentName, ChildName, PointerName);

                return modulePointerPath;
            }
        }

        /// <summary>
        /// Compares two operands: <see cref="CustomModule"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="CustomModule"/> to compare.</param>
        /// <param name="right">The <see cref="object"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomModule left, object right) => left?.Equals(right) ?? right is null;

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomModule"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomModule"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator ==(object left, CustomModule right) => right == left;

        /// <summary>
        /// Compares two operands: <see cref="CustomModule"/> and <see cref="object"/>.
        /// </summary>
        /// <param name="left">The <see cref="object"/> to compare.</param>
        /// <param name="right">The <see cref="CustomModule"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomModule left, object right) => !(left == right);

        /// <summary>
        /// Compares two operands: <see cref="object"/> and <see cref="CustomModule"/>.
        /// </summary>
        /// <param name="left">The left <see cref="object"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomModule"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(object left, CustomModule right) => !(left == right);

        /// <summary>
        /// Compares two operands: <see cref="CustomModule"/> and <see cref="CustomModule"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomModule"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomModule"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(CustomModule left, CustomModule right) => left?.Equals(right) ?? right is null;

        /// <summary>
        /// Compares two operands: <see cref="CustomModule"/> and <see cref="CustomModule"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomModule"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomModule"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomModule left, CustomModule right) => left.Id != right.Id;

        /// <summary>
        /// Loads all custom modules from the specified assembly and enables them.
        /// </summary>
        /// <remarks>
        /// This method iterates through all plugins loaded by Exiled, extracts their assemblies, and attempts to load custom modules from each assembly.
        /// It then sets the state of the automatic modules loader to enabled.
        /// </remarks>
        public static void LoadAll()
        {
            foreach (IPlugin<IConfig> plugin in Exiled.Loader.Loader.Plugins)
                Load(plugin.Assembly);

            AutomaticModulesLoaderState(true);
        }

        /// <summary>
        /// Unloads all currently loaded custom modules and sets the automatic modules loader state to disabled.
        /// </summary>
        public static void UnloadAll() => AutomaticModulesLoaderState(false);

        /// <summary>
        /// Loads custom modules from the specified assembly and optionally enables them.
        /// </summary>
        /// <param name="assembly">The assembly from which to load custom modules. If <see langword="null"/>, defaults to the calling assembly.</param>
        /// <param name="shouldBeEnabled">Determines whether the loaded modules should be enabled after loading.</param>
        public static void Load(Assembly assembly = null, bool shouldBeEnabled = false)
        {
            assembly ??= Assembly.GetCallingAssembly();

            UUModuleType FindClosestModuleType(Type t, IEnumerable<FieldInfo> source)
            {
                List<int> matches = new();
                matches.AddRange(source.Select(f => f.Name.LevenshteinDistance(t.Name)));
                return source.ElementAt(matches.IndexOf(matches.Min())).GetValue(null) as UUModuleType;
            }

            Type runtime_ModuleType = assembly.GetTypes().FirstOrDefault(t => !t.IsAbstract && typeof(UUModuleType).IsAssignableFrom(t));
            if (runtime_ModuleType is null)
            {
                Log.Debug("No UUModuleType-derived types were found. Custom modules must have an identifier based on UUModuleType.");
                return;
            }

            IEnumerable<FieldInfo> moduleTypeValuesInfo = runtime_ModuleType.GetFields(BindingFlags.Static | BindingFlags.Public).Where(f => f.GetValue(null) is UUModuleType);
            foreach (Type type in assembly.GetTypes())
            {
                if (type.BaseType != typeof(CustomModule) || Loader.Any(m => m.Type == type))
                    continue;

                IEnumerable<MethodInfo> rhMethods = type.GetMethods(ModuleInfo.SIGNATURE_BINDINGS)
                    .Where(m =>
                    {
                        ParameterInfo[] mParams = m.GetParameters();
                        return (m.Name is ModuleInfo.ENABLE_ALL_CALLBACK && mParams.Any(p => p.ParameterType == typeof(Assembly))) || m.Name is ModuleInfo.DISABLE_ALL_CALLBACK;
                    });

                MethodInfo enableAll = rhMethods.FirstOrDefault(m => m.Name is ModuleInfo.ENABLE_ALL_CALLBACK);
                MethodInfo disableAll = rhMethods.FirstOrDefault(m => m.Name is ModuleInfo.DISABLE_ALL_CALLBACK);

                if (enableAll is null)
                {
                    Log.Warn("Unable to locate the callback responsible for enabling module instances.");
                    continue;
                }

                if (disableAll is null)
                {
                    Log.Warn("Unable to locate the callback responsible for disabling module instances.");
                    continue;
                }

                Action<Assembly> enableAllAction = Delegate.CreateDelegate(typeof(Action<Assembly>), enableAll) as Action<Assembly>;
                Action disableAllAction = Delegate.CreateDelegate(typeof(Action), disableAll) as Action;

                ModuleInfo moduleInfo = new()
                {
                    Type = type,
                    EnableAll_Callback = enableAllAction,
                    DisableAll_Callback = disableAllAction,
                    IsCurrentlyLoaded = false,
                    ModuleType = FindClosestModuleType(type, moduleTypeValuesInfo),
                };

                ModuleInfo.AllModules.Add(moduleInfo);

                if (!shouldBeEnabled)
                    continue;

                moduleInfo.InvokeCallback(ModuleInfo.ENABLE_ALL_CALLBACK, assembly);
            }
        }

        /// <summary>
        /// Unloads custom modules from the specified assembly.
        /// </summary>
        /// <param name="assembly">The assembly from which to unload custom modules. If <see langword="null"/>, defaults to the calling assembly.</param>
        public static void Unload(Assembly assembly = null)
        {
            assembly ??= Assembly.GetCallingAssembly();

            foreach (Type type in assembly.GetTypes())
            {
                ModuleInfo moduleInfo = ModuleInfo.Get(type);
                if (moduleInfo.Type is null)
                    continue;

                moduleInfo.InvokeCallback(ModuleInfo.DISABLE_ALL_CALLBACK, assembly);
            }
        }

        /// <summary>
        /// Determines whether the provided id is equal to the current object.
        /// </summary>
        /// <param name="id">The id to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(uint id) => Id == id;

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="cr">The custom module to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public bool Equals(CustomModule cr) => cr && cr.GetType().IsAssignableFrom(GetType()) && (ReferenceEquals(this, cr) || Id == cr.Id);

        /// <summary>
        /// Determines whether the specified object is equal to the current object.
        /// </summary>
        /// <param name="obj">The object to compare.</param>
        /// <returns><see langword="true"/> if the object was equal; otherwise, <see langword="false"/>.</returns>
        public override bool Equals(object obj)
        {
            if (obj is CustomModule other)
                return Equals(other);

            try
            {
                return Equals((uint)obj);
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary>
        /// Returns a the 32-bit signed hash code of the current object instance.
        /// </summary>
        /// <returns>The 32-bit signed hash code of the current object instance.</returns>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Serializes the current module to a file specified by <see cref="ChildPath"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="ChildPath"/> is null.</exception>
        public void SerializeModule()
        {
            Directory.CreateDirectory(ParentPath);

            if (File.Exists(FilePath) && File.Exists(PointerPath))
            {
                File.WriteAllText(FilePath, EConfig.Serializer.Serialize(this));
                File.WriteAllText(PointerPath, EConfig.Serializer.Serialize(Config));
                return;
            }

            File.WriteAllText(FilePath ?? throw new ArgumentNullException(nameof(FilePath)), EConfig.Serializer.Serialize(this));
            File.WriteAllText(PointerPath ?? throw new ArgumentNullException(nameof(PointerPath)), EConfig.Serializer.Serialize(Config));
        }

        /// <summary>
        /// Deserializes the module from the file specified by <see cref="ChildPath"/>.
        /// </summary>
        public void DeserializeModule()
        {
            if (!File.Exists(FilePath))
            {
                Log.Info($"{GetType().Name} module configuration not found. Creating a new configuration file.");

                if (File.Exists(PointerPath))
                {
                    try
                    {
                        Config = EConfig.Deserializer.Deserialize<ModulePointer>(File.ReadAllText(PointerPath));
                    }
                    catch
                    {
                        Config = ModulePointer.Get(this);
                    }
                }
                else
                {
                    Config = ModulePointer.Get(this);
                }

                SerializeModule();
                return;
            }

            CustomModule deserializedModule = EConfig.Deserializer.Deserialize<CustomModule>(File.ReadAllText(FilePath));
            CopyProperties(deserializedModule);

            foreach (string file in Directory.GetFiles(ChildPath))
            {
                if (file == FilePath)
                    continue;

                try
                {
                    Config = EConfig.Deserializer.Deserialize<ModulePointer>(File.ReadAllText(file));
                }
                catch
                {
                    continue;
                }
            }
        }

        private static void AutomaticModulesLoaderState(bool shouldLoad)
        {
            Config config = CustomModules.Instance.Config;
            if (config.UseAutomaticModulesLoader)
            {
                foreach (IPlugin<IConfig> plugin in Exiled.Loader.Loader.Plugins)
                {
                    ModuleInfo.AllModules
                        .Where(moduleInfo => config.Modules.Any(m => m == moduleInfo.ModuleType.Name))
                        .ForEach(mod => mod.InvokeCallback(shouldLoad ? ModuleInfo.ENABLE_ALL_CALLBACK : ModuleInfo.DISABLE_ALL_CALLBACK, plugin.Assembly));
                }
            }
        }

        private void CopyProperties(CustomModule source)
        {
            foreach (PropertyInfo property in GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance))
            {
                if (property.CanWrite)
                    property.SetValue(this, property.GetValue(source));
            }
        }
    }
}