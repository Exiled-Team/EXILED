// -----------------------------------------------------------------------
// <copyright file="DeactivatingWorkstation.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="WorkStation.UnconnectTablet(GameObject)"/>.
    /// Adds the <see cref="Handlers.Player.ActivatingWorkstation"/> event.
    /// </summary>
    [HarmonyPatch(typeof(WorkStation), nameof(WorkStation.UnconnectTablet))]
    internal static class DeactivatingWorkstation
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 0;

            // Search for the last "ldarg.1".
            var index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldarg_1) + offset;

            // Define the return label and add it to the last "ret" instruction.
            var returnLabel = newInstructions[newInstructions.Count - 1].WithLabels(generator.DefineLabel()).labels[0];

            // var ev = new DeactivatingWorkstationEventArgs(Player.Get(taker), this, true);
            //
            // Handlers.Player.OnDeactivatingWorkstation(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(taker)
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // this
                new CodeInstruction(OpCodes.Ldarg_0),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new DeactivatingWorkstationEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(DeactivatingWorkstationEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Handlers.Player.OnDeactivatingWorkstation(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDeactivatingWorkstation))),

                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DeactivatingWorkstationEventArgs), nameof(DeactivatingWorkstationEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            for (var z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
