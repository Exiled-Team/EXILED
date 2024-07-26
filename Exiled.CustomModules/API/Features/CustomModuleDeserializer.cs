// -----------------------------------------------------------------------
// <copyright file="CustomModuleDeserializer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;

    using Exiled.CustomModules.API.Features.CustomRoles;
    using Exiled.CustomModules.API.Features.Deserializers;
    using YamlDotNet.Core;
    using YamlDotNet.Serialization;

    /// <inheritdoc />
    public class CustomModuleDeserializer : INodeDeserializer
    {
        /// <inheritdoc />
        public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            if (CustomRoleDeserializer.IsCustomRoleType(expectedType))
            {
                return CustomRoleDeserializer.Deserialize(parser, expectedType, nestedObjectDeserializer, out value);
            }

            if (expectedType == typeof(RoleSettings))
            {
                return RoleSettingsDeserializer.Deserialize(parser, expectedType, nestedObjectDeserializer, out value);
            }

            value = null;
            return false;
        }
    }
}