// -----------------------------------------------------------------------
// <copyright file="UsingRadioBattery.cs" company="Exiled Team">
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
    /// Patches <see cref="Radio.UseBattery"/>.
    /// Adds the <see cref="Handlers.Player.UsingRadioBattery"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Radio), nameof(Radio.UseBattery))]
    internal static class UsingRadioBattery
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            var offset = 0;

            // Search for the first "ldloc.0".
            var index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_0) + offset;

            // Get the return label of the last "ret".
            var returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            // Declare an "UsingRadioBatteryEventArgs" local variable.
            var ev = generator.DeclareLocal(typeof(UsingRadioBatteryEventArgs));

            // var ev = new UsingRadioBattery(this, Player.Get(base.GameObject), num);
            //
            // Handlers.Player.OnUsingRadioBattery(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            //
            // num = ev.Charge;
            //
            // this.inv.items.ModifyDuration(this.myRadio, num);
            //
            // return;
            newInstructions.InsertRange(index, new[]
            {
                // this
                new CodeInstruction(OpCodes.Ldarg_0),

                // Player.Get(base.GameObject)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(GameObject) })),

                // num (battery charge)
                new CodeInstruction(OpCodes.Ldloc_0),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new UsingRadioBattery(...)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingRadioBatteryEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnUsingRadioBattery(ev)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingRadioBattery))),

                // if (!ev.IsAllowed)
                //   return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UsingRadioBatteryEventArgs), nameof(UsingRadioBatteryEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),

                // num = ev.Charge
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UsingRadioBatteryEventArgs), nameof(UsingRadioBatteryEventArgs.Charge))),
                new CodeInstruction(OpCodes.Stloc_0),

                // this.inv.items.ModifyDuration(this.myRadio, num)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Radio), nameof(Radio.inv))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory.items))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Radio), nameof(Radio.myRadio))),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Inventory.SyncListItemInfo), nameof(Inventory.SyncListItemInfo.ModifyDuration))),

                // return
                new CodeInstruction(OpCodes.Ret),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
