// -----------------------------------------------------------------------
// <copyright file="UUEscapeScenarioType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Enums
{
    using Exiled.API.Features.Core.Generic;

    /// <summary>
    /// Represents the base enum class for all available escape scenarios.
    /// </summary>
    public class UUEscapeScenarioType : UnmanagedEnumClass<byte, UUEscapeScenarioType>
    {
        /// <summary>
        /// Represents an invalid scenario.
        /// </summary>
        public static readonly UUEscapeScenarioType None = new(0);

        /// <summary>
        /// Initializes a new instance of the <see cref="UUEscapeScenarioType"/> class.
        /// </summary>
        /// <param name="value">The <see cref="byte"/> value.</param>
        protected UUEscapeScenarioType(byte value)
            : base(value)
        {
        }
    }
}