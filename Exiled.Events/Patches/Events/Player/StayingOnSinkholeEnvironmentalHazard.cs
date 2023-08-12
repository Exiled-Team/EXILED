// -----------------------------------------------------------------------
// <copyright file="StayingOnSinkholeEnvironmentalHazard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using HarmonyLib;

    using Hazards;

    /// <summary>
    /// Patches <see cref="SinkholeEnvironmentalHazard"/>.
    /// <br>Adds the <see cref="Handlers.Player.StayingOnEnvironmentalHazard"/> event.</br>
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.StayingOnEnvironmentalHazard))]
    [HarmonyPatch(typeof(SinkholeEnvironmentalHazard), nameof(SinkholeEnvironmentalHazard.OnStay))]
    internal static class StayingOnSinkholeEnvironmentalHazard
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(0, StayingOnEnvironmentalHazard.GetInstructions(ret));

            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}