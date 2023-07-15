// -----------------------------------------------------------------------
// <copyright file="HazardListAdd.cs" company="Exiled Team">
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

    /// <summary>
    /// Patches <see cref="EnvironmentalHazard.Start()"/>.
    /// </summary>
    [HarmonyPatch(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.Start))]
    internal class HazardListAdd
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new CodeInstruction[]
                {
                    // new Hazard(this)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(Hazard))[0]),
                    new(OpCodes.Pop),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}