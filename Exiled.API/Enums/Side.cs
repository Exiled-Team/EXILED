// -----------------------------------------------------------------------
// <copyright file="Side.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Enums
{
    /// <summary>
    /// In which side a certain <see cref="RoleType"/> belongs.
    /// </summary>
    public enum Side
    {
        /// <summary>
        /// The same as <see cref="Team.SCP"/>.
        /// </summary>
        Scp,

        /// <summary>
        /// Mobile Task Forces team.
        /// Contains <see cref="RoleType.Scientist"/>, <see cref="RoleType.FacilityGuard"/>, <see cref="RoleType.NtfPrivate"/>, <see cref="RoleType.NtfSergeant"/>,
        /// <see cref="RoleType.NtfCaptain"/> and <see cref="RoleType.NtfSpecialist"/>.
        /// </summary>
        Mtf,

        /// <summary>
        /// Chaos Insurgency team.
        /// Contains <see cref="RoleType.ClassD"/>, <see cref="RoleType.ChaosConscript"/>, <see cref="RoleType.ChaosRepressor"/>, <see cref="RoleType.ChaosRifleman"/> and <see cref="RoleType.ChaosMarauder"/>.
        /// </summary>
        ChaosInsurgency,

        /// <summary>
        /// <see cref="Team.TUT"/>.
        /// </summary>
        Tutorial,

        /// <summary>
        /// <see cref="Team.RIP"/>.
        /// </summary>
        None,
    }
}
