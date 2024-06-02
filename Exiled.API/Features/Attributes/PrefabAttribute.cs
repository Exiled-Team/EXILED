// -----------------------------------------------------------------------
// <copyright file="PrefabAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes
{
    using System;

    /// <inheritdoc />
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class PrefabAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PrefabAttribute"/> class.
        /// </summary>
        /// <param name="assetId"><inheritdoc cref="AssetId"/></param>
        /// <param name="name"><inheritdoc cref="Name"/></param>
        public PrefabAttribute(uint assetId, string name)
        {
            AssetId = assetId;
            Name = name;
        }

        /// <summary>
        /// Gets the prefab's asset id.
        /// </summary>
        public uint AssetId { get; }

        /// <summary>
        /// Gets the prefab's name.
        /// </summary>
        public string Name { get; }
    }
}