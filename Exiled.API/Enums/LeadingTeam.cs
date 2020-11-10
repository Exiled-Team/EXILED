// -----------------------------------------------------------------------
// <copyright file="LeadingTeam.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// The team that is currently leading the round.
    /// </summary>
    public enum LeadingTeam : byte
    {
        /// <summary>
        /// Represents Scientists, Guards, and NTF.
        /// </summary>
        FacilityForces,

        /// <summary>
        /// Represents Class-D and Chaos Insurgency.
        /// </summary>
        ChaosInsurgency,

        /// <summary>
        /// Represents SCPs.
        /// </summary>
        Anomalies,

        /// <summary>
        /// Represents a draw.
        /// </summary>
        Draw,
    }
}
