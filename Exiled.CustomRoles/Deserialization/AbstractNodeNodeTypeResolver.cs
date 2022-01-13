// -----------------------------------------------------------------------
// <copyright file="AbstractNodeNodeTypeResolver.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles.Deserialization
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    ///     Object node deserializer that is capable of resolving subclasses of abstract and interface classes, while using
    ///     inner <see cref="INodeDeserializer" /> to deserialize them.
    /// </summary>
    public class AbstractNodeNodeTypeResolver : INodeDeserializer
    {
        private readonly INodeDeserializer original;
        private readonly ITypeDiscriminator[] typeDiscriminators;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AbstractNodeNodeTypeResolver" /> class.
        /// </summary>
        /// <param name="original">Original object node deserializer to be used after the type has been determined.</param>
        /// <param name="discriminators">One or more <see cref="ITypeDiscriminator" />s to be used in deserialization.</param>
        public AbstractNodeNodeTypeResolver(
            INodeDeserializer original,
            params ITypeDiscriminator[] discriminators) // params but eh
        {
            this.original = original;
            typeDiscriminators = discriminators;
        }

        /// <inheritdoc />
        public bool Deserialize(
            IParser reader,
            Type expectedType,
            Func<IParser, Type, object> nestedObjectDeserializer,
            out object value)
        {
            if (!reader.Accept(out MappingStart _))
            {
                value = null;
                return false;
            }

            var supportedTypes = typeDiscriminators.Where(t => t.BaseType == expectedType).ToList();
            if (!supportedTypes.Any())
                return original.Deserialize(reader, expectedType, nestedObjectDeserializer, out value);

            Mark start = reader.Current.Start;
            Type actualType;
            ParsingEventBuffer buffer;
            try
            {
                buffer = new ParsingEventBuffer(ReadNestedMapping(reader));
                actualType = CheckWithDiscriminators(expectedType, supportedTypes, buffer);
            }
            catch (Exception e)
            {
                throw new YamlException(start, reader.Current.End, "Failed when resolving abstract type", e);
            }

            buffer.Reset();
            return original.Deserialize(buffer, actualType, nestedObjectDeserializer, out value);
        }

        private static Type CheckWithDiscriminators(
            Type expectedType,
            IEnumerable<ITypeDiscriminator> supportedTypes,
            ParsingEventBuffer buffer)
        {
            foreach (ITypeDiscriminator discriminator in supportedTypes)
            {
                buffer.Reset();
                if (!discriminator.TryResolve(buffer, out Type actualType))
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