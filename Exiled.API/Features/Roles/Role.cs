// -----------------------------------------------------------------------
// <copyright file="Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;

    using Enums;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Attributes;
    using Exiled.API.Features.Core.Interfaces;
    using Exiled.API.Features.Spawn;
    using Exiled.API.Interfaces;
    using Extensions;
    using PlayerRoles;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using PlayerRoles.RoleAssign;
    using UnityEngine;

    using DestroyedGameRole = PlayerRoles.DestroyedRole;
    using FilmmakerGameRole = PlayerRoles.Filmmaker.FilmmakerRole;
    using HumanGameRole = PlayerRoles.HumanRole;
    using OverwatchGameRole = PlayerRoles.Spectating.OverwatchRole;
    using Scp049GameRole = PlayerRoles.PlayableScps.Scp049.Scp049Role;
    using Scp079GameRole = PlayerRoles.PlayableScps.Scp079.Scp079Role;
    using Scp096GameRole = PlayerRoles.PlayableScps.Scp096.Scp096Role;
    using Scp106GameRole = PlayerRoles.PlayableScps.Scp106.Scp106Role;
    using Scp173GameRole = PlayerRoles.PlayableScps.Scp173.Scp173Role;
    using Scp3114GameRole = PlayerRoles.PlayableScps.Scp3114.Scp3114Role;
    using Scp939GameRole = PlayerRoles.PlayableScps.Scp939.Scp939Role;
    using SpectatorGameRole = PlayerRoles.Spectating.SpectatorRole;

    /// <summary>
    /// Defines the class for role-related classes.
    /// </summary>
    [EClass(allowOnce: true, category: nameof(Role))]
    public abstract class Role : GameEntity, IWrapper<PlayerRoleBase>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        /// <param name="gameObject">The <see cref="GameObject"/>.</param>
        protected internal Role(GameObject gameObject)
            : base(gameObject)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Role"/> class.
        /// </summary>
        /// <param name="baseRole">the base <see cref="PlayerRoleBase"/>.</param>
        protected Role(PlayerRoleBase baseRole)
            : base(baseRole.gameObject)
        {
            if (baseRole.TryGetOwner(out ReferenceHub hub))
                Owner = Player.Get(hub);

            Base = baseRole;
        }

        /// <summary>
        /// Gets an array of all <see cref="RoleTypeId"/>.
        /// </summary>
        public static IEnumerable<RoleTypeId> AllRoles { get; } = EnumExtensions.QueryValues<RoleTypeId>();

        /// <summary>
        /// Gets a shuffled list of all possible <see cref="RoleTypeId"/>.
        /// </summary>
        public static IEnumerable<RoleTypeId> ShuffledAllRoles => AllRoles.Shuffle();

        /// <summary>
        /// Gets a random human <see cref="RoleTypeId"/>.
        /// </summary>
        public static RoleTypeId RandomHuman => RoleExtensions.GetRandomRole(r => r.IsHuman());

        /// <summary>
        /// Gets a random human <see cref="RoleTypeId"/>.
        /// </summary>
        public static RoleTypeId RandomScp => Team.SCPs.GetRandomRole();

        /// <summary>
        /// Gets the next Scp to spawn according to NW logic.
        /// </summary>
        public static RoleTypeId NextScpSpawn => ScpSpawner.NextScp;

        /// <summary>
        /// Gets the next Human to spawn according to NW logic.
        /// </summary>
        public static RoleTypeId NextHumanSpawn => HumanSpawner.NextHumanRoleToSpawn;

        /// <summary>
        /// Gets the <see cref="Player"/> this role is referring to.
        /// </summary>
        public virtual Player Owner { get; }

        /// <summary>
        /// Gets the <see cref="RoleTypeId"/> of this <see cref="Player"/>.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Role))]
        public abstract RoleTypeId Type { get; }

        /// <summary>
        /// Gets the base <see cref="PlayerRoleBase"/>.
        /// </summary>
        public PlayerRoleBase Base { get; }

        /// <summary>
        /// Gets the <see cref="RoleChangeReason"/>.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Role))]
        public RoleChangeReason SpawnReason => Base.ServerSpawnReason;

        /// <summary>
        /// Gets the <see cref="RoleSpawnFlags"/>.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Role))]
        public RoleSpawnFlags SpawnFlags => Base.ServerSpawnFlags;

        /// <summary>
        /// Gets the <see cref="PlayerRoles.Team"/> of this <see cref="Role"/>.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Role))]
        public Team Team => Base.Team;

        /// <summary>
        /// Gets the <see cref="Enums.Side"/> of this <see cref="Role"/>.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Role))]
        public Side Side => Team.GetSide();

        /// <summary>
        /// Gets the <see cref="UnityEngine.Color"/> of this <see cref="Role"/>.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Role))]
        public Color Color => Base.RoleColor;

        /// <summary>
        /// Gets the <see cref="Role"/> full name.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Role))]
        public string Name => Base.RoleName;

        /// <summary>
        /// Gets the last time the <see cref="Role"/> was active.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Role))]
        public TimeSpan ActiveTime => TimeSpan.FromSeconds(Base.ActiveTime);

        /// <summary>
        /// Gets a value indicating whether or not this role represents a dead role.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Role))]
        public bool IsDead => Team is Team.Dead;

        /// <summary>
        /// Gets a value indicating whether or not this role represents a living role.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Role))]
        public bool IsAlive => !IsDead;

        /// <summary>
        /// Gets a value indicating whether or not this role is still valid. This will only ever be <see langword="false"/> if the Role is stored and accessed at a later date.
        /// </summary>
        [EProperty(readOnly: true, category: nameof(Role))]
        public bool IsValid => Owner != null && Owner.IsConnected && Base == Owner.RoleManager.CurrentRole;

        /// <summary>
        /// Gets a random spawn position of this role.
        /// </summary>
        /// <returns>The spawn position.</returns>
        public virtual SpawnLocation RandomSpawnLocation => Type.GetRandomSpawnLocation();

        /// <summary>
        /// Converts a role to its appropriate <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        public static implicit operator RoleTypeId(Role role) => role?.Type ?? RoleTypeId.None;

        /// <summary>
        /// Returns whether or not 2 roles are the same.
        /// </summary>
        /// <param name="left">The role.</param>
        /// <param name="right">The other role.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(Role left, Role right) => left?.Equals(right) ?? right is null;

        /// <summary>
        /// Returns whether or not the two roles are different.
        /// </summary>
        /// <param name="left">The role.</param>
        /// <param name="right">The other role.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(Role left, Role right) => !(left == right);

        /// <summary>
        /// Returns whether or not the role has the same RoleTypeId as the given <paramref name="typeId"/>.
        /// </summary>
        /// <param name="role">The <see cref="Role"/>.</param>
        /// <param name="typeId">The <see cref="RoleTypeId"/>.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(Role role, RoleTypeId typeId) => role?.Type == typeId;

        /// <summary>
        /// Returns whether or not the role has a different RoleTypeId as the given <paramref name="typeId"/>.
        /// </summary>
        /// <param name="role">The <see cref="Role"/>.</param>
        /// <param name="typeId">The <see cref="RoleTypeId"/>.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(Role role, RoleTypeId typeId) => !(role == typeId);

        /// <summary>
        /// Returns whether or not the role has the same RoleTypeId as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="RoleTypeId"/>.</param>
        /// <param name="role">The <see cref="Role"/>.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(RoleTypeId type, Role role) => role == type;

        /// <summary>
        /// Returns whether or not the role has a different RoleTypeId as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="RoleTypeId"/>.</param>
        /// <param name="role">The <see cref="Role"/>.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(RoleTypeId type, Role role) => role != type;

        /// <summary>
        /// Gets a random <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="includeNonPlayableRoles">Specifies whether non-playable roles should be included.</param>
        /// <param name="except">An optional collection of role types to exclude.</param>
        /// <returns>A random <see cref="RoleTypeId"/>.</returns>
        public static RoleTypeId Random(bool includeNonPlayableRoles = false, IEnumerable<RoleTypeId> except = null)
        {
            IEnumerable<RoleTypeId> roles = includeNonPlayableRoles
                ? ShuffledAllRoles.Except(except ?? Enumerable.Empty<RoleTypeId>())
                : ShuffledAllRoles.RemoveSpecified(r => RoleExtensions.GetTeam(r) == Team.Dead).Except(except ?? Enumerable.Empty<RoleTypeId>());

            return roles.FirstOrDefault();
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => base.Equals(obj);

        /// <summary>
        /// Returns the role in a human-readable format.
        /// </summary>
        /// <returns>A string containing role-related data.</returns>
        public override string ToString() => $"{Side} {Team} {Type} {IsValid}";

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Sets the player's <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="newRole">The new <see cref="RoleTypeId"/> to be set.</param>
        /// <param name="reason">The <see cref="RoleChangeReason"/> defining why the player's role was changed.</param>
        public virtual void Set(RoleTypeId newRole, RoleChangeReason reason = RoleChangeReason.RemoteAdmin) => Set(newRole, reason, RoleSpawnFlags.All);

        /// <summary>S
        /// Sets the player's <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="newRole">The new <see cref="RoleTypeId"/> to be set.</param>
        /// <param name="spawnFlags">The <see cref="RoleSpawnFlags"/> defining player spawn logic.</param>
        public virtual void Set(RoleTypeId newRole, RoleSpawnFlags spawnFlags) => Owner.RoleManager.ServerSetRole(newRole, Enums.SpawnReason.ForceClass, spawnFlags);

        /// <summary>
        /// Sets the player's <see cref="RoleTypeId"/>.
        /// </summary>
        /// <param name="newRole">The new <see cref="RoleTypeId"/> to be set.</param>
        /// <param name="reason">The <see cref="RoleChangeReason"/> defining why the player's role was changed.</param>
        /// <param name="spawnFlags">The <see cref="RoleSpawnFlags"/> defining player spawn logic.</param>
        public virtual void Set(RoleTypeId newRole, RoleChangeReason reason, RoleSpawnFlags spawnFlags) =>
            Owner.RoleManager.ServerSetRole(newRole, reason, spawnFlags);

        /// <summary>
        /// Creates a role from <see cref="RoleTypeId"/> and <see cref="Player"/>.
        /// </summary>
        /// <param name="role">The <see cref="PlayerRoleBase"/>.</param>
        /// <returns>The created <see cref="Role"/> instance.</returns>
        internal static Role Create(PlayerRoleBase role) => role switch
        {
            Scp049GameRole scp049Role => new Scp049Role(scp049Role),
            ZombieRole scp0492Role => new Scp0492Role(scp0492Role),
            Scp079GameRole scp079Role => new Scp079Role(scp079Role),
            Scp096GameRole scp096Role => new Scp096Role(scp096Role),
            Scp106GameRole scp106Role => new Scp106Role(scp106Role),
            Scp173GameRole scp173Role => new Scp173Role(scp173Role),
            Scp3114GameRole scp3114Role => new Scp3114Role(scp3114Role),
            Scp939GameRole scp939Role => new Scp939Role(scp939Role),
            OverwatchGameRole overwatchRole => new OverwatchRole(overwatchRole),
            SpectatorGameRole spectatorRole => new SpectatorRole(spectatorRole),
            HumanGameRole humanRole => new HumanRole(humanRole),
            FilmmakerGameRole filmmakerRole => new FilmMakerRole(filmmakerRole),
            DestroyedGameRole destroyedRole => new DestroyedRole(destroyedRole),
            _ => new NoneRole(role),
        };
    }
}
