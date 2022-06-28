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

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Mirror;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="CustomNetworkManager.OnServerDisconnect(NetworkConnection)"/>.
    /// Adds the <see cref="Handlers.Player.Left"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CustomNetworkManager), nameof(CustomNetworkManager.OnServerDisconnect), typeof(NetworkConnection))]
    internal static class Left
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder netIdentity = generator.DeclareLocal(typeof(NetworkIdentity));
            LocalBuilder hub = generator.DeclareLocal(typeof(ReferenceHub));

            Label cdc = generator.DefineLabel();

            newInstructions[0].labels.Add(cdc);

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(NetworkConnection), nameof(NetworkConnection.identity))),
                new(OpCodes.Brfalse_S, cdc),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(NetworkConnection), nameof(NetworkConnection.identity))),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, netIdentity.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(NetworkIdentity), nameof(NetworkIdentity.netId))),
                new(OpCodes.Ldloca_S, hub.LocalIndex),
                new(OpCodes.Call, Method(typeof(ReferenceHub), nameof(ReferenceHub.TryGetHubNetID))),
                new(OpCodes.Brfalse_S, cdc),
                new(OpCodes.Ldloc_S, hub.LocalIndex),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),
                new(OpCodes.Brfalse_S, cdc),
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsHost))),
                new(OpCodes.Brtrue_S, cdc),
                new(OpCodes.Ldstr, "Player {0} disconnected"),
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Nickname))),
                new(OpCodes.Call, Method(typeof(string), nameof(string.Format), new[] { typeof(string), typeof(object) })),
                new(OpCodes.Ldc_I4_S, 10),
                new(OpCodes.Call, Method(typeof(Log), nameof(Log.SendRaw), new[] { typeof(string), typeof(ConsoleColor) })),
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(LeftEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnLeft))),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, player.LocalIndex),
                new(OpCodes.Call, Method(typeof(Left), nameof(Left.ForceCleanPlayerInfo))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void ForceCleanPlayerInfo(Player player)
        {
            if (Player.InstantiatedRolesToPlayers.TryGetValue(player.Role, out HashSet<Player> previousRolePlayers))
            {
                previousRolePlayers.Remove(player);
            }

            if (Player.InstantiatedTeamToPlayers.TryGetValue(player.Role.Team, out HashSet<Player> previousTeamPlayers))
            {
                previousTeamPlayers.Remove(player);
            }

            if (Player.InstantiatedSideToPlayers.TryGetValue(player.Role.Side, out HashSet<Player> previousSidePlayers))
            {
                previousSidePlayers.Remove(player);
            }
        }
    }
}
