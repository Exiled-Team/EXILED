// -----------------------------------------------------------------------
// <copyright file="SpawningRagdoll.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="RagdollManager.SpawnRagdoll(Vector3, Quaternion, Vector3, int, PlayerStats.HitInfo, bool, string, string, int)"/>.
    /// Adds the <see cref="Player.SpawningRagdoll"/> event.
    /// </summary>
    [HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.SpawnRagdoll))]
    internal class SpawningRagdoll
    {
        private static bool Prefix(RagdollManager __instance, ref Vector3 pos, ref Quaternion rot, ref int classId, ref PlayerStats.HitInfo ragdollInfo, ref bool allowRecall, ref string ownerID, ref string ownerNick, ref int playerId)
        {
            try
            {
                var ev = new SpawningRagdollEventArgs(
                    ragdollInfo.PlayerId == 0 ? null : API.Features.Player.Get(ragdollInfo.PlayerId), API.Features.Player.Get(__instance.gameObject), pos, rot, (RoleType)classId, ragdollInfo, allowRecall, ownerID, ownerNick, playerId);

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
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.SpawningRagdoll: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
