// -----------------------------------------------------------------------
// <copyright file="ExitingSinkholeEnvironmentalHazard.cs" company="Exiled Team">
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
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Hazards;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="SinkholeEnvironmentalHazard.OnExit(ReferenceHub)"/> with <see cref="SinkholeEnvironmentalHazard"/>.
    /// Adds the <see cref="Handlers.Player.ExitingEnvironmentalHazard"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ExitingEnvironmentalHazard))]
    [HarmonyPatch(typeof(SinkholeEnvironmentalHazard), nameof(SinkholeEnvironmentalHazard.OnExit))]
    internal static class ExitingSinkholeEnvironmentalHazard
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // Player.Get(player)
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // this
                    new(OpCodes.Ldarg_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ExitingEnvironmentalHazardEventArgs ev = new(Player, EnvironmentalHazard, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ExitingEnvironmentalHazardEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnExitingEnvironmentalHazard(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnExitingEnvironmentalHazard))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ExitingEnvironmentalHazardEventArgs), nameof(ExitingEnvironmentalHazardEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}