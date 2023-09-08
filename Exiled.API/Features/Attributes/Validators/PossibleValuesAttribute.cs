// -----------------------------------------------------------------------
// <copyright file="PossibleValuesAttribute.cs" company="Exiled Team">
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
    /// <typeparam name="T">The type.</typeparam>
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PossibleValuesAttribute<T> : Attribute, IValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PossibleValuesAttribute{T}"/> class.
        /// </summary>
        /// <param name="values">Values should be used.</param>
        public PossibleValuesAttribute(params T[] values)
        {
            Values = values;
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public T[] Values { get; }

        /// <inheritdoc/>
        public bool Validate(object value) => value is T type && Values.Contains(type);
    }
}