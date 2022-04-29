// -----------------------------------------------------------------------
// <copyright file="AggregateExpectationTypeResolver.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Exiled.API.Features;
    using Exiled.CustomRoles.API.Features;
    using Exiled.CustomRoles.API.Features.Extensions;
    using Exiled.CustomRoles.API.Features.Interfaces;

    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <inheritdoc />
    public class AggregateExpectationTypeResolver<T> : ITypeDiscriminator
        where T : class
    {
        private const string TargetKey = nameof(CustomAbility.AbilityType);
        private readonly string targetKey;
        private readonly Dictionary<string, Type> typeLookup;

        /// <summary>
        /// Initializes a new instance of the <see cref="AggregateExpectationTypeResolver{T}"/> class.
        /// </summary>
        /// <param name="namingConvention">The <see cref="INamingConvention"/> instance.</param>
        public AggregateExpectationTypeResolver(INamingConvention namingConvention)
        {
            targetKey = namingConvention.Apply(TargetKey);
            typeLookup = new Dictionary<string, Type>();
            foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                try
                {
                    foreach (Type t in assembly.GetTypes().Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T))))
                        typeLookup.Add(t.Name, t);
                }
                catch (Exception e)
                {
                    Log.Error($"Error loading types for {assembly.FullName}\n{e}");
                }
            }
        }

        /// <inheritdoc />
        public Type BaseType => typeof(CustomAbility);

        /// <inheritdoc />
        public bool TryResolve(ParsingEventBuffer buffer, out Type suggestedType)
        {
            if (buffer.TryFindMappingEntry(
                scalar => targetKey == scalar.Value,
                out Scalar key,
                out ParsingEvent value))
            {
                if (value is Scalar valueScalar)
                {
                    suggestedType = CheckName(valueScalar.Value);

                    return true;
                }

                FailEmpty();
            }

            suggestedType = null;
            return false;
        }

        private void FailEmpty()
        {
            throw new Exception($"Could not determine expectation type, {targetKey} has an empty value");
        }

        private Type CheckName(string value)
        {
            if (typeLookup.TryGetValue(value, out Type childType))
                return childType;

            string known = string.Join(",", typeLookup.Keys);
            throw new Exception(
                $"Could not match `{targetKey}: {value}` to a known expectation. Expecting one of: {known}");
        }
    }
}
