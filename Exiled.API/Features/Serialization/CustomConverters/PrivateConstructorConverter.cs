// -----------------------------------------------------------------------
// <copyright file="PrivateConstructorConverter.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Serialization.CustomConverters
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;

    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A YAML type converter for types with private constructors.
    /// This converter handles deserialization by invoking a private parameterless constructor
    /// and serialization by writing only non-default property values.
    /// </summary>
    public class PrivateConstructorConverter : IYamlTypeConverter
    {
        /// <summary>
        /// Determines whether this converter can handle the specified type.
        /// </summary>
        /// <param name="type">The type to check.</param>
        /// <returns><see langword="true"/> if the type has a private parameterless constructor; otherwise, <see langword="false"/>.</returns>
        public bool Accepts(Type type)
        {
            return type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null) != null;
        }

        /// <summary>
        /// Reads YAML data and creates an instance of the specified type.
        /// Uses reflection to invoke a private parameterless constructor to create the instance.
        /// </summary>
        /// <param name="parser">The YAML parser used to read the data.</param>
        /// <param name="type">The type of the object to create.</param>
        /// <returns>A new instance of the specified type with properties set from the YAML data.</returns>
        /// <exception cref="InvalidOperationException">Thrown if no private parameterless constructor is found.</exception>
        public object ReadYaml(IParser parser, Type type)
        {
            Dictionary<string, object> yamlData = new();
            parser.Consume<MappingStart>();
            while (parser.Current is not MappingEnd)
            {
                if (parser.TryConsume(out Scalar keyScalar) && parser.TryConsume(out Scalar valueScalar))
                    yamlData[keyScalar.Value] = valueScalar.Value;
            }

            parser.Consume<MappingEnd>();

            ConstructorInfo constructor = type.GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, Type.EmptyTypes, null);
            if (constructor == null)
                throw new InvalidOperationException("No private constructor found");

            object instance = constructor.Invoke(null);
            ApplyYamlData(instance, yamlData);
            return instance;
        }

        /// <summary>
        /// Writes an object to YAML.
        /// Serializes only the properties that have values different from their default values.
        /// </summary>
        /// <param name="emitter">The YAML emitter used to write the data.</param>
        /// <param name="value">The object to serialize.</param>
        /// <param name="type">The type of the object to serialize.</param>
        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            emitter.Emit(new MappingStart());

            foreach (PropertyInfo property in properties)
            {
                object propertyValue = property.GetValue(value);
                object defaultValue = GetDefaultValue(property.PropertyType);

                if (!Equals(propertyValue, defaultValue))
                {
                    emitter.Emit(new Scalar(property.Name));
                    emitter.Emit(new Scalar(propertyValue?.ToString() ?? string.Empty));
                }
            }

            emitter.Emit(new MappingEnd());
        }

        /// <summary>
        /// Applies YAML data to an object by setting its properties.
        /// </summary>
        /// <param name="instance">The instance of the object to update.</param>
        /// <param name="yamlData">The dictionary containing YAML data.</param>
        private void ApplyYamlData(object instance, Dictionary<string, object> yamlData)
        {
            Type type = instance.GetType();
            foreach (KeyValuePair<string, object> kvp in yamlData)
            {
                PropertyInfo property = type.GetProperty(kvp.Key, BindingFlags.Public | BindingFlags.Instance);
                if (property != null)
                {
                    object convertedValue = Convert.ChangeType(kvp.Value, property.PropertyType);
                    property.SetValue(instance, convertedValue);
                }
            }
        }

        /// <summary>
        /// Gets the default value for a type.
        /// </summary>
        /// <param name="type">The type to get the default value for.</param>
        /// <returns>The default value for the type, or <see langword="null"/> for reference types.</returns>
        private object GetDefaultValue(Type type) => type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}
