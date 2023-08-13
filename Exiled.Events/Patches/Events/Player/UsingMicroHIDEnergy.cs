// -----------------------------------------------------------------------
// <copyright file="UsingMicroHIDEnergy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.MicroHID;

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
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(UsingMicroHIDEnergyEventArgs));

            const int offset = -7;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(Mathf), nameof(Mathf.Clamp01)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
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

                    // UsingMicroHIDEnergyEventArgs ev = new(Player, MicroHIDItem, HidState, float, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingMicroHIDEnergyEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.UsingMicroHIDEnergy(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingMicroHIDEnergy))),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingMicroHIDEnergyEventArgs), nameof(UsingMicroHIDEnergyEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // num = ev.Drain
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Call, PropertyGetter(typeof(UsingMicroHIDEnergyEventArgs), nameof(UsingMicroHIDEnergyEventArgs.Drain))),
                    new(OpCodes.Stloc_2),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}