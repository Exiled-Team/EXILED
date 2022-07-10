// -----------------------------------------------------------------------
// <copyright file="WarheadStatus.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// All the available warhead statuses.
    /// </summary>
    public enum WarheadStatus
    {
        /// <summary>
        /// The warhead is not armed.
        /// </summary>
        NotArmed,

        /// <summary>
        /// The warhead is armed.
        /// </summary>
        Armed,

        /// <summary>
        /// The warhead detonation is in progress.
        /// </summary>
        InProgress,

        /// <summary>
        /// The warhead has detonated.
        /// </summary>
        Detonated,
    }
}
