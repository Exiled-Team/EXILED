// -----------------------------------------------------------------------
// <copyright file="Role.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features.Roles
{
    using Exiled.API.Extensions;

    /// <summary>
    /// Defines the class for role-related classes.
    /// </summary>
    public abstract class Role
    {
#pragma warning disable 1584
        /// <summary>
        /// Gets the player this role is referring to.
        /// </summary>
        public abstract Player Owner { get; }

        /// <summary>
        /// Gets or sets the <see cref="RoleType"/> of this player.
        /// </summary>
        public RoleType Type
        {
            get => RoleType;
            set => Owner?.SetRole(value);
        }

        /// <summary>
        /// Gets the <see cref="global::Team"/> of this role.
        /// </summary>
        public Team Team => Type.GetTeam();

        /// <summary>
        /// Gets the <see cref="Enums.Side"/> of this role.
        /// </summary>
        public Enums.Side Side => Type.GetSide();

        /// <summary>
        /// Gets the <see cref="UnityEngine.Color"/> of this role.
        /// </summary>
        public UnityEngine.Color Color => Type.GetColor();

        /// <summary>
        /// Gets a value indicating whether or not this role is still valid. This will only ever be <see langword="false"/> if the Role is stored and accessed at a later date.
        /// </summary>
        public bool IsValid => Type == Owner.ReferenceHub.characterClassManager.NetworkCurClass;

        /// <summary>
        /// Gets the RoleType belonging to this role.
        /// </summary>
        internal abstract RoleType RoleType { get; }

        /// <summary>
        /// Converts a role to its appropriate <see cref="global::RoleType"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        public static implicit operator RoleType(Role role) => role?.Type ?? RoleType.None;

        /// <summary>
        /// Returns whether or not 2 roles are the same.
        /// </summary>
        /// <param name="role1">The role.</param>
        /// <param name="role2">The other role.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(Role role1, Role role2)
        {
            if (role1 is null)
                return role2 is null;
            return role1.Equals(role2);
        }

        /// <summary>
        /// Returns whether or not the two roles are different.
        /// </summary>
        /// <param name="role1">The role.</param>
        /// <param name="role2">The other role.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(Role role1, Role role2)
        {
            if (role1 is null)
                return !(role2 is null);
            return !role1.Equals(role2);
        }

        /// <summary>
        /// Returns whether or not the role has the same RoleType as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="type">The <see cref="global::RoleType"/>.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(Role role, RoleType type)
            => role?.Type == type;

        /// <summary>
        /// Returns whether or not the role has a different RoleType as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="type">The <see cref="global::RoleType"/>.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(Role role, RoleType type)
            => role?.Type != type;

        /// <summary>
        /// Returns whether or not the role has the same RoleType as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="global::RoleType"/>.</param>
        /// <param name="role">The role.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(RoleType type, Role role)
            => role?.Type == type;

        /// <summary>
        /// Returns whether or not the role has a different RoleType as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="global::RoleType"/>.</param>
        /// <param name="role">The role.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(RoleType type, Role role)
            => role?.Type != type;

        /// <summary>
        /// Casts the role to the specified role type.
        /// </summary>
        /// <typeparam name="T">The type of the class.</typeparam>
        /// <returns>The casted class, if possible.</returns>
        public T As<T>()
            where T : Role => this as T;

        /// <summary>
        /// Safely casts the role to the specified role type.
        /// </summary>
        /// <typeparam name="T">The type of the class.</typeparam>
        /// <param name="role">The casted class, if possible.</param>
        /// <returns><see langword="true"/> if the cast was successful; otherwise, <see langword="false"/>.</returns>
        public bool Is<T>(out T role)
            where T : Role
        {
            role = this is T t ? t : null;
            return this is T;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => base.Equals(obj);

        /// <inheritdoc/>
        public override string ToString() => Type.ToString();

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        /// <summary>
        /// Creates a role from RoleType and Player.
        /// </summary>
        /// <param name="type">The RoleType.</param>
        /// <param name="player">The Player.</param>
        /// <returns>A role.</returns>
        internal static Role Create(RoleType type, Player player) => type switch
        {
            RoleType.Scp049 => new Scp049Role(player),
            RoleType.Scp0492 => new Scp0492Role(player),
            RoleType.Scp079 => new Scp079Role(player),
            RoleType.Scp096 => new Scp096Role(player),
            RoleType.Scp106 => new Scp106Role(player),
            RoleType.Scp173 => new Scp173Role(player),
            RoleType.Scp93953 or RoleType.Scp93989 => new Scp939Role(player, type),
            RoleType.Spectator => new SpectatorRole(player),
            RoleType.None => new NoneRole(player),
            _ => new HumanRole(player, type),
        };
    }
}
