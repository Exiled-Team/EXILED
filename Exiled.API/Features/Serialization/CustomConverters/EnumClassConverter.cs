// -----------------------------------------------------------------------
// <copyright file="EnumClassConverter.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Serialization.CustomConverters
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using Exiled.API.Features.Core.Generic;
    using Exiled.API.Interfaces;
    using YamlDotNet.Core;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Converts instances of <see cref="EnumClass{TSource,TObject}"/> to YAML configurations and vice versa.
    /// This class implements the <see cref="IYamlTypeConverter"/> interface to handle the serialization and deserialization
    /// of enum classes that use private constructors for their instantiation.
    /// </summary>
    public class EnumClassConverter : IYamlTypeConverter
    {
        /// <summary>
        /// Determines whether this converter can handle the specified type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><see langword="true"/> if the type is assignable to <see cref="IEnumClass"/>; otherwise, <see langword="false"/>.</returns>
        public bool Accepts(Type type) => typeof(IEnumClass).IsAssignableFrom(type);

        /// <summary>
        /// Reads YAML data and creates an instance of the specified type.
        /// Uses reflection to invoke a private parameterless constructor to create the instance.
        /// </summary>
        /// <param name="parser">The YAML parser used to read the data.</param>
        /// <param name="type">The type of the object to create.</param>
        /// <returns>A new instance of the specified type.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no private parameterless constructor is found.</exception>
        public object ReadYaml(IParser parser, Type type)
        {
            // Read YAML data into a dictionary or custom object
            IDeserializer deserializer = new DeserializerBuilder().Build();
            Dictionary<string, object> yamlData = deserializer.Deserialize<Dictionary<string, object>>(parser);

            // Create an instance using the private constructor
            ConstructorInfo constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            if (constructor == null)
                throw new InvalidOperationException("No private constructor found");

            object instance = constructor.Invoke(null);

            // Apply YAML data to the instance
            ApplyYamlData(instance, yamlData);

            return instance;
        }

        /// <summary>
        /// Writes an object to YAML.
        /// This method is currently not implemented.
        /// </summary>
        /// <param name="emitter">The YAML emitter used to write the data.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="type">The type of the object to serialize.</param>
        public void WriteYaml(IEmitter emitter, object value, Type type) => EConfig.Serializer.Serialize(emitter, value);

        private void ApplyYamlData(object instance, Dictionary<string, object> yamlData)
        {
            // Get the type of the instance
            Type type = instance.GetType();

            // Iterate over each property in the dictionary
            foreach (KeyValuePair<string, object> kvp in yamlData)
            {
                // Find the property by name
                PropertyInfo property = type.GetProperty(kvp.Key, BindingFlags.Public | BindingFlags.Instance);

                // If the property exists and is writable, set its value
                if (property != null && property.CanWrite)
                {
                    // Convert the value to the property's type
                    object value = Convert.ChangeType(kvp.Value, property.PropertyType);
                    property.SetValue(instance, value);
                }
            }
        }
    }
}