// -----------------------------------------------------------------------
// <copyright file="TargetOffset.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;

    using HarmonyLib;
    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="RoundSummary.SerializeSyncVars(NetworkWriter, bool)" />.
    /// </summary>
    [HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.SerializeSyncVars), typeof(NetworkWriter), typeof(bool))]
    internal static class TargetOffset
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            static bool IsField(CodeInstruction instruction) => instruction.opcode == OpCodes.Ldfld
                && (FieldInfo)instruction.operand == Field(typeof(RoundSummary), nameof(RoundSummary._chaosTargetCount));

            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            CodeInstruction[] addInstructions = new CodeInstruction[]
                {
                    new(OpCodes.Call, PropertyGetter(typeof(Round), nameof(Round.TargetOffset))),
                    new(OpCodes.Add),
                };

            int offset = 1;
            int index = newInstructions.FindIndex(IsField) + offset;
            newInstructions.InsertRange(index, addInstructions);

            index = newInstructions.FindLastIndex(IsField) + offset;
            newInstructions.InsertRange(index, addInstructions);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
