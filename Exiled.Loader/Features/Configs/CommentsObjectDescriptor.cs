// -----------------------------------------------------------------------
// <copyright file="CommentsObjectDescriptor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Features.Configs
{
    extern alias Yaml;

    using System;

    using IObjectDescriptor = Yaml::YamlDotNet.Serialization.IObjectDescriptor;
    using ScalarStyle = Yaml::YamlDotNet.Core.ScalarStyle;

    /// <summary>
    /// Source: https://dotnetfiddle.net/8M6iIE.
    /// </summary>
    public sealed class CommentsObjectDescriptor : Yaml::YamlDotNet.Serialization.IObjectDescriptor
    {
        private readonly IObjectDescriptor innerDescriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentsObjectDescriptor"/> class.
        /// </summary>
        /// <param name="innerDescriptor">The inner descriptor instance.</param>
        /// <param name="comment">The comment to be written.</param>
        public CommentsObjectDescriptor(IObjectDescriptor innerDescriptor, string comment)
        {
            this.innerDescriptor = innerDescriptor;
            Comment = comment;
        }

        /// <summary>
        /// Gets the comment to be written.
        /// </summary>
        public string Comment { get; private set; }

        /// <inheritdoc cref="IObjectDescriptor" />
        public object Value => innerDescriptor.Value;

        /// <inheritdoc cref="IObjectDescriptor" />
        public Type Type => innerDescriptor.Type;

        /// <inheritdoc cref="IObjectDescriptor" />
        public Type StaticType => innerDescriptor.StaticType;

        /// <inheritdoc cref="IObjectDescriptor" />
        public ScalarStyle ScalarStyle => innerDescriptor.ScalarStyle;
    }
}