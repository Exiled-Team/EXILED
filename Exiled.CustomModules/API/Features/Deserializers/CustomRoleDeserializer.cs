// -----------------------------------------------------------------------
// <copyright file="CustomRoleDeserializer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.Deserializers
{
    using System;
    using System.Reflection;

    using Exiled.CustomModules.API.Features.CustomRoles;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;

    /// <summary>
    /// The deserializer for Custom Roles.
    /// </summary>
    public static class CustomRoleDeserializer
    {
        /// <summary>
        /// The actual deserializer.
        /// </summary>
        /// <param name="parser">The Yaml Parser.</param>
        /// <param name="expectedType">The type.</param>
        /// <param name="nestedObjectDeserializer">The base deserializer as backup.</param>
        /// <param name="value">If valid, returns this.</param>
        /// <returns>A bool stating if it was successful or not.</returns>
        public static bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
                value = Activator.CreateInstance(expectedType);
                parser.Consume<MappingStart>();

                while (parser.TryConsume<Scalar>(out Scalar scalar))
                {
                    PropertyInfo property = expectedType.GetProperty(scalar.Value, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (property != null)
                    {
                        object propertyValue = nestedObjectDeserializer(parser, property.PropertyType);
                        property.SetValue(value, propertyValue);
                    }
                    else if (scalar.Value.Equals("settings", StringComparison.OrdinalIgnoreCase))
                    {
                        PropertyInfo settingsProperty = expectedType.GetProperty("Settings", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
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
        /// A function that returns whether a type is a custom role.
        /// </summary>
        /// <param name="type">The Type.</param>
        /// <returns>A bool that says if the type is a custom role.</returns>
        public static bool IsCustomRoleType(Type type)
        {
            while (type != null)
            {
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(CustomRole<>))
                {
                    return true;
                }

                type = type.BaseType;
            }

            return false;
        }
    }
}