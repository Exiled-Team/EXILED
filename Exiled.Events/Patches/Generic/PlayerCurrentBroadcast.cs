// -----------------------------------------------------------------------
// <copyright file="PlayerCurrentBroadcast.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using System.Collections.Generic;
    using System.Linq;
    using Exiled.API.Features;
    using HarmonyLib;

    using Hints;

    using MEC;
    using Mirror;

    using BaseBroadcast = Broadcast;

    /// <summary>
    /// Patches <see cref=""/>.
    /// </summary>
    internal static class PlayerCurrentBroadcast
    {
        private static void AddCurrentBroadcast(Player player, Broadcast broadcast)
        {
            // Check if player is null
            if (player == null)
                return;
            if (!player.CurrentBroadcastProccess.Key.IsRunning)
            {
                player.CurrentBroadcastProccess = new(Timing.RunCoroutine(CurrentBroadcast(player, broadcast.Duration)), broadcast);
                return;
            }

            player.BroadcastsInQueue.Add(broadcast);
        }

        private static void ClearBroadcast(Player player)
        {
            // Check if player is null
            if (player == null)
                return;
            if (player.CurrentBroadcastProccess.Key.IsRunning)
            {
                player.CurrentBroadcastProccess = new();
                return;
            }

            player.BroadcastsInQueue.Clear();
        }

        private static IEnumerator<float> CurrentBroadcast(Player player, float duration)
        {
            // Waiting for the hint to end
            yield return Timing.WaitForSeconds(duration);

            // If player gameobject doesn't exists, break the coroutine
            if (!player.IsConnected)
                yield break;
            Broadcast broadcast = player.BroadcastsInQueue.FirstOrDefault();
            if (broadcast is null)
            {
                player.CurrentBroadcastProccess = new();
                yield break;
            }

            player.CurrentBroadcastProccess = new(Timing.RunCoroutine(CurrentBroadcast(player, broadcast.Duration)), broadcast);
            player.BroadcastsInQueue.RemoveAt(0);
        }

        [HarmonyPatch(typeof(BaseBroadcast), nameof(BaseBroadcast.RpcAddElement))]
        internal static class PlayerAddCurrentBroadcastEveryone
        {
            private static void Postfix(string data, ushort time, BaseBroadcast.BroadcastFlags flags)
            {
                foreach (Player player in Player.List)
                {
                    AddCurrentBroadcast(player, new Broadcast(data, time, true, flags));
                }
            }
        }

        [HarmonyPatch(typeof(BaseBroadcast), nameof(BaseBroadcast.RpcClearElements))]
        internal static class PlayerClearCurrentBroadcastEveryone
        {
            private static void Postfix()
            {
                foreach (Player player in Player.List)
                {
                    ClearBroadcast(player);
                }
            }
        }

        [HarmonyPatch(typeof(BaseBroadcast), nameof(BaseBroadcast.TargetAddElement))]
        internal static class PlayerAddCurrentBroadcast
        {
            private static void Postfix(NetworkConnection connection, string data, ushort time, BaseBroadcast.BroadcastFlags flags)
            {
                AddCurrentBroadcast(Player.Get(connection), new Broadcast(data, time, true, flags));
            }
        }

        [HarmonyPatch(typeof(BaseBroadcast), nameof(BaseBroadcast.TargetClearElements))]
        internal static class PlayerClearCurrentBroadcast
        {
            private static void Postfix(NetworkConnection connection)
            {
                ClearBroadcast(Player.Get(connection));
            }
        }
    }
}