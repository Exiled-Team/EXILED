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
        /// <summary>
        /// Gets the player this role is referring to.
        /// </summary>
        public abstract Player Player { get; }

        /// <summary>
        /// Gets the <see cref="global::RoleType"/> of this role.
        /// </summary>
        public abstract RoleType RoleType { get; }

        /// <summary>
        /// Gets the <see cref="global::Team"/> of this role.
        /// </summary>
        public Team Team => RoleType.GetTeam();

        /// <summary>
        /// Gets the <see cref="Enums.Side"/> of this role.
        /// </summary>
        public Enums.Side Side => RoleType.GetSide();

        /// <summary>
        /// Gets the <see cref="UnityEngine.Color"/> of this role.
        /// </summary>
        public UnityEngine.Color Color => RoleType.GetColor();

        /// <summary>
        /// Gets a value indicating whether or not this role is still valid. This will only ever be <see langword="false"/> if the Role is stored and accessed at a later date.
        /// </summary>
        public bool IsValid => RoleType == Player.ReferenceHub.characterClassManager.NetworkCurClass;

        /// <summary>
        /// Converts a role to its appropriate <see cref="global::RoleType"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        public static implicit operator RoleType(Role role) => role.RoleType;

        /// <summary>
        /// Returns whether or not the role has the same RoleType as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="type">The <see cref="global::RoleType"/>.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(Role role, RoleType type)
            => role.RoleType == type;

        /// <summary>
        /// Returns whether or not the role has a different RoleType as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="type">The <see cref="global::RoleType"/>.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(Role role, RoleType type)
            => role.RoleType != type;

        /// <summary>
        /// Returns whether or not the role has the same RoleType as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="global::RoleType"/>.</param>
        /// <param name="role">The role.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(RoleType type, Role role)
            => role.RoleType == type;

        /// <summary>
        /// Returns whether or not the role has a different RoleType as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="global::RoleType"/>.</param>
        /// <param name="role">The role.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(RoleType type, Role role)
            => role.RoleType != type;

        /// <summary>
        /// Casts the role to the specified role class.
        /// </summary>
        /// <typeparam name="T">The type of the class.</typeparam>
        /// <returns>The casted class, if possible.</returns>
        public T As<T>()
            where T : Role
            => this as T;

        /// <inheritdoc/>
        public override bool Equals(object obj) => base.Equals(obj);

        /// <inheritdoc/>
        public override string ToString() => RoleType.ToString();

        /// <inheritdoc/>
        public override int GetHashCode() => base.GetHashCode();

        internal static Role Create(RoleType type, Player player)
        {
            switch (type)
            {
                case RoleType.Scp079:
                    return new Scp079Role(player);
                case RoleType.Scp049:
                    return new Scp049Role(player);
                case RoleType.Spectator:
                    return new SpectatorRole(player);
                default:
                    return new HumanRole(player);
            }
        }
    }
}
