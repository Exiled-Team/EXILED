// -----------------------------------------------------------------------
// <copyright file="EClassAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Attributes
{
    using System;

    /// <summary>
    /// An attribute used to specify and identify classes that can implement any type of <see cref="EProperty"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public class EClassAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EClassAttribute"/> class.
        /// </summary>
        /// <param name="assetRegistrySearchable">Indicates whether the class is searchable in the asset registry.</param>
        /// <param name="allowOnce">Indicates whether the class instance is allowed once.</param>
        /// <param name="category">The category of the class.</param>
        /// <param name="exactName">The exact name of the class.</param>
        public EClassAttribute(
            bool assetRegistrySearchable = true,
            bool allowOnce = false,
            string category = AssetRegistry.DEFAULT_CATEGORY,
            string exactName = AssetRegistry.DEFAULT_EXACT_NAME)
        {
            AssetRegistrySearchable = assetRegistrySearchable;
            AllowOnce = allowOnce;
            Category = category;
            ExactName = exactName;
        }

        /// <summary>
        /// Gets a value indicating whether the class is searchable in the asset registry.
        /// </summary>
        public bool AssetRegistrySearchable { get; }

        /// <summary>
        /// Gets a value indicating whether the class instance is allowed once.
        /// </summary>
        public bool AllowOnce { get; }

        /// <summary>
        /// Gets the category of the class.
        /// </summary>
        public string Category { get; }

        /// <summary>
        /// Gets the exact name of the class.
        /// </summary>
        public string ExactName { get; }
    }
}