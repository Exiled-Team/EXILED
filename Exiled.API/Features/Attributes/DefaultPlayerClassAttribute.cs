// -----------------------------------------------------------------------
// <copyright file="DefaultPlayerClassAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes
{
    using System;

    /// <summary>
    /// This attribute determines whether the class which is being applied to should replace the default <see cref="Player"/> class.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = true)]
    public class DefaultPlayerClassAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultPlayerClassAttribute"/> class.
        /// </summary>
        /// <param name="enforceAuthority">A value indicating whether the type should enforce its authority, ignoring all other <see cref="Player"/> classes.</param>
        public DefaultPlayerClassAttribute(bool enforceAuthority = false) => EnforceAuthority = enforceAuthority;

        /// <summary>
        /// Gets a value indicating whether the type should enforce its authority, ignoring all other <see cref="Player"/> classes.
        /// </summary>
        public bool EnforceAuthority { get; }
    }
}