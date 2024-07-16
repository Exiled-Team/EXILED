// -----------------------------------------------------------------------
// <copyright file="CustomModule.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;
    using System.IO;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.API.Features.Core;
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

        private string modulePath;

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
        /// Gets the module's configuration.
        /// </summary>
        [YamlIgnore]
        public abstract object Config { get; }

        /// <summary>
        /// Gets the name of the serialized module.
        /// </summary>
        [YamlIgnore]
        internal string SerializableModuleName => $"{GetType().Name}-Module";

        /// <summary>
        /// Gets the name of the serialized module file.
        /// </summary>
        internal string SerializableModuleFile => $"{Name}.yml";

        /// <summary>
        /// Gets the path to the serialized module file.
        /// </summary>
        [YamlIgnore]
        internal string SerializableModulePath
        {
            get
            {
                if (string.IsNullOrEmpty(modulePath))
                    modulePath = Path.Combine(Path.Combine(Path.Combine(Paths.Configs, CUSTOM_MODULES_FOLDER), SerializableModuleName), SerializableModuleFile);

                return modulePath;
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
        public static bool operator ==(CustomModule left, CustomModule right) => left is null ? right is null : left.Equals(right);

        /// <summary>
        /// Compares two operands: <see cref="CustomModule"/> and <see cref="CustomModule"/>.
        /// </summary>
        /// <param name="left">The left <see cref="CustomModule"/> to compare.</param>
        /// <param name="right">The right <see cref="CustomModule"/> to compare.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(CustomModule left, CustomModule right) => !(left.Id == right.Id);

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
        /// Serializes the current module to a file specified by <see cref="SerializableModulePath"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException">Thrown when <see cref="SerializableModulePath"/> is null.</exception>
        public void SerializeModule()
        {
            Directory.CreateDirectory(CUSTOM_MODULES_FOLDER);

            if (File.Exists(SerializableModulePath))
            {
                File.WriteAllText(SerializableModulePath, EConfig.Serializer.Serialize(this));
                return;
            }

            File.WriteAllText(SerializableModulePath ?? throw new ArgumentNullException(nameof(SerializableModulePath)), EConfig.Serializer.Serialize(this));
        }

        /// <summary>
        /// Deserializes the module from the file specified by <see cref="SerializableModulePath"/>.
        /// </summary>
        public void DeserializeModule()
        {
            if (!File.Exists(SerializableModulePath))
            {
                Log.Info($"{GetType().Name} module configuration not found. Creating a new configuration file.");
                SerializeModule();
                return;
            }

            CustomModule deserializedModule = EConfig.Deserializer.Deserialize<CustomModule>(File.ReadAllText(SerializableModulePath));
            CopyProperties(deserializedModule);
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