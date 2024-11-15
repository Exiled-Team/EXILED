// -----------------------------------------------------------------------
// <copyright file="DynamicTypeConverter.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Serialization.CustomConverters
{
    using System;

    using Exiled.API.Features;
    using YamlDotNet.Core;
    using YamlDotNet.Serialization;

    /// <summary>
    /// A type converter that dynamically handles the (de)serialization process for a specified type.
    /// This converter allows custom (de)serialization logic based on the type's compatibility with the YAML deserializer.
    /// </summary>
    /// <typeparam name="T">The type that this converter will handle during the (de)serialization process.</typeparam>
    public class DynamicTypeConverter<T> : IYamlTypeConverter
        where T : class
    {
        private readonly bool shouldDeserialize;

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicTypeConverter{T}"/> class.
        /// </summary>
        /// <param name="inShouldDeserialize">Whether the entry should be deserialized.</param>
        public DynamicTypeConverter(bool inShouldDeserialize = true)
        {
            shouldDeserialize = inShouldDeserialize;
        }

        /// <inheritdoc/>
        public bool Accepts(Type type)
        {
            return typeof(T).IsAssignableFrom(type);
        }

        /// <inheritdoc/>
        public object ReadYaml(IParser parser, Type type)
        {
            if (shouldDeserialize)
            {
                return ConfigSubsystem.Deserializer.Deserialize(parser, type);
            }
            else
            {
                parser.SkipThisAndNestedEvents();
                return null;
            }
        }

        /// <inheritdoc/>
        public void WriteYaml(IEmitter emitter, object value, Type type) => ConfigSubsystem.Serializer.Serialize(emitter, value, type);
    }
}