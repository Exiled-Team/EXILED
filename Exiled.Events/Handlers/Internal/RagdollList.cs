// -----------------------------------------------------------------------
// <copyright file="RagdollList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using Exiled.API.Features;

    using PlayerRoles.Ragdolls;

    /// <summary>
    /// Handles adding and removing from <see cref="Ragdoll.BasicRagdollToRagdoll"/>.
    /// </summary>
    internal static class RagdollList
    {
        /// <summary>
        /// Called after a ragdoll is spawned. Hooked to <see cref="RagdollManager.OnRagdollSpawned"/>.
        /// </summary>
        /// <param name="ragdoll">The spawned ragdoll.</param>
        public static void OnSpawnedRagdoll(BasicRagdoll ragdoll) => Ragdoll.Get(ragdoll);

        /// <summary>
        /// Called before a ragdoll is destroyed. Hooked to <see cref="RagdollManager.OnRagdollRemoved"/>.
        /// </summary>
        /// <param name="ragdoll">The destroyed ragdoll.</param>
        public static void OnRemovedRagdoll(BasicRagdoll ragdoll) => Ragdoll.BasicRagdollToRagdoll.Remove(ragdoll);
    }
}
