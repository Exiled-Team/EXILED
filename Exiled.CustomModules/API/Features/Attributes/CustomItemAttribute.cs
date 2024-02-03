// -----------------------------------------------------------------------
// <copyright file="CustomItemAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.Attributes
{
    using System;

    using Exiled.CustomModules.API.Features.CustomItems;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should be treated as <see cref="CustomItem"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public sealed class CustomItemAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomItemAttribute"/> class.
        /// </summary>
        public CustomItemAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomItemAttribute"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        public CustomItemAttribute(uint id) => Id = id;

        /// <summary>
        /// Gets the custom item's id.
        /// </summary>
        internal uint Id { get; }
    }
}