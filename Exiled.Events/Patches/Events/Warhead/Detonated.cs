// -----------------------------------------------------------------------
// <copyright file="Detonated.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Handlers;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="AlphaWarheadController.Detonate" />.
    ///     Adds the WarheadDetonated event.
    /// </summary>
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Detonate))]
    internal static class Detonated
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(AlphaWarheadController), nameof(AlphaWarheadController.RpcShake)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Warhead.OnDetonated();
                    new CodeInstruction(OpCodes.Call, Method(typeof(Warhead), nameof(Warhead.OnDetonated))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}