// -----------------------------------------------------------------------
// <copyright file="RespawningTeam.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using Respawning;

    using static HarmonyLib.AccessTools;

    using Server = Exiled.Events.Handlers.Server;

    /// <summary>
    /// Patch the <see cref="RespawnManager.Spawn"/>.
    /// Adds the <see cref="Server.RespawningTeam"/> event.
    /// </summary>
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
                new CodeInstruction(OpCodes.Ldloc_1).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, Method(typeof(RespawningTeam), nameof(GetPlayers))),

                // num
                new CodeInstruction(OpCodes.Ldloc_3),

                // this.NextKnownTeam
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(RespawnManager), nameof(RespawnManager.NextKnownTeam))),
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new RespawningTeamEventArgs(players, num, this.NextKnownTeam)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(RespawningTeamEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Server.OnRespawningTeam(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Server), nameof(Server.OnRespawningTeam))),

                // if (!ev.IsAllowed)
                // {
                //    this.NextKnownTeam = SpawnableTeam.None;
                //    return;
                // }
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brtrue, continueLabel),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stfld, Field(typeof(RespawnManager), nameof(RespawnManager.NextKnownTeam))),
                new CodeInstruction(OpCodes.Ret),

                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex).WithLabels(continueLabel),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),

                // num = ev.MaximumRespawnAmount
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.MaximumRespawnAmount))),
                new CodeInstruction(OpCodes.Stloc_3),

                // spawnableTeamHandler = ev.SpawnableTeam
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.SpawnableTeam))),
                new CodeInstruction(OpCodes.Stloc_0),

                // list1 = GetHubs(ev.Players)
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(RespawningTeamEventArgs), nameof(RespawningTeamEventArgs.Players))),
                new CodeInstruction(OpCodes.Call, Method(typeof(RespawningTeam), nameof(GetHubs))),
                new CodeInstruction(OpCodes.Stloc_1),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static List<Player> GetPlayers(List<ReferenceHub> hubs) => hubs.Select(Player.Get).ToList();

        private static List<ReferenceHub> GetHubs(List<Player> players) => players.Select(p => p.ReferenceHub).ToList();

        private static void LogThing(int i) => Log.Error(i);
    }
}
