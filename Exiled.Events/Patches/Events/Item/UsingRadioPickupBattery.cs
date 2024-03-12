// -----------------------------------------------------------------------
// <copyright file="UsingRadioPickupBattery.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Item
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Item;
    using HarmonyLib;
    using InventorySystem.Items.Radio;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RadioPickup.LateUpdate"/>
    /// to add <see cref="Handlers.Item.UsingRadioPickupBattery"/> event.
    /// </summary>
    [HarmonyPatch(typeof(RadioPickup), nameof(RadioPickup.LateUpdate))]
    internal class UsingRadioPickupBattery
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(UsingRadioPickupBatteryEventArgs));

            Label continueLabel = generator.DefineLabel();

            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldloc_1);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // this
                    new(OpCodes.Ldarg_0),

                    // num
                    new(OpCodes.Ldloc_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // UsingRadioPickupBatteryEventArgs ev = new(this, num, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingRadioPickupBatteryEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Item.OnUsingRadioPickupBattery(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Item), nameof(Handlers.Item.OnUsingRadioPickupBattery))),

                    // if (ev.IsAllowed)
                    //   goto continueLabel
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingRadioPickupBatteryEventArgs), nameof(UsingRadioPickupBatteryEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    // return
                    new(OpCodes.Ret),

                    // num = ev.Drain;
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingRadioPickupBatteryEventArgs), nameof(UsingRadioPickupBatteryEventArgs.Drain))),
                    new(OpCodes.Stloc_1),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}