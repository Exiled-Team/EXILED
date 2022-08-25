// -----------------------------------------------------------------------
// <copyright file="UsingMicroHIDEnergy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.MicroHID;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="MicroHIDItem.ExecuteServerside" />.
    ///     Adds the <see cref="Handlers.Player.OnUsingMicroHIDEnergy" /> event.
    /// </summary>
    [HarmonyPatch(typeof(MicroHIDItem), nameof(MicroHIDItem.ExecuteServerside))]
    internal static class UsingMicroHIDEnergy
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = -7;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Call &&
                                                                 (MethodInfo)instruction.operand == Method(typeof(Mathf), nameof(Mathf.Clamp01))) + offset;

            Label returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(base.Owner)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(MicroHIDItem), nameof(MicroHIDItem.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // this
                new(OpCodes.Ldarg_0),

                // currentState
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(MicroHIDItem), nameof(MicroHIDItem.State))),

                // num
                new(OpCodes.Ldloc_2),

                // true
                new(OpCodes.Ldc_I4_1),

                // var ev = new UsingMicroHIDEnergyEventArgs(...)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingMicroHIDEnergyEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),

                // Handlers.Player.UsingMicroHIDEnergy(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingMicroHIDEnergy))),

                // num = ev.Drain
                new(OpCodes.Call, PropertyGetter(typeof(UsingMicroHIDEnergyEventArgs), nameof(UsingMicroHIDEnergyEventArgs.Drain))),
                new(OpCodes.Stloc_2),

                // if (!ev.IsAllowed)
                //   return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(UsingMicroHIDEnergyEventArgs), nameof(UsingMicroHIDEnergyEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
