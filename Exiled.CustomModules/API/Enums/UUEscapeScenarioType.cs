// -----------------------------------------------------------------------
// <copyright file="UUEscapeScenarioType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Enums
{
    using Exiled.API.Features.Core.Generics;

    /// <summary>
    /// Represents the base enum class for all available escape scenarios.
    /// </summary>
    public class UUEscapeScenarioType : UniqueUnmanagedEnumClass<byte, UUEscapeScenarioType>
    {
        /// <summary>
        /// Represents an invalid scenario.
        /// </summary>
        public static readonly UUEscapeScenarioType None = new();
    }
}