// -----------------------------------------------------------------------
// <copyright file="Scp1507Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System.Collections.Generic;
    using System.Linq;

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

        /// <summary>
        /// Gets or sets a list with flamingos, which are close to owner.
        /// </summary>
        public IEnumerable<Player> NearbyFlamingos
        {
            get => SwarmAbility._nearbyFlamingos.Select(x => Player.Get(x._lastOwner));
            set
            {
                SwarmAbility._nearbyFlamingos.Clear();

                foreach (var player in value.Where(x => x.Role.Is<Scp1507Role>(out _)))
                    SwarmAbility._nearbyFlamingos.Add(player.Role.As<Scp1507Role>().Base);
            }
        }

        /// <summary>
        /// Gets or sets a list with all flamingos.
        /// </summary>
        public IEnumerable<Player> AllFlamingos
        {
            get => SwarmAbility._entireFlock.Select(x => Player.Get(x._lastOwner));
            set
            {
                SwarmAbility._entireFlock.Clear();

                foreach (var player in value.Where(x => x.Role.Is<Scp1507Role>(out _)))
                    SwarmAbility._entireFlock.Add(player.Role.As<Scp1507Role>().Base);

                SwarmAbility._flockSize = (byte)(SwarmAbility._entireFlock.Count - 1);
            }
        }

        /// <summary>
        /// Gets or sets a multiplier for healing.
        /// </summary>
        public float Multiplier
        {
            get => SwarmAbility.Multiplier;
            set => SwarmAbility.Multiplier = value;
        }

        /// <inheritdoc/>
        public HumeShieldModuleBase HumeShieldModule { get; }

        /// <summary>
        /// Tries to attack door.
        /// </summary>
        /// <returns><see langword="true"/> if successfully. Otherwise, <see langword="false"/>.</returns>
        /// <remarks>This method does not modify game logic, so if you want this method to work correctly, make sure that player is staying in front of the door.</remarks>
        public bool TryAttackDoor() => AttackAbility.TryAttackDoor();

        /// <summary>
        /// Forces a SCP-1507 to scream.
        /// </summary>
        public void Scream() => VocalizeAbility.ServerScream();
    }
}