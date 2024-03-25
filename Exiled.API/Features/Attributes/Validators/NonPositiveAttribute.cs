// -----------------------------------------------------------------------
// <copyright file="NonPositiveAttribute.cs" company="Exiled Team">
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
    public sealed class NonPositiveAttribute : Attribute, IValidator
    {
        /// <inheritdoc/>
        public bool Validate(object value) => value is <= 0;
    }
}