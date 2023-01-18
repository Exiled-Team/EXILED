// -----------------------------------------------------------------------
// <copyright file="VectorsConverter.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Features.Configs.CustomConverters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using YamlDotNet.Core;
    using YamlDotNet.Core.Events;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Converts a Vector2, Vector3 or Vector4 to Yaml configs and vice versa.
    /// </summary>
    public sealed class VectorsConverter : IYamlTypeConverter
    {
        /// <inheritdoc/>
        public bool Accepts(Type type) => type == typeof(Vector2) || type == typeof(Vector3) || type == typeof(Vector4);

        /// <inheritdoc/>
        public object ReadYaml(IParser parser, Type type)
        {
            if (!parser.TryConsume<MappingStart>(out _))
                throw new InvalidDataException($"Cannot deserialize object of type {type.FullName}.");

            List<object> coordinates = ListPool<object>.Shared.Rent(4);
            int i = 0;

            while (!parser.TryConsume<MappingEnd>(out _))
            {
                if (i++ % 2 == 0)
                {
                    parser.MoveNext();
                    continue;
                }

                if (!parser.TryConsume(out Scalar scalar) || !float.TryParse(scalar.Value, NumberStyles.Float, CultureInfo.GetCultureInfo("en-US"), out float coordinate))
                {
                    ListPool<object>.Shared.Return(coordinates);
                    throw new InvalidDataException($"Invalid float value.");
                }

                coordinates.Add(coordinate);
            }

            object vector = Activator.CreateInstance(type, coordinates.ToArray());

            ListPool<object>.Shared.Return(coordinates);

            return vector;
        }

        /// <inheritdoc/>
        public void WriteYaml(IEmitter emitter, object value, Type type)
        {
            Dictionary<string, float> coordinates = new(4);

            if (value is Vector2 vector2)
            {
                coordinates["x"] = vector2.x;
                coordinates["y"] = vector2.y;
            }
            else if (value is Vector3 vector3)
            {
                coordinates["x"] = vector3.x;
                coordinates["y"] = vector3.y;
                coordinates["z"] = vector3.z;
            }
            else if (value is Vector4 vector4)
            {
                coordinates["x"] = vector4.x;
                coordinates["y"] = vector4.y;
                coordinates["z"] = vector4.z;
                coordinates["w"] = vector4.w;
            }

            emitter.Emit(new MappingStart());

            foreach (KeyValuePair<string, float> coordinate in coordinates)
            {
                emitter.Emit(new Scalar(coordinate.Key));
                emitter.Emit(new Scalar(coordinate.Value.ToString(CultureInfo.GetCultureInfo("en-US"))));
            }

            emitter.Emit(new MappingEnd());
        }
    }
}