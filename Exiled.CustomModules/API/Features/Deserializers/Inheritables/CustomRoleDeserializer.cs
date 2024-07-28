// -----------------------------------------------------------------------
// <copyright file="CustomRoleDeserializer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.Deserializers.Inheritables
{
    using System;
    using System.Reflection;

    using Exiled.CustomModules.API.Features.CustomRoles;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;

    /// <summary>
    /// The deserializer for Custom Roles.
    /// </summary>
    public class CustomRoleDeserializer : ModuleParser
    {
        /// <inheritdoc />
        public override ParserContext.ModuleDelegate Delegate { get; set; } = Deserialize;

        /// <summary>
        /// The actual deserializer.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="value">If valid, returns this.</param>
        /// <returns>A bool stating if it was successful or not.</returns>
        public static bool Deserialize(in ParserContext ctx, out object value)
        {
            if (IsCustomRoleType(ctx.ExpectedType))
            {
                value = Activator.CreateInstance(ctx.ExpectedType);
                ctx.Parser.Consume<MappingStart>();

                while (ctx.Parser.TryConsume<Scalar>(out Scalar scalar))
                {
                    PropertyInfo property = ctx.ExpectedType.GetProperty(scalar.Value, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    if (property != null)
                    {
                        object propertyValue = ctx.NestedObjectDeserializer(ctx.Parser, property.PropertyType);
                        property.SetValue(value, propertyValue);
                    }
                    else if (scalar.Value.Equals("settings", StringComparison.OrdinalIgnoreCase))
                    {
                        PropertyInfo settingsProperty = ctx.ExpectedType.GetProperty("Settings", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        if (settingsProperty != null)
                        {
                            object settingsValue = ctx.NestedObjectDeserializer(ctx.Parser, settingsProperty.PropertyType);
                            settingsProperty.SetValue(value, settingsValue);
                        }
                    }
                    else
                    {
                        // Skip unknown properties
                        ctx.Parser.SkipThisAndNestedEvents();
                    }
                }

                ctx.Parser.Consume<MappingEnd>();
                return true;
            }

            value = null;
            return false;
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