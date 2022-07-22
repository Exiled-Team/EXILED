// -----------------------------------------------------------------------
// <copyright file="SinkholeEffectFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.Patches.Events.Player;

    using HarmonyLib;
    using NorthwoodLib.Pools;

    /// <summary>
    /// Patches <see cref="SinkholeEnvironmentalHazard.OnEnter(ReferenceHub)"/>.
    /// Adds the better effect logic.
    /// </summary>
    /// <seealso cref="StayingOnSinkholeEnvironmentalHazard"/>
    /// <seealso cref="ExitingSinkholeEnvironmentalHazard"/>
    [HarmonyPatch(typeof(SinkholeEnvironmentalHazard), nameof(SinkholeEnvironmentalHazard.OnEnter))]
    internal static class SinkholeEffectFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Ldc_R4);

            newInstructions[index] = new CodeInstruction(OpCodes.Ldc_R4, 0.0f);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
