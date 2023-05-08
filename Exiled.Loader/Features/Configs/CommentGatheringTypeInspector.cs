// -----------------------------------------------------------------------
// <copyright file="CommentGatheringTypeInspector.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Loader.Features.Configs
{
    extern alias Yaml;

    using System;
    using System.Collections.Generic;
    using System.Linq;

    using IPropertyDescriptor = Yaml::YamlDotNet.Serialization.IPropertyDescriptor;
    using ITypeInspector = Yaml::YamlDotNet.Serialization.ITypeInspector;

    /// <summary>
    /// Spurce: https://dotnetfiddle.net/8M6iIE.
    /// </summary>
    public sealed class CommentGatheringTypeInspector : Yaml::YamlDotNet.Serialization.TypeInspectors.TypeInspectorSkeleton
    {
        private readonly ITypeInspector innerTypeDescriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="CommentGatheringTypeInspector"/> class.
        /// </summary>
        /// <param name="innerTypeDescriptor">The inner type description instance.</param>
        public CommentGatheringTypeInspector(ITypeInspector innerTypeDescriptor)
        {
            this.innerTypeDescriptor = innerTypeDescriptor ?? throw new ArgumentNullException("innerTypeDescriptor");
        }

        /// <inheritdoc/>
        public override IEnumerable<IPropertyDescriptor> GetProperties(Type type, object container)
        {
            return innerTypeDescriptor
                .GetProperties(type, container)
                .Select(descriptor => new CommentsPropertyDescriptor(descriptor));
        }
    }
}