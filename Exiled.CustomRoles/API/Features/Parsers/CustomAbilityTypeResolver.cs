// -----------------------------------------------------------------------
// <copyright file="CustomAbilityTypeResolver.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features.Parsers
{
    using System;
    using System.Collections.Generic;

    using Exiled.CustomRoles.API.Features;
    using Exiled.CustomRoles.API.Features.Interfaces;

    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// <see cref="ITypeDiscriminator" /> that is able to resolve subclasses of <see cref="CustomAbility" />.
    /// </summary>
    public class CustomAbilityTypeResolver : ITypeDiscriminator
    {
        private const string TargetKey = nameof(CustomAbility.AbilityType);
        private readonly string targetKey;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CustomAbilityTypeResolver" /> class.
        /// </summary>
        /// <param name="namingConvention">Naming convention to use in deserialization.</param>
        public CustomAbilityTypeResolver(INamingConvention namingConvention)
        {
            targetKey = namingConvention.Apply(TargetKey);
        }

        /// <summary>
        /// Gets a <see cref="Dictionary{TKey,TValue}"/> for storing class names and types used in type resolving.
        /// </summary>
        public Dictionary<string, Type> TypeLookup { get; } = new Dictionary<string, Type>();

        /// <inheritdoc />
        public Type BaseType => typeof(CustomAbility);

        /// <inheritdoc />
        public bool TryResolve(ParsingEventBuffer parser, out Type suggestedType)
        {
            if (parser.TryFindMappingEntry(
                scalar => scalar.Value == targetKey,
                out Scalar _,
                out ParsingEvent value))
            {
                if (value is Scalar valueScalar)
                {
                    suggestedType = CheckName(valueScalar.Value);
                    return true;
                }
            }

            suggestedType = null;
            return false;
        }

        private Type CheckName(string value)
        {
            if (TypeLookup.TryGetValue(value, out Type childType))
                return childType;

            var known = string.Join(",", TypeLookup.Keys);
            throw new Exception(
                $"Could not match `{targetKey}: {value}` to a known expectation. Expecting one of: {known}");
        }
    }
}