// -----------------------------------------------------------------------
// <copyright file="AbstractClassNodeTypeResolver.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.API.Features.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using Exiled.CustomRoles.API.Features.Interfaces;

    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A node resolver for <see cref="CustomAbility"/>.
    /// </summary>
    public class AbstractClassNodeTypeResolver : INodeDeserializer
    {
        private readonly INodeDeserializer original;
        private readonly ITypeDiscriminator[] typeDiscriminators;

        /// <summary>
        /// Initializes a new instance of the <see cref="AbstractClassNodeTypeResolver"/> class.
        /// </summary>
        /// <param name="original">The <see cref="INodeDeserializer"/> original deserializer.</param>
        /// <param name="discriminators">The <see cref="ITypeDiscriminator"/> array of discriminators.</param>
        public AbstractClassNodeTypeResolver(INodeDeserializer original, params ITypeDiscriminator[] discriminators)
        {
            this.original = original;
            typeDiscriminators = discriminators;
        }

        /// <inheritdoc cref="INodeDeserializer"/>
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object?> nestedObjectDeserializer, out object? value)
        {
            if (!reader.Accept<MappingStart>(out MappingStart? mapping))
            {
                value = null;
                return false;
            }

            IEnumerable<ITypeDiscriminator> supportedTypes = typeDiscriminators.Where(t => t.BaseType == expectedType).ToArray();
            if (!supportedTypes.Any())
            {
                if (original.Deserialize(reader, expectedType, nestedObjectDeserializer, out value))
                {
                    Validator.ValidateObject(value!, new ValidationContext(value!, null, null), true);

                    return true;
                }

                return false;
            }

            Mark? start = reader.Current?.Start;
            Type? actualType;
            ParsingEventBuffer buffer;
            try
            {
                buffer = new ParsingEventBuffer(ReadNestedMapping(reader));
                actualType = CheckWithDiscriminators(expectedType, supportedTypes, buffer);
            }
            catch (Exception exception)
            {
                throw new YamlException(start ?? new(), reader.Current?.End ?? new(), "Failed when resolving abstract type", exception);
            }

            buffer.Reset();

            if (original.Deserialize(buffer, actualType!, nestedObjectDeserializer, out value))
            {
                Validator.ValidateObject(value!, new ValidationContext(value!, null, null), true);

                return true;
            }

            return false;
        }

        private static Type? CheckWithDiscriminators(Type expectedType, IEnumerable<ITypeDiscriminator> supportedTypes, ParsingEventBuffer buffer)
        {
            foreach (ITypeDiscriminator discriminator in supportedTypes)
            {
                buffer.Reset();
                if (!discriminator.TryResolve(buffer, out Type? actualType))
                    continue;

                return actualType;
            }

            throw new Exception(
                $"None of the registered type discriminators could supply a child class for {expectedType}");
        }

        private static LinkedList<ParsingEvent> ReadNestedMapping(IParser reader)
        {
            LinkedList<ParsingEvent> result = new();
            result.AddLast(reader.Consume<MappingStart>());
            int depth = 0;
            do
            {
                ParsingEvent next = reader.Consume<ParsingEvent>();
                depth += next.NestingIncrease;
                result.AddLast(next);
            }
            while (depth >= 0);

            return result;
        }
    }
}