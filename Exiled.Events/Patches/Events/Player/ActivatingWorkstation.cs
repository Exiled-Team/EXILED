// -----------------------------------------------------------------------
// <copyright file="ActivatingWorkstation.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="WorkStation.ConnectTablet(UnityEngine.GameObject)"/>.
    /// Adds the <see cref="Handlers.Player.ActivatingWorkstation"/> event.
    /// </summary>
    [HarmonyPatch(typeof(WorkStation), nameof(WorkStation.ConnectTablet))]
    internal static class ActivatingWorkstation
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            const int offset = 0;

            // Search for the last "ldloc.0".
            var index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_0) + offset;

            // The leave index offset.
            const int leaveIndexOffset = 0;

            // Search for the first "leave.s".
            var leaveIndex = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Leave_S) + leaveIndexOffset;

            // Define the return label and add it to the "leave,s" instruction.
            var returnLabel = newInstructions[leaveIndex].WithLabels(generator.DefineLabel()).labels[0];

            // var ev = new ActivatingWorkstationEventArgs(Player.Get(tabletOwner), this, true);
            //
            // Handlers.Player.OnActivatingWorkstation(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(tabletOwner)
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // this
                new CodeInstruction(OpCodes.Ldarg_0),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new ActivatingWorkstationEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingWorkstationEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Handlers.Player.OnActivatingWorkstation(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnActivatingWorkstation))),

                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingWorkstationEventArgs), nameof(ActivatingWorkstationEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            for (var z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
