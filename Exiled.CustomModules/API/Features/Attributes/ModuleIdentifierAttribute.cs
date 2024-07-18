// -----------------------------------------------------------------------
// <copyright file="ModuleIdentifierAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.Attributes
{
    using System;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should identify a <see cref="CustomModule"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class ModuleIdentifierAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleIdentifierAttribute"/> class.
        /// </summary>
        public ModuleIdentifierAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleIdentifierAttribute"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        public ModuleIdentifierAttribute(uint id) => Id = id;

        /// <summary>
        /// Initializes a new instance of the <see cref="ModuleIdentifierAttribute"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        /// <param name="types"><inheritdoc cref="Types"/></param>
        public ModuleIdentifierAttribute(uint id, params Type[] types)
            : this(id) => Types = types;

        /// <summary>
        /// Gets the modules's id.
        /// </summary>
        internal uint Id { get; }

        /// <summary>
        /// Gets all component types, if supported by the target module.
        /// </summary>
        internal Type[] Types { get; }
    }
}
