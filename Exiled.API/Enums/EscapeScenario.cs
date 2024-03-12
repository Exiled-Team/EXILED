// -----------------------------------------------------------------------
// <copyright file="EscapeScenario.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Exiled.API.Features.Core.Generic;

    using static Escape;

    /// <summary>
    /// A set of different Escape Scenario Type.
    /// </summary>
    public sealed class EscapeScenario : EnumClass<EscapeScenarioType, EscapeScenario>
    {
        /// <summary>
        /// No Escape Scenario.
        /// </summary>
        public static readonly EscapeScenario None = new(EscapeScenarioType.None);

        /// <summary>
        /// ClassD Escape Scenario.
        /// </summary>
        public static readonly EscapeScenario ClassD = new(EscapeScenarioType.ClassD);

        /// <summary>
        /// Cuffed ClassD Escape.
        /// </summary>
        public static readonly EscapeScenario CuffedClassD = new(EscapeScenarioType.CuffedClassD);

        /// <summary>
        /// Scientist Escape.
        /// </summary>
        public static readonly EscapeScenario Scientist = new(EscapeScenarioType.Scientist);

        /// <summary>
        /// Cuffed Scientist Escape.
        /// </summary>
        public static readonly EscapeScenario CuffedScientist = new(EscapeScenarioType.CuffedScientist);

        /// <summary>
        /// Unspecified Escape.
        /// </summary>
        public static readonly EscapeScenario CustomEscape = new((EscapeScenarioType)5);

        private EscapeScenario(EscapeScenarioType value)
            : base(value)
        {
        }
    }
}