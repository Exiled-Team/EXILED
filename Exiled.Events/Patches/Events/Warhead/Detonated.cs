// -----------------------------------------------------------------------
// <copyright file="Detonated.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Warhead
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="AlphaWarheadController.Detonate"/>.
    /// Adds the WarheadDetonated event.
    /// </summary>
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Detonate))]
    internal static class Detonated
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // Warhead.OnDetonated();
            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Call, Method(typeof(Warhead), nameof(Warhead.OnDetonated))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
