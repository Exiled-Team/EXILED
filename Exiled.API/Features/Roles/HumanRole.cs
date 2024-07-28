// -----------------------------------------------------------------------
// <copyright file="HumanRole.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using Exiled.API.Features.Core.Attributes;
    using PlayerRoles;
    using Respawning;
    using Respawning.NamingRules;

    using HumanGameRole = PlayerRoles.HumanRole;

    /// <summary>
    /// Defines a role that represents a human class.
    /// </summary>
    public class HumanRole : FpcRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HumanRole"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="HumanGameRole"/>.</param>
        internal HumanRole(HumanGameRole baseRole)
            : base(baseRole)
        {
            Base = baseRole;
        }

        /// <inheritdoc/>
        [EProperty(readOnly: true, category: nameof(Role))]
        public override RoleTypeId Type => Base.RoleTypeId;

        /// <summary>
        /// Gets or sets the <see cref="SpawnableTeamType"/>.
        /// </summary>
        public SpawnableTeamType SpawnableTeamType
        {
            get => Base.AssignedSpawnableTeam;
            set => Base.AssignedSpawnableTeam = value;
        }

        /// <summary>
        /// Gets the player's unit name.
        /// </summary>
        public string UnitName => UnitNameMessageHandler.GetReceived(Base.AssignedSpawnableTeam, Base.UnitNameId);

        /// <summary>
        /// Gets or sets the <see cref="UnitNameId"/>.
        /// </summary>
        public byte UnitNameId
        {
            get => Base.UnitNameId;
            set => Base.UnitNameId = value;
        }

        /// <summary>
        /// Gets a value indicating whether the <see cref="HumanRole"/> uses unit names or not.
        /// </summary>
        public bool UsesUnitNames => Base.UsesUnitNames;

        /// <summary>
        /// Gets the game <see cref="HumanGameRole"/>.
        /// </summary>
        public new HumanGameRole Base { get; }

        /// <summary>
        /// Gets the <see cref="HumanRole"/> armor efficacy based on a specific <see cref="HitboxType"/> and the armor the <see cref="Role.Owner"/> is wearing.
        /// </summary>
        /// <param name="hitbox">The <see cref="HitboxType"/>.</param>
        /// <returns>The armor efficacy.</returns>
        public int GetArmorEfficacy(HitboxType hitbox) => Base.GetArmorEfficacy(hitbox);
    }
}