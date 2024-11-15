// -----------------------------------------------------------------------
// <copyright file="CustomEscapeScenarioType.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example.TestEscape
{
    using Exiled.CustomModules.API.Enums;

    /// <summary>
    /// The custom escape scenario type.
    /// </summary>
    public class CustomEscapeScenarioType : UUEscapeScenarioType
    {
        /// <summary>
        /// Initializes a new custom escape scenario id.
        /// </summary>
        public static readonly CustomEscapeScenarioType CustomScenario = new();
    }
}