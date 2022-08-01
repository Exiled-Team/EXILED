// -----------------------------------------------------------------------
// <copyright file="StayingOnEnvironmentalHazard.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1600 // Elements should be documented

    /// <summary>
    /// Patches <see cref="EnvironmentalHazard.OnStay(ReferenceHub)"/>.
    /// Adds the <see cref="Handlers.EnvironementalHazard.StayingOnEnvironmentalHazard"/> event.
    /// </summary>
    [HarmonyPatch(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.OnStay))]
    internal static class StayingOnEnvironmentalHazard
    {
        internal static CodeInstruction[] GetInstructions(Label ret) => new CodeInstruction[]
        {
            new(OpCodes.Ldarg_1),
            new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
            new(OpCodes.Ldarg_0),
            new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StayingOnEnvironmentalHazardEventArgs))[0]),
            new(OpCodes.Call, Method(typeof(Handlers.EnvironementalHazard), nameof(Handlers.EnvironementalHazard.OnStayingOnEnvironmentalHazard))),
        };

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(0, GetInstructions(ret));

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
