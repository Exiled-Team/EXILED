// -----------------------------------------------------------------------
// <copyright file="Left.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CustomNetworkManager.OnServerDisconnect(NetworkConnectionToClient)" />.
    /// Adds the <see cref="Handlers.Player.Left" /> event.
    /// </summary>
    [HarmonyPatch(typeof(CustomNetworkManager), nameof(CustomNetworkManager.OnServerDisconnect), typeof(NetworkConnectionToClient))]
    internal static class Left
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Left), nameof(HandleDisconnection))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void HandleDisconnection(NetworkConnection connection)
        {
            if (connection.identity == null)
                return;

            if (!ReferenceHub.TryGetHubNetID(connection.identity.netId, out ReferenceHub referenceHub))
                return;

            Player player = Player.Get(referenceHub);

            if (player == null || player == Server.Host)
                return;

            Log.SendRaw($"Player {player.Nickname} disconnected", ConsoleColor.Green);

            LeftEventArgs ev = new(player);

            Handlers.Player.OnLeft(ev);
        }
    }
}