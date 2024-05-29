// -----------------------------------------------------------------------
// <copyright file="OptimiseNWPocketDimensionGenerator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.API.Features.Pools;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

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
            // replace "PocketDimensionTeleport[] array = Exiled.API.Features.Map.TeleportsValue.ToArray()"
            newInstructions.RemoveAt(0);
            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Call, PropertyGetter(typeof(Map), nameof(Map.PocketDimensionTeleports))),
                new(OpCodes.Call, Method(typeof(Enumerable), nameof(Enumerable.ToArray)).MakeGenericMethod(typeof(PocketDimensionTeleport))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}