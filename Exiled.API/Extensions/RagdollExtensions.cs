// -----------------------------------------------------------------------
// <copyright file="RagdollExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// A set of extensions for <see cref="Ragdoll"/>.
    /// </summary>
    public static class RagdollExtensions
    {
        /// <summary>
        /// Finds and returns the <see cref="Room"/> the ragdoll is located in.
        /// </summary>
        /// <param name="ragdoll">The <see cref="Ragdoll"/> to check the room of.</param>
        /// <returns>The <see cref="Room"/> the ragdoll is located in.</returns>
        [Obsolete("Use Ragdoll.Room property instead.")]
        public static Room GetRoom(this Ragdoll ragdoll) => ragdoll.Room;

        /// <summary>
        /// Returns the <see cref="RoleType"/> of the ragdoll.
        /// </summary>
        /// <param name="ragdoll">The <see cref="Ragdoll"/> to check the role of.</param>
        /// <returns>The <see cref="RoleType"/> of the ragdoll.</returns>
        [Obsolete("Use Ragdoll.Role property instead.")]
        public static RoleType GetRole(this Ragdoll ragdoll) => ragdoll.Role;

        /// <summary>
        /// Returns the owner <see cref="Player"/>, or null if the ragdoll does not have an owner.
        /// </summary>
        /// <param name="ragdoll">The <see cref="Ragdoll"/> to get the owner of.</param>
        /// <returns>The owner of the ragdoll, or null if the ragdoll does not have an owner.</returns>
        [Obsolete("Use Ragdoll.Owner property instead.")]
        public static Player GetOwner(this Ragdoll ragdoll) => ragdoll.Owner;

        /// <summary>
        /// Returns the killing <see cref="Player"/>, or null if the killer is not a player.
        /// </summary>
        /// <param name="ragdoll">The <see cref="Ragdoll"/> to get the killer of.</param>
        /// <returns>The killing <see cref="Player"/>, or null if the killer is not a player.</returns>
        [Obsolete("Use Ragdoll.Killer property instead.")]
        public static Player GetKiller(this Ragdoll ragdoll) => ragdoll.Killer;
    }
}
