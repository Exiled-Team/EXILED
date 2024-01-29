// -----------------------------------------------------------------------
// <copyright file="CustomGameModeAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should be treated as <see cref="CustomGameMode"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomGameModeAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomGameModeAttribute"/> class.
        /// </summary>
        public CustomGameModeAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomGameModeAttribute"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        public CustomGameModeAttribute(uint id) => Id = id;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomGameModeAttribute"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        /// <param name="types"><inheritdoc cref="Types"/></param>
        public CustomGameModeAttribute(uint id, params Type[] types)
            : this(id) => Types = types;

        /// <summary>
        /// Gets the custom gamemode's id.
        /// </summary>
        internal uint Id { get; }

        /// <summary>
        /// Gets all component types.
        /// </summary>
        internal Type[] Types { get; }
    }
}
