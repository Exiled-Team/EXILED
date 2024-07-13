// -----------------------------------------------------------------------
// <copyright file="EPropertyAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Attributes
{
    using System;

    /// <summary>
    /// An attribute used to specify and identify properties that can be manipulated externally and within subobjects.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class EPropertyAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EPropertyAttribute"/> class.
        /// </summary>
        /// <param name="registrySearchable">Indicates whether the property is searchable in the registry.</param>
        /// <param name="readOnly">Indicates whether the property is read-only.</param>
        /// <param name="serializable">Indicates whether the property is serializable.</param>
        /// <param name="category">The category of the property.</param>
        /// <param name="exactName">The exact name of the property.</param>
        public EPropertyAttribute(
            bool registrySearchable = true,
            bool readOnly = false,
            bool serializable = true,
            string category = AssetRegistry.DEFAULT_CATEGORY,
            string exactName = AssetRegistry.DEFAULT_EXACT_NAME)
        {
            RegistrySearchable = registrySearchable;
            ReadOnly = readOnly;
            Serializable = serializable;
            Category = category;
            ExactName = exactName;
        }

        /// <summary>
        /// Gets a value indicating whether the property is searchable in the asset registry.
        /// </summary>
        public bool RegistrySearchable { get; }

        /// <summary>
        /// Gets a value indicating whether the property is read-only.
        /// </summary>
        public bool ReadOnly { get; }

        /// <summary>
        /// Gets a value indicating whether the property is serializable.
        /// </summary>
        public bool Serializable { get; }

        /// <summary>
        /// Gets the category of the property.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Gets the exact name of the property.
        /// </summary>
        public string ExactName { get; }
    }
}