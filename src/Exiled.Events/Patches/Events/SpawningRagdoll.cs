// -----------------------------------------------------------------------
// <copyright file="SpawningRagdoll.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="RagdollManager.SpawnRagdoll(Vector3, Quaternion, Vector3, int, PlayerStats.HitInfo, bool, string, string, int)"/>.
    /// Adds the <see cref="Player.SpawningRagdoll"/> event.
    /// </summary>
    [HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.SpawnRagdoll))]
    public class SpawningRagdoll
    {
        /// <summary>
        /// Prefix of <see cref="RagdollManager.SpawnRagdoll(Vector3, Quaternion, Vector3, int, PlayerStats.HitInfo, bool, string, string, int)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="RagdollManager"/> instance.</param>
        /// <param name="pos"><inheritdoc cref="SpawningRagdollEventArgs.Position"/></param>
        /// <param name="rot"><inheritdoc cref="SpawningRagdollEventArgs.Rotation"/></param>
        /// <param name="classId"><inheritdoc cref="SpawningRagdollEventArgs.RoleType"/></param>
        /// <param name="ragdollInfo"><inheritdoc cref="SpawningRagdollEventArgs.HitInformations"/></param>
        /// <param name="allowRecall"><inheritdoc cref="SpawningRagdollEventArgs.IsRecallAllowed"/></param>
        /// <param name="ownerID"><inheritdoc cref="SpawningRagdollEventArgs.DissonanceId"/></param>
        /// <param name="ownerNick"><inheritdoc cref="SpawningRagdollEventArgs.PlayerNickname"/></param>
        /// <param name="playerId"><inheritdoc cref="SpawningRagdollEventArgs.PlayerId"/></param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(RagdollManager __instance, ref Vector3 pos, ref Quaternion rot, ref int classId, ref PlayerStats.HitInfo ragdollInfo, ref bool allowRecall, ref string ownerID, ref string ownerNick, ref int playerId)
        {
            var ev = new SpawningRagdollEventArgs(
                ragdollInfo.PlayerId == 0 ? null : API.Features.Player.Get(ragdollInfo.PlayerId),
                API.Features.Player.Get(__instance.gameObject),
                pos,
                rot,
                (RoleType)classId,
                ragdollInfo,
                allowRecall,
                ownerID,
                ownerNick,
                playerId);

            Player.OnSpawningRagdoll(ev);

            pos = ev.Position;
            rot = ev.Rotation;
            classId = (int)ev.RoleType;
            ragdollInfo = ev.HitInformations;
            allowRecall = ev.IsRecallAllowed;
            ownerID = ev.DissonanceId;
            ownerNick = ev.PlayerNickname;
            playerId = ev.PlayerId;

            return ev.IsAllowed;
        }
    }
}
