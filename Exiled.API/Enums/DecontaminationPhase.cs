// -----------------------------------------------------------------------
// <copyright file="DecontaminationPhase.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// The different types of Decontamination phase.
    /// </summary>
    public enum DecontaminationPhase
    {
        /// <summary>
        /// The decontamination didn't start.
        /// </summary>
        NotActive = -1,

        /// <summary>
        /// The decontamination has start.
        /// </summary>
        Start,

        /// <summary>
        /// It's remaining 10 minutes before LCZ being deconataminating.
        /// </summary>
        Remaining10Minutes,

        /// <summary>
        /// It's remaining 5 minutes before LCZ being deconataminating.
        /// </summary>
        Remaining5Minutes,

        /// <summary>
        /// It's remaining 1 minutes before LCZ being deconataminating.
        /// </summary>
        Remaining1Minutes,

        /// <summary>
        /// It's remaining 30 secounds before LCZ being deconataminating.
        /// </summary>
        Remaining30Secounds,

        /// <summary>
        /// Decontamination is finish and decontaminating all the LCZ zone.
        /// </summary>
        Lockdown = 6,
    }
}
