// -----------------------------------------------------------------------
// <copyright file="UpdateDecontaminationTime.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.NPCs
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Exiled.API.Extensions;
    using HarmonyLib;
    using LightContainmentZoneDecontamination;
    using NorthwoodLib.Pools;
    using UnityEngine;

#pragma warning disable SA1118
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="DecontaminationController.UpdateTime"/> to prevent the sending of networked subtitles to npcs.
    /// </summary>
    [HarmonyPatch(typeof(DecontaminationController), nameof(DecontaminationController.UpdateTime))]
    internal static class UpdateDecontaminationTime
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Br_S);
            Label continueLabel = (Label)newInstructions[index].operand;

            const int offset = 1;
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_3) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloca_S, 3),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(KeyValuePair<GameObject, ReferenceHub>), nameof(KeyValuePair<GameObject, ReferenceHub>.Key))),
                new CodeInstruction(OpCodes.Call, Method(typeof(NpcExtensions), nameof(NpcExtensions.IsNpc), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Brtrue_S, continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
