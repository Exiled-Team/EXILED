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
    /// An attribute that validates if the value of the marked property is greater than a specified number.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class GreaterThanAttribute : Attribute, IValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GreaterThanAttribute"/> class.
        /// </summary>
        /// <param name="number">The number the marked property should be greater than.</param>
        /// <param name="isIncluded">Whether or not the comparison in inclusive (includes <see cref="Number"/> as a valid value for the marked property).</param>
        public GreaterThanAttribute(IComparable number, bool isIncluded = false)
        {
            Number = number;
            IsIncluded = isIncluded;
        }

        /// <summary>
        /// Gets the number that the value of the marked property should be greater than.
        /// </summary>
        public IComparable Number { get; }

        /// <summary>
        /// Gets a value indicating whether or not the comparison is inclusive.
        /// <remarks>If this returns true, <see cref="Number"/> is a valid value for the marked property.</remarks>
        /// </summary>
        public bool IsIncluded { get; }

        /// <inheritdoc/>
        public bool Validate(object value) => Number.CompareTo(value) is -1 || (IsIncluded && Number.CompareTo(value) is 0);
    }
}
