// -----------------------------------------------------------------------
// <copyright file="SelectingRespawnTeam.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using System.Collections.Generic;
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
            LocalBuilder ev = generator.DeclareLocal(typeof(SelectingRespawnTeamEventArgs));

            const int offset = 1;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Stloc_1) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // SelectingRespawnTeamEventArgs ev = new(dominatingTeam);
                new CodeInstruction(OpCodes.Ldloc_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SelectingRespawnTeamEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev),

                // Handlers.Server.OnSelectingRespawnTeam(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnSelectingRespawnTeam))),

                // dominatingTeam = ev.Team;
                new(OpCodes.Ldloc, ev),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SelectingRespawnTeamEventArgs), nameof(SelectingRespawnTeamEventArgs.Team))),
                new(OpCodes.Stloc_1),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}