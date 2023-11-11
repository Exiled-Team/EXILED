// -----------------------------------------------------------------------
// <copyright file="Side.cs" company="Exiled Team">
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
    /// In which side a certain <see cref="RoleTypeId"/> belongs.
    /// </summary>
    /// <seealso cref="RoleExtensions.GetSide(RoleTypeId)"/>
    /// <seealso cref="RoleExtensions.GetSide(Team)"/>
    /// <seealso cref="Player.Get(Side)"/>
    /// <seealso cref="Round.AliveSides"/>
    /// <seealso cref="Side"/>
    public enum Side
    {
        /// <summary>
        /// SCP team. Same as <see cref="Team.SCPs"/>.
        /// Contains all SCP-related roles: <see cref="RoleTypeId.Scp049"/>, <see cref="RoleTypeId.Scp0492"/>, <see cref="RoleTypeId.Scp079"/>, <see cref="RoleTypeId.Scp096"/>,
        /// <see cref="RoleTypeId.Scp106"/>, <see cref="RoleTypeId.Scp173"/>, and <see cref="RoleTypeId.Scp939"/>.
        /// </summary>
        Scp,

        /// <summary>
        /// Mobile Task Forces team.
        /// Contains <see cref="RoleTypeId.Scientist"/>, <see cref="RoleTypeId.FacilityGuard"/>, <see cref="RoleTypeId.NtfPrivate"/>, <see cref="RoleTypeId.NtfSergeant"/>,
        /// <see cref="RoleTypeId.NtfCaptain"/> and <see cref="RoleTypeId.NtfSpecialist"/>.
        /// </summary>
        Mtf,

        /// <summary>
        /// Chaos Insurgency team.
        /// Contains <see cref="RoleTypeId.ClassD"/>, <see cref="RoleTypeId.ChaosConscript"/>, <see cref="RoleTypeId.ChaosRepressor"/>, <see cref="RoleTypeId.ChaosRifleman"/> and <see cref="RoleTypeId.ChaosMarauder"/>.
        /// </summary>
        ChaosInsurgency,

        /// <summary>
        /// Tutorial team. Contains <see cref="RoleTypeId.Tutorial"/>. Same as <see cref="Team.OtherAlive"/>.
        /// </summary>
        Tutorial,

        /// <summary>
        /// No team. Same as <see cref="Team.Dead"/>, <see cref="RoleTypeId.Overwatch"/> and <see cref="RoleTypeId.Filmmaker"/>.
        /// </summary>
        None,
    }
}