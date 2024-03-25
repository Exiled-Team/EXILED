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
    [AttributeUsage(AttributeTargets.Property)]
    public sealed class PossibleValuesAttribute : Attribute, IValidator
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PossibleValuesAttribute"/> class.
        /// </summary>
        /// <param name="values">Values should be used.</param>
        public PossibleValuesAttribute(params object[] values)
        {
            Values = values;
        }

        /// <summary>
        /// Gets the values.
        /// </summary>
        public object[] Values { get; }

        /// <inheritdoc/>
        public bool Validate(object value) => Values.Contains(value);
    }
}