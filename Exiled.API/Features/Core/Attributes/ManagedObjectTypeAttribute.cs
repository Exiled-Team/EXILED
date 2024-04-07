// -----------------------------------------------------------------------
// <copyright file="ManagedObjectTypeAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Core.Attributes
{
    using System;

    /// <summary>
    /// An attribute to mark an <see cref="EObject"/> type as a managed object type.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ManagedObjectTypeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ManagedObjectTypeAttribute"/> class.
        /// </summary>
        public ManagedObjectTypeAttribute()
        {
        }
    }
}