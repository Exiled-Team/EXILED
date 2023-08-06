// -----------------------------------------------------------------------
// <copyright file="HazardListRemove.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Hazards;
    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using Hazards;

    using TemporaryHazard = global::Hazards.TemporaryHazard;

    /// <summary>
    /// Patches <see cref="TemporaryHazard.ServerDestroy()"/> and <see cref="EnvironmentalHazard.OnDestroy()"/>.
    /// </summary>
    [HarmonyPatch]
    internal class HazardListRemove
    {
        private static List<CodeInstruction> InstructionsToAdd { get; } = new()
        {
            // Hazard.EnvironmentalHazardToHazard.Remove(this)
            new(OpCodes.Ldarg_0),
            new(OpCodes.Ldsfld, AccessTools.Field(typeof(Hazard), nameof(Hazard.EnvironmentalHazardToHazard))),
            new(OpCodes.Call, AccessTools.Method(typeof(Dictionary<EnvironmentalHazard, Hazard>), nameof(Dictionary<EnvironmentalHazard, Hazard>.Remove))),
            new(OpCodes.Pop),
        };

        [HarmonyPatch(typeof(TemporaryHazard), nameof(TemporaryHazard.ServerDestroy))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> TemporaryHazardDestroy(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(newInstructions.Count - 1, InstructionsToAdd);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        [HarmonyPatch(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.OnDestroy))]
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> HazardDestroy(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(newInstructions.Count - 1, InstructionsToAdd);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}