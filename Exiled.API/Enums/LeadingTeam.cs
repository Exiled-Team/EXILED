// -----------------------------------------------------------------------
// <copyright file="LeadingTeam.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    using Extensions;

    using Features;

    using PlayerRoles;

    /// <summary>
    /// The team that is currently leading the round.
    /// </summary>
    /// <seealso cref="RoleExtensions.GetLeadingTeam(Team)"/>
    /// <seealso cref="Player.LeadingTeam"/>
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