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

    using InventorySystem.Items.Radio;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RadioItem.Update"/>.
    /// Adds the <see cref="Handlers.Player.UsingRadioBattery"/> event.
    /// </summary>
    [HarmonyPatch(typeof(RadioItem), nameof(RadioItem.Update))]
    internal static class UsingRadioBattery
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The index offset.
            int offset = -4;

            // Search for the first "ldloc.0".
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_0) + offset;

            // Get the return label of the last "ret".
            Label returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            // Declare an "UsingRadioBatteryEventArgs" local variable.
            LocalBuilder ev = generator.DeclareLocal(typeof(UsingRadioBatteryEventArgs));

            // var ev = new UsingRadioBatteryEventArgs(this, Player.Get(base.Owner), num);
            //
            // Handlers.Player.OnUsingRadioBattery(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            //
            // num = ev.Drain;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // this
                new(OpCodes.Ldarg_0),

                // Player.Get(base.Owner)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(RadioItem), nameof(RadioItem.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // num
                new(OpCodes.Ldloc_0),

                // true
                new(OpCodes.Ldc_I4_1),

                // var ev = new UsingRadioBatteryEventArgs(...)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingRadioBatteryEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Player.OnUsingRadioBattery(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingRadioBattery))),

                // if (!ev.IsAllowed)
                //   return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(UsingRadioBatteryEventArgs), nameof(UsingRadioBatteryEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),

                // num = ev.Drain
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(UsingRadioBatteryEventArgs), nameof(UsingRadioBatteryEventArgs.Drain))),
                new(OpCodes.Stloc_0),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
