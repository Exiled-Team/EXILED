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

    using API.Features.Pools;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Hazards;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1600 // Elements should be documented

    /// <summary>
    /// Patches <see cref="EnvironmentalHazard.OnStay(ReferenceHub)"/>.
    /// Adds the <see cref="Handlers.Player.StayingOnEnvironmentalHazard"/> event.
    /// </summary>
    [HarmonyPatch(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.OnStay))]
    internal static class StayingOnEnvironmentalHazard
    {
        internal static CodeInstruction[] GetInstructions(Label ret) => new CodeInstruction[]
        {
            // Player.Get(player)
            new(OpCodes.Ldarg_1),
            new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

            // this
            new(OpCodes.Ldarg_0),

            // StayingOnEnvironmentalHazardEventArgs ev = new(Player, EnvironmentalHazard)
            new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StayingOnEnvironmentalHazardEventArgs))[0]),

            // Handlers.Player.OnStayingOnEnvironmentalHazard(ev)
            new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnStayingOnEnvironmentalHazard))),
        };

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(0, GetInstructions(ret));

            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}