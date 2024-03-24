// -----------------------------------------------------------------------
// <copyright file="ValidatingNodeDeserializer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Serialization
{
    using System;
    using System.ComponentModel.DataAnnotations;

    using Exiled.API.Features;

    using YamlDotNet.Core;
    using YamlDotNet.Serialization;

    /// <summary>
    /// Basic configs validation.
    /// </summary>
    public sealed class ValidatingNodeDeserializer : INodeDeserializer
    {
        private readonly INodeDeserializer nodeDeserializer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidatingNodeDeserializer"/> class.
        /// </summary>
        /// <param name="nodeDeserializer">The node deserializer instance.</param>
        public ValidatingNodeDeserializer(INodeDeserializer nodeDeserializer)
        {
            this.nodeDeserializer = nodeDeserializer;
        }

        /// <inheritdoc cref="INodeDeserializer"/>
        public bool Deserialize(IParser parser, Type expectedType, Func<IParser, Type, object> nestedObjectDeserializer, out object value)
        {
            try
            {
                if (nodeDeserializer.Deserialize(parser, expectedType, nestedObjectDeserializer, out value))
                {
                    if (value is null)
                        Log.Error("Null value");
                    Validator.ValidateObject(value, new ValidationContext(value, null, null), true);

                    return true;
                }

                return false;
            }
            catch (Exception e)
            {
                Log.Error(e);
                value = null;
                return false;
            }
        }
    }
}