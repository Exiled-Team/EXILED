// -----------------------------------------------------------------------
// <copyright file="RagdollExtensions.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Extensions
{
    using System.Linq;

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
        public static Room GetRoom(this Ragdoll ragdoll) =>
            Map.FindParentRoom(ragdoll.gameObject);

        /// <summary>
        /// Returns the <see cref="RoleType"/> of the ragdoll.
        /// </summary>
        /// <param name="ragdoll">The <see cref="Ragdoll"/> to check the role of.</param>
        /// <returns>The <see cref="RoleType"/> of the ragdoll.</returns>
        public static RoleType GetRole(this Ragdoll ragdoll) =>
            CharacterClassManager._staticClasses.FirstOrDefault(role => role.fullName == ragdoll.owner.FullName).roleId;

        /// <summary>
        /// Returns the owner <see cref="Player"/>, or null if the ragdoll does not have an owner.
        /// </summary>
        /// <param name="ragdoll">The <see cref="Ragdoll"/> to get the owner of.</param>
        /// <returns>The owner of the ragdoll, or null if the ragdoll does not have an owner.</returns>
        public static Player GetOwner(this Ragdoll ragdoll) =>
            Player.Get(ragdoll.owner.PlayerId);

        /// <summary>
        /// Returns the killing <see cref="Player"/>, or null if the killer is not a player.
        /// </summary>
        /// <param name="ragdoll">The <see cref="Ragdoll"/> to get the killer of.</param>
        /// <returns>The killing <see cref="Player"/>, or null if the killer is not a player.</returns>
        public static Player GetKiller(this Ragdoll ragdoll) =>
            ragdoll.owner.DeathCause.IsPlayer ? Player.Get(ragdoll.owner.DeathCause.RHub) : null;
    }
}
