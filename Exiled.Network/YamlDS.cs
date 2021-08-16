// -----------------------------------------------------------------------
// <copyright file="YamlDS.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Network
{
    using YamlDotNet.Serialization;
    using YamlDotNet.Serialization.NamingConventions;

    /// <summary>
    /// Yaml deserializer and serializer.
    /// </summary>
    public class YamlDS
    {
        /// <summary>
        /// Gets the serializer for configs.
        /// </summary>
        public static ISerializer Serializer { get; } = new SerializerBuilder()
            .WithNamingConvention(UnderscoredNamingConvention.Instance)
            .IgnoreFields()
            .Build();

        /// <summary>
        /// Gets the deserializer for configs.
        /// </summary>
        public static IDeserializer Deserializer { get; } = new DeserializerBuilder()
            .IgnoreFields()
            .IgnoreUnmatchedProperties()
            .Build();
    }
}
