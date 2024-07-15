// -----------------------------------------------------------------------
// <copyright file="IValidator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Interfaces
{
    /// <summary>
    /// Defines the contract for config validator.
    /// </summary>
    public interface IValidator
    {
        /// <summary>
        /// Validates a config value.
        /// </summary>
        /// <param name="value">A value to validate.</param>
        /// <returns>Returns whether config value is correct.</returns>
        bool Validate(object value);
    }
}