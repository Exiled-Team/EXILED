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

            int offset = -4;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_0) + offset;

            Label returnLabel = newInstructions[newInstructions.Count - 1].labels[0];
            Label continueLabel = newInstructions[newInstructions.Count - 1].labels[0];

            LocalBuilder ev = generator.DeclareLocal(typeof(UsingRadioBatteryEventArgs));
            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            newInstructions[index].labels.Add(continueLabel);

            // if (Player.Get(base.Owner) is not Player player)
            //   continue;
            //
            // var ev = new UsingRadioBatteryEventArgs(this, player, num, true);
            //
            // Handlers.Player.OnUsingRadioBattery(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            //
            // num = ev.Drain;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, PropertyGetter(typeof(RadioItem), nameof(RadioItem.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, player.LocalIndex),

                new(OpCodes.Brfalse_S, continueLabel),

                new(OpCodes.Ldarg_0),

                new(OpCodes.Ldloc_S, player.LocalIndex),

                new(OpCodes.Ldloc_0),

                new(OpCodes.Ldc_I4_1),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingRadioBatteryEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingRadioBattery))),

                new(OpCodes.Callvirt, PropertyGetter(typeof(UsingRadioBatteryEventArgs), nameof(UsingRadioBatteryEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),

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
