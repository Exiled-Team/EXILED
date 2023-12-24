// -----------------------------------------------------------------------
// <copyright file="CustomTeamAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features
{
    using System;

    using Exiled.CustomModules.API.Features.CustomRoles;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should be treated as <see cref="CustomTeam"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class CustomTeamAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTeamAttribute"/> class.
        /// </summary>
        public CustomTeamAttribute()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTeamAttribute"/> class.
        /// </summary>
        /// <param name="id"><inheritdoc cref="Id"/></param>
        public CustomTeamAttribute(uint id) => Id = id;

        /// <summary>
        /// Gets the custom team's id.
        /// </summary>
        internal uint Id { get; }
    }
}
