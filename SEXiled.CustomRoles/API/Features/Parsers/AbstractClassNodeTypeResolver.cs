// -----------------------------------------------------------------------
// <copyright file="AbstractClassNodeTypeResolver.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.CustomRoles.API.Features.Parsers
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    using SEXiled.CustomRoles.API.Features.Interfaces;

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

        /// <inheritdoc />
        public bool Deserialize(IParser reader, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            if (!reader.Accept<MappingStart>(out var mapping))
            {
                value = null;
                return false;
            }

            var supportedTypes = typeDiscriminators.Where(t => t.BaseType == expectedType).ToList();
            if (!supportedTypes.Any())
            {
                if (original.Deserialize(reader, expectedType, nestedObjectDeserializer, out value))
                {
                    Validator.ValidateObject(value, new ValidationContext(value, null, null), true);

                    return true;
                }

                return false;
            }

            var start = reader.Current.Start;
            Type actualType;
            ParsingEventBuffer buffer;
            try
            {
                buffer = new ParsingEventBuffer(ReadNestedMapping(reader));
                actualType = CheckWithDiscriminators(expectedType, supportedTypes, buffer);
            }
            catch (Exception exception)
            {
                throw new YamlException(start, reader.Current.End, "Failed when resolving abstract type", exception);
            }

            buffer.Reset();

            if (original.Deserialize(buffer, actualType, nestedObjectDeserializer, out value))
            {
                Validator.ValidateObject(value, new ValidationContext(value, null, null), true);

                return true;
            }

            return false;
        }

        private static Type CheckWithDiscriminators(Type expectedType, IEnumerable<ITypeDiscriminator> supportedTypes, ParsingEventBuffer buffer)
        {
            foreach (var discriminator in supportedTypes)
            {
                buffer.Reset();
                if (!discriminator.TryResolve(buffer, out var actualType))
                    continue;

                return actualType;
            }

            throw new Exception(
                $"None of the registered type discriminators could supply a child class for {expectedType}");
        }

        private static LinkedList<ParsingEvent> ReadNestedMapping(IParser reader)
        {
            var result = new LinkedList<ParsingEvent>();
            result.AddLast(reader.Consume<MappingStart>());
            var depth = 0;
            do
            {
                var next = reader.Consume<ParsingEvent>();
                depth += next.NestingIncrease;
                result.AddLast(next);
            }
            while (depth >= 0);

            return result;
        }
    }
}
