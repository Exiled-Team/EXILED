// -----------------------------------------------------------------------
// <copyright file="PossibleValuesAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes
{
    using System;

    /// <summary>
    /// An attribute to easily manage config values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PossibleValuesAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PossibleValuesAttribute"/> class.
        /// </summary>
        /// <param name="values">Values should be used.</param>
        public PossibleValuesAttribute(params string[] values)
        {
            Values = values;
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public string[] Values { get; }
    }
}