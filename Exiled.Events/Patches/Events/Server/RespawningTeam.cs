// -----------------------------------------------------------------------
// <copyright file="RespawningTeam.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Server;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using Respawning;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.API.Features.Player;

    /// <summary>
    ///     Patch the <see cref="RespawnManager.Spawn" />.
    ///     Adds the <see cref="Server.RespawningTeam" /> event.
    /// </summary>
    [EventPatch(typeof(Server), nameof(Server.RespawningTeam))]
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.Spawn))]
    internal static class RespawningTeam
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_3) + offset;
            LocalBuilder ev = generator.DeclareLocal(typeof(RespawningTeamEventArgs));
            Label continueLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // List<Player> players = GetPlayers(list1);
                new(OpCodes.Ldloc_1),
                new(OpCodes.Call, Method(typeof(RespawningTeam), nameof(GetPlayers))),

                // num
                new(OpCodes.Ldloc_3),

                // this.NextKnownTeam
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(RespawnManager), nameof(RespawnManager.NextKnownTeam))),
                new(OpCodes.Ldc_I4_1),

                // var ev = new RespawningTeamEventArgs(players, num, this.NextKnownTeam)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RespawningTeamEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Server.OnRespawningTeam(ev)
                new(OpCodes.Call, Method(typeof(Server), nameof(Server.OnRespawningTeam))),

                // if (!ev.IsAllowed)
                // {
                //    this.NextKnownTeam = SpawnableTeam.None;
                //    return;
                // }
                new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.IsAllowed))),
                new(OpCodes.Brtrue, continueLabel),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Stfld, Field(typeof(RespawnManager), nameof(RespawnManager.NextKnownTeam))),
                new(OpCodes.Ret),

                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex).WithLabels(continueLabel),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Dup),

                // num = ev.MaximumRespawnAmount
                new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.MaximumRespawnAmount))),
                new(OpCodes.Stloc_3),

                // spawnableTeamHandler = ev.SpawnableTeam
                new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.SpawnableTeam))),
                new(OpCodes.Stloc_0),

                // list1 = GetHubs(ev.Players)
                new(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.Players))),
                new(OpCodes.Call, Method(typeof(RespawningTeam), nameof(GetHubs))),
                new(OpCodes.Stloc_1),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static List<Player> GetPlayers(List<ReferenceHub> hubs)
        {
            return hubs.Select(Player.Get).ToList();
        }

        private static List<ReferenceHub> GetHubs(List<Player> players)
        {
            return players.Select(p => p.ReferenceHub).ToList();
        }
    }
}
