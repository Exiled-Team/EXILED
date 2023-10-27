// -----------------------------------------------------------------------
// <copyright file="Scp3114Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;
    using System.Linq;

    using Exiled.API.Features.Hazards;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp3114;
    using PlayerRoles.PlayableScps.Subroutines;
    using UnityEngine;

    using Scp3114GameRole = PlayerRoles.PlayableScps.Scp3114.Scp3114Role;

    /// <summary>
    /// Defines a role that represents SCP-173.
    /// </summary>
    public class Scp3114Role : FpcRole, ISubroutinedScpRole, IHumeShieldRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp3114Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="Scp3114GameRole"/>.</param>
        internal Scp3114Role(Scp3114GameRole baseRole)
            : base(baseRole)
        {
            Base = baseRole;
            SubroutineModule = baseRole.SubroutineModule;
            HumeShieldModule = baseRole.HumeShieldModule;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp3114;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets the <see cref="Scp3114GameRole"/> instance.
        /// </summary>
        public new Scp3114GameRole Base { get; }

        /// <summary>
        /// Gets the Spawn Chance of SCP-173.
        /// </summary>
        /// <param name="alreadySpawned">The List of Roles already spawned.</param>
        /// <returns>The Spawn Chance.</returns>
        public float GetSpawnChance(List<RoleTypeId> alreadySpawned) => 0;
    }
}