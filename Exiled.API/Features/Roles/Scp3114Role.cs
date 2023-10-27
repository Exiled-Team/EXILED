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

            if (!SubroutineModule.TryGetSubroutine(out Scp3114Slap scp3114Slap))
                Log.Error("Scp3114Slap not found in Scp3114Role::ctor");

            Slap = scp3114Slap;

            if (!SubroutineModule.TryGetSubroutine(out Scp3114Dance scp3114Dance))
                Log.Error("Scp3114Dance not found in Scp3114Role::ctor");

            Dance = scp3114Dance;

            if (!SubroutineModule.TryGetSubroutine(out Scp3114Reveal scp3114Reveal))
                Log.Error("Scp3114Reveal not found in Scp3114Role::ctor");

            Reveal = scp3114Reveal;

            if (!SubroutineModule.TryGetSubroutine(out Scp3114Identity scp3114Identity))
                Log.Error("Scp3114Identity not found in Scp3114Role::ctor");

            Identity = scp3114Identity;

            if (!SubroutineModule.TryGetSubroutine(out Scp3114History scp3114History))
                Log.Error("Scp3114History not found in Scp3114Role::ctor");

            History = scp3114History;

            if (!SubroutineModule.TryGetSubroutine(out Scp3114FakeModelManager scp3114FakeModelManager))
                Log.Error("Scp3114FakeModelManager not found in Scp3114Role::ctor");

            FakeModelManager = scp3114FakeModelManager;

            if (!SubroutineModule.TryGetSubroutine(out Scp3114Disguise scp3114Disguise))
                Log.Error("Scp3114Disguise not found in Scp3114Role::ctor");

            Disguise = scp3114Disguise;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type { get; } = RoleTypeId.Scp3114;

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Gets SCP-173's movement module.
        /// </summary>
        public Scp3114Slap Slap { get; }

        /// <summary>
        /// Gets SCP-173's movement module.
        /// </summary>
        public Scp3114Dance Dance { get; }

        /// <summary>
        /// Gets SCP-173's movement module.
        /// </summary>
        public Scp3114Reveal Reveal { get; }

        /// <summary>
        /// Gets SCP-173's movement module.
        /// </summary>
        public Scp3114Identity Identity { get; }

        /// <summary>
        /// Gets SCP-173's movement module.
        /// </summary>
        public Scp3114History History { get; }

        /// <summary>
        /// Gets SCP-173's movement module.
        /// </summary>
        public Scp3114FakeModelManager FakeModelManager { get; }

        /// <summary>
        /// Gets SCP-173's movement module.
        /// </summary>
        public Scp3114Disguise Disguise { get; }

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