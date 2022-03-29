// -----------------------------------------------------------------------
// <copyright file="CustomReasonDamageHandlerFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using PlayerStatsSystem;

    /// <summary>
    /// Patches <see cref="CustomReasonDamageHandler(string, float, string)"/>.
    /// <br>Fixes a NullReferenceException caused by <see cref="Subtitles.SubtitlePart"/>[] initialization.</br>
    /// </summary>
    [HarmonyPatch(typeof(CustomReasonDamageHandler), MethodType.Constructor, typeof(string), typeof(float), typeof(string))]
    internal static class CustomReasonDamageHandlerFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldarg_0);

            newInstructions.Insert(index, new CodeInstruction(OpCodes.Ret));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
