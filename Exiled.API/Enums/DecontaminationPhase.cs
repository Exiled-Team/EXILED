// -----------------------------------------------------------------------
// <copyright file="DecontaminationPhase.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Features;

    /// <summary>
    /// Represents the state of a <see cref="LightContainmentZoneDecontamination.DecontaminationController"/>.
    /// </summary>
    /// <seealso cref="Map.DecontaminationPhase"/>
    public enum DecontaminationPhase
    {
        /// <summary>
        /// Decontamination has started.
        /// </summary>
        Start,

        /// <summary>
        /// It's 10 minutes remaining.
        /// </summary>
        Remain10Minutes,

        /// <summary>
        /// It's 5 minutes remaining.
        /// </summary>
        Remain5Minutes,

        /// <summary>
        /// It's 1 minute remaining.
        /// </summary>
        Remain1Minute,

        /// <summary>
        /// It's 30 seconds remaining.
        /// </summary>
        Countdown,

        /// <summary>
        /// All doors are closed and lock.
        /// </summary>
        Lockdown,

        /// <summary>
        /// The decontamination has finished.
        /// </summary>
        Finish,
    }
}
