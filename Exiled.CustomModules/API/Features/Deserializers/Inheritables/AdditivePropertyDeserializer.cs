// -----------------------------------------------------------------------
// <copyright file="AdditivePropertyDeserializer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.Deserializers.Inheritables
{
    using System;
    using System.Reflection;

    using Exiled.API.Features.Core.Interfaces;
    using Exiled.CustomModules.API.Features.CustomRoles;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;

    /// <summary>
    /// The deserializer for Role Settings.
    /// </summary>
    public class AdditivePropertyDeserializer : ModuleParser
    {
        /// <inheritdoc />
        public override ParserContext.ModuleDelegate Delegate { get; set; } = Deserialize;

        /// <summary>
        /// The actual deserializer.
        /// </summary>
        /// <param name="ctx">The context.</param>
        /// <param name="value">The output object (if successful).</param>
        /// <returns>A bool stating if it was successful or not.</returns>
        public static bool Deserialize(in ParserContext ctx, out object value)
        {
            IAdditiveProperty additiveProperty = Activator.CreateInstance(ctx.ExpectedType) as IAdditiveProperty;
            ctx.Parser.Consume<MappingStart>();

            while (ctx.Parser.TryConsume(out Scalar scalar))
            {
                PropertyInfo property = typeof(RoleSettings).GetProperty(scalar.Value, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property != null)
                {
                    object propertyValue = ctx.NestedObjectDeserializer(ctx.Parser, property.PropertyType);
                    property.SetValue(additiveProperty, propertyValue);
                }
                else
                {
                    // If the property does not exist, skip the scalar value
                    ctx.Parser.SkipThisAndNestedEvents();
                }
            }

            ctx.Parser.Consume<MappingEnd>();
            value = additiveProperty;
            return true;
        }
    }
}