// -----------------------------------------------------------------------
// <copyright file="UsingRadioBattery.cs" company="Exiled Team">
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
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Radio;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RadioItem.Update" />.
    /// Adds the <see cref="Handlers.Player.UsingRadioBattery" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UsingRadioBattery))]
    [HarmonyPatch(typeof(RadioItem), nameof(RadioItem.Update))]
    internal static class UsingRadioBattery
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = newInstructions[newInstructions.Count - 1].labels[0];
            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(UsingRadioBatteryEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_0) + offset;

            newInstructions[index].WithLabels(continueLabel);

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // if (Player.Get(base.Owner) is not Player player)
                    //    continue;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(RadioItem), nameof(RadioItem.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // this
                    new(OpCodes.Ldarg_0),

                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // num
                    new(OpCodes.Ldloc_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // UsingRadioBatteryEventArgs ev = new(RadioItem, Player, float, bool);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingRadioBatteryEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnUsingRadioBattery(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingRadioBattery))),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingRadioBatteryEventArgs), nameof(UsingRadioBatteryEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // num = ev.Drain;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingRadioBatteryEventArgs), nameof(UsingRadioBatteryEventArgs.Drain))),
                    new(OpCodes.Stloc_0),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}