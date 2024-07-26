// -----------------------------------------------------------------------
// <copyright file="RoleSettingsDeserializer.cs" company="Exiled Team">
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
    /// The deserializer for Role Settings.
    /// </summary>
    public static class RoleSettingsDeserializer
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
            RoleSettings roleSettings = new RoleSettings();
            parser.Consume<MappingStart>();

            while (parser.TryConsume<Scalar>(out Scalar scalar))
            {
                PropertyInfo property = typeof(RoleSettings).GetProperty(scalar.Value, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
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
    }
}