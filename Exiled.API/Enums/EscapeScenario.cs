// -----------------------------------------------------------------------
// <copyright file="EscapeScenario.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// A set of different Escape Scenario Type.
    /// </summary>
    public enum EscapeScenario
    {
        /// <summary>
        /// No Escape Scenario.
        /// </summary>
        None,

        /// <summary>
        /// ClassD Escape Scenario.
        /// </summary>
        ClassD,

        /// <summary>
        /// Cuffed ClassD Escape.
        /// </summary>
        CuffedClassD,

        /// <summary>
        /// Scientist Escape.
        /// </summary>
        Scientist,

        /// <summary>
        /// Cuffed Scientist Escape.
        /// </summary>
        CuffedScientist,

        /// <summary>
        /// Unspecified Escape.
        /// </summary>
        CustomEscape,
    }
}