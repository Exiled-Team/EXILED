// -----------------------------------------------------------------------
// <copyright file="NpcServerCountFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1402 // File may only contain a single type

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ServerConsole.RefreshOnlinePlayers"/> to prevent NPC's from appearing as real players in the server list count.
    /// </summary>
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.RefreshOnlinePlayers))]
    internal static class NpcServerCountFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 0;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldsflda) + offset;

            Label labelcontinue = (Label)newInstructions[index - 1].operand;

            // if (Player.Get(referenceHub).IsNPC)
            //  continue;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsNPC))),
                new(OpCodes.Brtrue_S, labelcontinue),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="ServerConsole.HandlePlayerJoin"/> to prevent NPC's from being send to NW Info.
    /// </summary>
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.HandlePlayerJoin))]
    internal static class NpcServerConsoleFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 0;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldsflda) + offset;

            Label ret = generator.DefineLabel();

            // if (Player.Get(referenceHub).IsNPC)
            //  return;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_1),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.IsNPC))),
                new(OpCodes.Brtrue_S, ret),
            });

            newInstructions[index].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}