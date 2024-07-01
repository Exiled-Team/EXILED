// -----------------------------------------------------------------------
// <copyright file="EClass.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core
{
    using System;

    using Exiled.API.Features.Core.Attributes;

    /// <summary>
    /// Represents a class of an <see cref="EntityAsset"/>.
    /// </summary>
    public readonly struct EClass
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EClass"/> struct.
        /// </summary>
        /// <param name="entity">The entity belonging to the property.</param>
        /// <param name="assetRegistrySearchable">Indicates whether the class is searchable in the asset registry.</param>
        /// <param name="allowOnce">Indicates whether the class instance is allowed once.</param>
        /// <param name="category">The category of the property.</param>
        /// <param name="exactName">The exact name of the property.</param>
        internal EClass(
            object entity,
            bool assetRegistrySearchable = true,
            bool allowOnce = false,
            string category = AssetRegistry.DEFAULT_CATEGORY,
            string exactName = AssetRegistry.DEFAULT_CATEGORY)
        {
            Entity = entity;
            Type = entity.GetType();
            AssetRegistrySearchable = assetRegistrySearchable;
            AllowOnce = allowOnce;
            Category = category;
            ExactName = exactName;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="EClass"/> struct.
        /// </summary>
        /// <param name="entity">The entity belonging to the property.</param>
        /// <param name="classAttribute">The <see cref="EClassAttribute"/> containing all information to initialize the class.</param>
        internal EClass(object entity, EClassAttribute classAttribute)
        {
            Entity = entity;
            Type = entity.GetType();
            AssetRegistrySearchable = classAttribute.AssetRegistrySearchable;
            AllowOnce = classAttribute.AllowOnce;
            Category = classAttribute.Category;
            ExactName = classAttribute.ExactName;
        }

        /// <summary>
        /// Gets the entity instance belonging to this property.
        /// </summary>
        public object Entity { get; }

        /// <summary>
        /// Gets the class type.
        /// </summary>
        public Type Type { get; }

        /// <summary>
        /// Gets a value indicating whether the class is searchable in the asset registry.
        /// </summary>
        public bool AssetRegistrySearchable { get; }

        /// <summary>
        /// Gets a value indicating whether the class instance is allowed once.
        /// </summary>
        public bool AllowOnce { get; }

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