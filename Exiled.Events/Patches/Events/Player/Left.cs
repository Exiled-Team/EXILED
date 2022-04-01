// -----------------------------------------------------------------------
// <copyright file="Left.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player {
#pragma warning disable SA1313
    using System;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Mirror;

    /// <summary>
    /// Patches <see cref="CustomNetworkManager.OnServerDisconnect(Mirror.NetworkConnection)"/>.
    /// Adds the <see cref="Handlers.Player.Left"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CustomNetworkManager), nameof(CustomNetworkManager.OnServerDisconnect), new[] { typeof(NetworkConnection) })]
    internal static class Left {
        private static void Prefix(NetworkConnection conn) {
            try {
                // The game checks for null NetworkIdentity, do the same
                // GameObjects don't support the null-conditional operator (?) and the null-coalescing operator (??)
                if (conn.identity == null || conn.identity.gameObject == null)
                    return;

                Player player = Player.Get(conn.identity.gameObject);
                if (player == null || player.IsHost)
                    return;

                Log.SendRaw($"Player {player.Nickname} ({player.UserId}) ({player.Id}) disconnected", ConsoleColor.Green);
                Handlers.Player.OnLeft(new LeftEventArgs(player));
            }
            catch (Exception exception) {
                Log.Error($"{typeof(Left).FullName}.{nameof(Prefix)}:\n{exception}");
            }
        }
    }
}
