// -----------------------------------------------------------------------
// <copyright file="Scp1507Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using Exiled.API.Enums;
    using Exiled.API.Interfaces;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.HumeShield;
    using PlayerRoles.PlayableScps.Scp1507;
    using PlayerRoles.Subroutines;

    using BaseRole = PlayerRoles.PlayableScps.Scp1507.Scp1507Role;

    /// <summary>
    /// A wrapper for <see cref="BaseRole"/>.
    /// </summary>
    public class Scp1507Role : Role, IWrapper<BaseRole>, ISubroutinedScpRole, IHumeShieldRole
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Scp1507Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="PlayerRoleBase"/>.</param>
        public Scp1507Role(BaseRole baseRole)
            : base(baseRole)
        {
            Base = baseRole;

            SubroutineModule = baseRole.SubroutineModule;
            HumeShieldModule = baseRole.HumeShieldModule;

            if (!SubroutineModule.TryGetSubroutine(out Scp1507AttackAbility attackAbility))
                Log.Error($"Attack ability is not a subroutine for {nameof(Scp1507Role)}");

            AttackAbility = attackAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp1507SwarmAbility swarmAbility))
                Log.Error($"Swarm ability is not a subroutine for {nameof(Scp1507Role)}");

            SwarmAbility = swarmAbility;

            if (!SubroutineModule.TryGetSubroutine(out Scp1507VocalizeAbility vocalizeAbility))
                Log.Error($"Vocalize ability is not a subroutine for {nameof(Scp1507Role)}");

            VocalizeAbility = vocalizeAbility;
        }

        /// <inheritdoc/>
        public override RoleTypeId Type => Base._roleTypeId;

        /// <inheritdoc/>
        public new BaseRole Base { get; }

        /// <inheritdoc/>
        public SubroutineManagerModule SubroutineModule { get; }

        /// <summary>
        /// Gets the <see cref="Scp1507AttackAbility"/> for this role.
        /// </summary>
        public Scp1507AttackAbility AttackAbility { get; }

        /// <summary>
        /// Gets the <see cref="Scp1507SwarmAbility"/> for this role.
        /// </summary>
        public Scp1507SwarmAbility SwarmAbility { get; }

        /// <summary>
        /// Gets the <see cref="Scp1507VocalizeAbility"/> for this role.
        /// </summary>
        public Scp1507VocalizeAbility VocalizeAbility { get; }

        /// <summary>
        /// Gets or sets how much damage should deal SCP-1507.
        /// </summary>
        public float Damage
        {
            get => AttackAbility._damage;
            set => AttackAbility._damage = value;
        }

        /// <summary>
        /// Gets the delay between attacks.
        /// </summary>
        public float AttackDelay => AttackAbility.AttackDelay;

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }
    }
}