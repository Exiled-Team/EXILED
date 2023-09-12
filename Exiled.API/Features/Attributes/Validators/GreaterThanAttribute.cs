// -----------------------------------------------------------------------
// <copyright file="GreaterThanAttribute.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Attributes.Validators
{
    using System;

    using Exiled.API.Interfaces;

    /// <summary>
    /// An attribute to easily manage config values.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class GreaterThanAttribute : Attribute, IValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class.
        /// </summary>
        /// <param name="number">A number the value should be greater.</param>
        /// <param name="isIncluded">Whether or not <paramref name="number"></paramref> is included.</param>
        public GreaterThanAttribute(int number, bool isIncluded = false)
        {
            Number = number;
            IsIncluded = isIncluded;
        }

        /// <summary>
        /// Gets the number.
        /// </summary>
        public int Number { get; }

        /// <summary>
        /// Gets a value indicating whether or not <see cref="Number"/> is included.
        /// </summary>
        public bool IsIncluded { get; }

        /// <inheritdoc/>
        public bool Validate(object value) => value is int number && (IsIncluded ? number >= Number : number > Number);
    }
}