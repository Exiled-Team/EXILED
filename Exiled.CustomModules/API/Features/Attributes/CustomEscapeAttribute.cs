// -----------------------------------------------------------------------
// <copyright file="CustomEscapeAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;
    using Exiled.CustomModules.API.Features.CustomEscapes;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should be treated as <see cref="CustomEscape"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomEscapeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEscapeAttribute"/> class.
        /// </summary>
        public CustomEscapeAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomEscapeAttribute"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        public CustomEscapeAttribute(uint id) => Id = id;

        /// <summary>
        /// Gets the custom escape's id.
        /// </summary>
        internal uint Id { get; }
    }
}
