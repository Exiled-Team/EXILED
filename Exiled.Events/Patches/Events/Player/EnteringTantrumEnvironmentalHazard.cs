// -----------------------------------------------------------------------
// <copyright file="EnteringTantrumEnvironmentalHazard.cs" company="Exiled Team">
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

    /// <summary>
    /// Patches <see cref="TantrumEnvironmentalHazard.OnEnter(ReferenceHub)"/>.
    /// Adds the <see cref="Handlers.Player.EnteringEnvironmentalHazard"/> event.
    /// </summary>
    [HarmonyPatch(typeof(TantrumEnvironmentalHazard), nameof(TantrumEnvironmentalHazard.OnEnter))]
    internal static class EnteringTantrumEnvironmentalHazard
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();

            int offset = -2;
            int index = newInstructions.FindIndex(i => i.Calls(Method(typeof(EnvironmentalHazard), nameof(EnvironmentalHazard.OnEnter)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(player)
                    new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // this
                    new(OpCodes.Ldarg_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // EnteringEnvironmentalHazardEventArgs ev = new(Player, EnvironmentalHazard, true)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(EnteringEnvironmentalHazardEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnEnteringEnvironmentalHazard(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnEnteringEnvironmentalHazard))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(EnteringEnvironmentalHazardEventArgs), nameof(EnteringEnvironmentalHazardEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}