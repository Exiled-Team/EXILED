// -----------------------------------------------------------------------
// <copyright file="LeadingTeam.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.API.Enums
{
    /// <summary>
    /// The team that is currently leading the round.
    /// </summary>
    public enum LeadingTeam : byte
    {
        /// <summary>
        /// Represents the Scientists, Guards, and NTF team.
        /// </summary>
        FacilityForces,

        /// <summary>
        /// Represents the Class-D and Chaos Insurgency team.
        /// </summary>
        ChaosInsurgency,

        /// <summary>
        /// Represents the SCP team.
        /// </summary>
        Anomalies,

        /// <summary>
        /// Represents a draw.
        /// </summary>
        Draw,
    }
}
