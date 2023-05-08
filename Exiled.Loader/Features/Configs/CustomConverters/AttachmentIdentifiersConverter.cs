// -----------------------------------------------------------------------
// <copyright file="AttachmentIdentifiersConverter.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Features.Configs.CustomConverters
{
    extern alias Yaml;

    using System;
    using System.Collections.Generic;
    using System.IO;

    using API.Structs;

    using InventorySystem.Items.Firearms.Attachments;

    using Yaml::YamlDotNet.Core.Events;
    using Yaml::YamlDotNet.Serialization;

    using IEmitter = Yaml::YamlDotNet.Core.IEmitter;
    using IParser = Yaml::YamlDotNet.Core.IParser;

    /// <summary>
    /// Converts a <see cref="IEnumerable{T}"/> of <see cref="AttachmentName"/> to Yaml configs and vice versa.
    /// </summary>
    public sealed class AttachmentIdentifiersConverter : IYamlTypeConverter
    {
        /// <inheritdoc cref="IYamlTypeConverter" />
        public bool Accepts(Type type) => type == typeof(AttachmentName);

        /// <inheritdoc cref="IYamlTypeConverter" />
        public object ReadYaml(IParser parser, Type type)
        {
            if (!Yaml::YamlDotNet.Core.ParserExtensions.TryConsume(parser, out Scalar scalar) || !AttachmentIdentifier.TryParse(scalar.Value, out AttachmentName name))
                throw new InvalidDataException($"Invalid AttachmentNameTranslation value: {scalar?.Value}.");

            return Enum.Parse(type, name.ToString());
        }

        /// <inheritdoc cref="IYamlTypeConverter" />
        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            AttachmentName name = default;

            if (value is AttachmentName locAttachment)
                name = locAttachment;

            emitter.Emit(new Scalar(name.ToString()));
        }
    }
}