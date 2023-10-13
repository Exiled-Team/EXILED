// -----------------------------------------------------------------------
// <copyright file="SelectingRespawnTeam.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Server;

    using HarmonyLib;

    using Respawning;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RespawnManager.Update"/> to add the <see cref="Handlers.Server.SelectingRespawnTeam"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Server), nameof(Handlers.Server.SelectingRespawnTeam))]
    [HarmonyPatch(typeof(RespawnManager), nameof(RespawnManager.Update))]
    internal static class SelectingRespawnTeam
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label skipReset = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(SelectingRespawnTeamEventArgs));

            const int offset = 2;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand == Method(typeof(RespawnManager), nameof(RespawnManager.RestartSequence))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // SelectingRespawnTeamEventArgs ev = new(dominatingTeam);
                new(OpCodes.Ldloc_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SelectingRespawnTeamEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev),

                // Handlers.Server.OnSelectingRespawnTeam(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnSelectingRespawnTeam))),

                // if (ev.ChosenTeam == SpawnableTeamType.None)
                // {
                //    this.RestartSequence();
                //    return;
                // }
                new(OpCodes.Ldloc_S, ev),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SelectingRespawnTeamEventArgs), nameof(SelectingRespawnTeamEventArgs.ChosenTeam))),
                new(OpCodes.Brfalse_S, skipReset),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(RespawnManager), nameof(RespawnManager.RestartSequence))),
                new(OpCodes.Ret),

                // dominatingTeam = ev.ChosenTeam;
                new(OpCodes.Ldloc_S, ev),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SelectingRespawnTeamEventArgs), nameof(SelectingRespawnTeamEventArgs.ChosenTeam))).WithLabels(skipReset),
                new(OpCodes.Stloc_1),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}