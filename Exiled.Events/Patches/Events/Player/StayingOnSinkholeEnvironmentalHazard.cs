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

    using Exiled.Events.Patches.Fixes;
    using Exiled.Events.Attributes;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    /// <summary>
    /// Patches <see cref="SinkholeEnvironmentalHazard"/>.
    /// <br>Adds the <see cref="Handlers.Player.StayingOnEnvironmentalHazard"/> event.</br>
    /// <br>Adds the better effect logic.</br>
    /// </summary>
    /// <seealso cref="SinkholeEffectFix"/>
    /// <seealso cref="ExitingSinkholeEnvironmentalHazard"/>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.StayingOnEnvironmentalHazard))]
    [HarmonyPatch(typeof(SinkholeEnvironmentalHazard), nameof(SinkholeEnvironmentalHazard.OnStay))]
    internal static class StayingOnSinkholeEnvironmentalHazard
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label ret = generator.DefineLabel();

            newInstructions.Clear();

            newInstructions.AddRange(StayingOnEnvironmentalHazard.GetInstructions(ret));
            newInstructions.Add(new CodeInstruction(OpCodes.Ret).WithLabels(ret));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
