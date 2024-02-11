// -----------------------------------------------------------------------
// <copyright file="TeleportList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1402 // File may only contain a single type
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.API.Features.Core.Generic.Pools;
    using HarmonyLib;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PocketDimensionGenerator.GenerateMap(int)"/>.
    /// </summary>
    [HarmonyPatch(typeof(PocketDimensionGenerator), nameof(PocketDimensionGenerator.GenerateMap))]
    internal class TeleportList
    {
        private static void Postfix()
        {
            Map.TeleportsValue.Clear();
            Map.TeleportsValue.AddRange(Object.FindObjectsOfType<PocketDimensionTeleport>());
        }
    }

    /// <summary>
    /// Patches <see cref="PocketDimensionGenerator.PrepTeleports()"/>.
    /// </summary>
    [HarmonyPatch(typeof(PocketDimensionGenerator), nameof(PocketDimensionGenerator.PrepTeleports))]
    internal class OptimiseNWPocketDimensionGenerator
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // replace "PocketDimensionTeleport[] array = UnityEngine.Object.FindObjectsOfType<PocketDimensionTeleport>();"
            // with
            // replace "PocketDimensionTeleport[] array = Exiled.API.Features.Map.TeleportsValue"
            int offset = 0;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Call && instruction.operand == (object)Method(typeof(Object), nameof(Object.FindObjectsOfType))) + offset;
            newInstructions[index] = new(OpCodes.Call, PropertyGetter(typeof(Map), nameof(Map.PocketDimensionTeleports)));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}