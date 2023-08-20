// -----------------------------------------------------------------------
// <copyright file="CustomTeamAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes
{
    using System;

    using PlayerRoles;

    /// <summary>
    /// An attribute to easily manage CustomRole initialization.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public sealed class CustomTeamAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTeamAttribute"/> class.
        /// </summary>
        public CustomTeamAttribute()
        {
        }
    }
}