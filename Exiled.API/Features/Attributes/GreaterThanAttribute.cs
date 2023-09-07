// -----------------------------------------------------------------------
// <copyright file="GreaterThanAttribute.cs" company="Exiled Team">
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
    public sealed class GreaterThanAttribute : Attribute
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class.
        /// </summary>
        /// <param name="number">A number the value should be greater.</param>
        public GreaterThanAttribute(int number)
        {
            Number = number;
        }

        /// <summary>
        /// Gets the number.
        /// </summary>
        public int Number { get; }
    }
}