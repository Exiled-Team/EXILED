// -----------------------------------------------------------------------
// <copyright file="UsingMicroHIDEnergy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

    using InventorySystem.Items.MicroHID;

    using NorthwoodLib.Pools;

    using Exiled.Events.EventArgs;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="MicroHIDItem.ExecuteServerside"/>.
    /// Adds the <see cref="Handlers.Player.OnUsingMicroHIDEnergy"/> event.
    /// </summary>
    [HarmonyPatch(typeof(MicroHIDItem), nameof(MicroHIDItem.ExecuteServerside))]
    internal static class UsingMicroHIDEnergy
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            var offset = -19;

            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Call &&
            (MethodInfo)instruction.operand == Method(typeof(Mathf), nameof(Mathf.Clamp01))) + offset;

            var returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            var ev = generator.DeclareLocal(typeof(UsingMicroHIDEnergyEventArgs));

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(base.Owner)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(MicroHIDItem), nameof(MicroHIDItem.Owner))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // this
                new CodeInstruction(OpCodes.Ldarg_0),

                // currentState
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(MicroHIDItem), nameof(MicroHIDItem.State))),

                // num
                new CodeInstruction(OpCodes.Ldloc_2),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new UsingMicroHIDEnergyEventArgs(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingMicroHIDEnergyEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.UsingMicroHIDEnergy(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingMicroHIDEnergy))),

                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UsingMicroHIDEnergyEventArgs), nameof(UsingMicroHIDEnergyEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),

                // num = ev.Drain
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UsingMicroHIDEnergyEventArgs), nameof(UsingMicroHIDEnergyEventArgs.Drain))),
                new CodeInstruction(OpCodes.Stloc_2),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
