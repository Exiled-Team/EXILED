// -----------------------------------------------------------------------
// <copyright file="PlayerCurrentBroadcast.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using HarmonyLib;
    using MEC;
    using Mirror;

    /// <summary>
    /// Patches <see cref="Server.Broadcast.TargetAddElement"/>.
    /// </summary>
    [HarmonyPatch(typeof(Server), nameof(Server.Broadcast.TargetAddElement))]
    internal static class PlayerCurrentBroadcast
    {
        private static void Postfix(NetworkConnection conn, string data, ushort time, global::Broadcast.BroadcastFlags flags)
        {
            Player player = Player.Get(conn);

            if (player is null)
                return;

            if (player.BroadcastCoroutine.Value.IsRunning)
                Timing.KillCoroutines(player.BroadcastCoroutine.Value);

            player.BroadcastCoroutine = new KeyValuePair<Broadcast, CoroutineHandle>(new Broadcast(data, time, type: flags), Timing.RunCoroutine(ResetPlayerBroadcast(player, time)));
        }

        private static IEnumerator<float> ResetPlayerBroadcast(Player player, float duration)
        {
            yield return Timing.WaitForSeconds(duration);

            if (!player.IsConnected)
                yield break;

            player.BroadcastCoroutine = new();
        }
    }
}
