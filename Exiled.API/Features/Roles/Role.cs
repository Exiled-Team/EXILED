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
        public abstract Player Owner { get; }

        /// <summary>
        /// Gets the <see cref="RoleType"/> of this role.
        /// </summary>
        public abstract RoleType Type { get; }

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
        /// Converts a role to its appropriate <see cref="global::RoleType"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        public static implicit operator RoleType(Role role) => role.Type;

        /// <summary>
        /// Returns whether or not the role has the same RoleType as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="type">The <see cref="global::RoleType"/>.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(Role role, RoleType type)
            => role.Type == type;

        /// <summary>
        /// Returns whether or not the role has a different RoleType as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="role">The role.</param>
        /// <param name="type">The <see cref="global::RoleType"/>.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(Role role, RoleType type)
            => role.Type != type;

        /// <summary>
        /// Returns whether or not the role has the same RoleType as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="global::RoleType"/>.</param>
        /// <param name="role">The role.</param>
        /// <returns><see langword="true"/> if the values are equal.</returns>
        public static bool operator ==(RoleType type, Role role)
            => role.Type == type;

        /// <summary>
        /// Returns whether or not the role has a different RoleType as the given <paramref name="type"/>.
        /// </summary>
        /// <param name="type">The <see cref="global::RoleType"/>.</param>
        /// <param name="role">The role.</param>
        /// <returns><see langword="true"/> if the values are not equal.</returns>
        public static bool operator !=(RoleType type, Role role)
            => role.Type != type;

        /// <summary>
        /// Casts the role to the specified role type.
        /// </summary>
        /// <typeparam name="T">The type of the class.</typeparam>
        /// <returns>The casted class, if possible.</returns>
        public T As<T>()
            where T : Role
            => this as T;

        /// <summary>
        /// Returns a value indicating whether or not this role can be casted to the specified role type.
        /// </summary>
        /// <param name="role">If the return value is <see langword="true"/>, this parameter will be the type of the role when casted. If the return value is <see langword="false"/>, this will be <see langword="null"/>.</param>
        /// <typeparam name="T">The type to check.</typeparam>
        /// <returns><see langword="true"/> if the cast is valid; otherwise, <see langword="false"/>.</returns>
        public bool Is<T>(out T role)
            where T : Role
        {
            role = As<T>();
            return role != null;
        }

        /// <inheritdoc/>
        public override bool Equals(object obj) => base.Equals(obj);

        /// <inheritdoc/>
        public override string ToString() => Type.ToString();

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
