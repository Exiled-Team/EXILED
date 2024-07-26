// -----------------------------------------------------------------------
// <copyright file="CustomModuleDeserializer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;
    using System.Reflection;

    using Exiled.CustomModules.API.Features.CustomRoles;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <inheritdoc />
    public class CustomModuleDeserializer : INodeDeserializer
    {
        /// <summary>
        /// The default property bindings.
        /// </summary>
        public const BindingFlags DEFAULT_PROPERTY_BINDINGS = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

        /// <inheritdoc />
        public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            if (IsCustomModule(expectedType))
                return DeserializeModule(parser, expectedType, nestedObjectDeserializer, out value);

            if (expectedType.IsAssignableFrom(typeof(RoleSettings)))
                return DeserializeSettings(parser, nestedObjectDeserializer, out value);

            value = null;
            return false;
        }

        /// <summary>
        /// Deserializes the custom module instance.
        /// </summary>
        /// <param name="parser">The yaml parser.</param>
        /// <param name="expectedType">The expected type.</param>
        /// <param name="nestedObjectDeserializer">The base deserializer as backup.</param>
        /// <param name="value">If valid, returns this.</param>
        /// <returns>Whether the deserialization was successful.</returns>
        internal static bool DeserializeModule(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
                value = Activator.CreateInstance(expectedType);
                parser.Consume<MappingStart>();

                while (parser.TryConsume(out Scalar scalar))
                {
                    PropertyInfo property = expectedType.GetProperty(scalar.Value, DEFAULT_PROPERTY_BINDINGS);
                    if (property is not null)
                    {
                        object propertyValue = nestedObjectDeserializer(parser, property.PropertyType);
                        property.SetValue(value, propertyValue);
                    }
                    else if (scalar.Value.Equals("settings", StringComparison.OrdinalIgnoreCase))
                    {
                        PropertyInfo settingsProperty = expectedType.GetProperty("Settings", DEFAULT_PROPERTY_BINDINGS);
                        if (settingsProperty != null)
                        {
                            object settingsValue = nestedObjectDeserializer(parser, settingsProperty.PropertyType);
                            settingsProperty.SetValue(value, settingsValue);
                        }
                    }
                    else
                    {
                        // Skip unknown properties
                        parser.SkipThisAndNestedEvents();
                    }
                }

                parser.Consume<MappingEnd>();
                return true;
        }

        /// <summary>
        /// Deserializes the custom module's settings instance.
        /// </summary>
        /// <param name="parser">The yaml parser.</param>
        /// <param name="nestedObjectDeserializer">The base deserializer as backup.</param>
        /// <param name="value">If valid, returns this.</param>
        /// <returns>Whether the deserialization was successful.</returns>
        internal static bool DeserializeSettings(IParser parser, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            RoleSettings roleSettings = new();
            parser.Consume<MappingStart>();

            while (parser.TryConsume(out Scalar scalar))
            {
                PropertyInfo property = typeof(RoleSettings).GetProperty(scalar.Value, DEFAULT_PROPERTY_BINDINGS);
                if (property != null)
                {
                    object propertyValue = nestedObjectDeserializer(parser, property.PropertyType);
                    property.SetValue(roleSettings, propertyValue);
                }
                else
                {
                    // If the property does not exist, skip the scalar value
                    parser.SkipThisAndNestedEvents();
                }
            }

            parser.Consume<MappingEnd>();
            value = roleSettings;
            return true;
        }

        /// <summary>
        /// Checks whether the given type is a custom module.
        /// </summary>
        /// <param name="type">The given type.</param>
        /// <returns>Whether the given type is a custom module.</returns>
        internal static bool IsCustomModule(Type type)
        {
            while (type is not null)
            {
                if (type.IsAssignableFrom(typeof(CustomModule)))
                    return true;

                type = type.BaseType;
            }

            return false;
        }
    }
}