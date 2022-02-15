// -----------------------------------------------------------------------
// <copyright file="AttachmentIdentifiersConverter.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Loader.Features.Configs.CustomConverters
{
    using System;
    using System.Collections.Generic;
    using System.IO;

    using SEXiled.API.Structs;

    using InventorySystem.Items.Firearms.Attachments;

    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Converts a <see cref="IEnumerable{T}"/> of <see cref="AttachmentNameTranslation"/> to Yaml configs and vice versa.
    /// </summary>
    public sealed class AttachmentIdentifiersConverter : IYamlTypeConverter
    {
        /// <inheritdoc/>
        public bool Accepts(Type type) => type == typeof(AttachmentNameTranslation);

        /// <inheritdoc/>
        public object ReadYaml(IParser parser, Type type)
        {
            if (!parser.TryConsume(out Scalar scalar) || !AttachmentIdentifier.TryParse(scalar.Value, out AttachmentNameTranslation name))
                throw new InvalidDataException($"Invalid AttachmentNameTranslation value: {scalar.Value}.");

            return Enum.Parse(type, name.ToString());
        }

        /// <inheritdoc/>
        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            AttachmentNameTranslation name = default;

            if (value is AttachmentNameTranslation locAttachment)
                name = locAttachment;

            emitter.Emit(new Scalar(name.ToString()));
        }
    }
}
