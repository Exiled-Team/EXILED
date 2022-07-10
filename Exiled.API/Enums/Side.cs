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
        /// SCP team. Same as <see cref="Team.SCP"/>.
        /// Contains all SCP-related roles: <see cref="RoleType.Scp049"/>, <see cref="RoleType.Scp0492"/>, <see cref="RoleType.Scp079"/>, <see cref="RoleType.Scp096"/>,
        /// <see cref="RoleType.Scp106"/>, <see cref="RoleType.Scp173"/>, <see cref="RoleType.Scp93953"/>, and <see cref="RoleType.Scp93989"/>.
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
        /// Tutorial team. Contains <see cref="RoleType.Tutorial"/>. Same as <see cref="Team.TUT"/>.
        /// </summary>
        Tutorial,

        /// <summary>
        /// No team. Same as <see cref="Team.RIP"/>.
        /// </summary>
        None,
    }
}
