// -----------------------------------------------------------------------
// <copyright file="ProcessingHotkey.cs" company="Exiled Team">
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

    using InventorySystem;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patch the <see cref="Inventory.UserCode_CmdProcessHotkey__ActionName__UInt16" />.
    ///     Adds the <see cref="Handlers.Player.ProcessingHotkey" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdProcessHotkey__ActionName__UInt16))]
    internal static class ProcessingHotkey
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            Label defaultLabel = generator.DefineLabel();
            Label breakLabel = generator.DefineLabel();

            Label[] switchLabels =
            {
                generator.DefineLabel(),
                generator.DefineLabel(),
                generator.DefineLabel(),
                generator.DefineLabel(),
            };

            LocalBuilder hotkeyButton = generator.DeclareLocal(typeof(byte));

            newInstructions.InsertRange(
                0,
                new[]
                {
                    // if (hotkeyButtonPressed != ActionName.HotkeyKeycard)
                    //    goto switchLabels[0];
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldc_I4_S, 20),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brfalse_S, switchLabels[0]),

                    // hotkeyButton = 0
                    // break
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Stloc_S, hotkeyButton.LocalIndex),
                    new(OpCodes.Br_S, breakLabel),

                    // switchLabels[0]:
                    //
                    // if (hotKeyButtonPressed != ActionName.HotkeyPrimaryFirearm)
                    //    goto switchLabels[1];
                    new CodeInstruction(OpCodes.Ldarg_1).WithLabels(switchLabels[0]),
                    new(OpCodes.Ldc_I4_S, 24),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brfalse_S, switchLabels[1]),

                    // hotkeyButton = 1
                    // break
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Stloc_S, hotkeyButton.LocalIndex),
                    new(OpCodes.Br_S, breakLabel),

                    // switchLabels[1]:
                    //
                    // if (hotKeyButtonPressed != ActionName.HotkeySecondaryFirearm)
                    //    goto switchLabels[2];
                    new CodeInstruction(OpCodes.Ldarg_1).WithLabels(switchLabels[1]),
                    new(OpCodes.Ldc_I4_S, 25),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brfalse_S, switchLabels[2]),

                    // hotkeyButton = 2
                    // break
                    new(OpCodes.Ldc_I4_2),
                    new(OpCodes.Stloc_S, hotkeyButton.LocalIndex),
                    new(OpCodes.Br_S, breakLabel),

                    // switchLabels[2]:
                    //
                    // if (hotKeyButtonPressed != ActionName.HotkeyMedical)
                    //    goto switchLabels[3];
                    new CodeInstruction(OpCodes.Ldarg_1).WithLabels(switchLabels[2]),
                    new(OpCodes.Ldc_I4_S, 26),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brfalse_S, switchLabels[3]),

                    // hotkeyButton = 3
                    // break
                    new(OpCodes.Ldc_I4_3),
                    new(OpCodes.Stloc_S, hotkeyButton.LocalIndex),
                    new(OpCodes.Br_S, breakLabel),

                    // switchLabels[3]:
                    //
                    // if (hotKeyButtonPressed != ActionName.HotkeyGrenade)
                    //    goto defaultLabel;
                    new CodeInstruction(OpCodes.Ldarg_1).WithLabels(switchLabels[3]),
                    new(OpCodes.Ldc_I4_S, 29),
                    new(OpCodes.Ceq),
                    new(OpCodes.Brfalse_S, defaultLabel),

                    // hotkeyButton = 4
                    // break
                    new(OpCodes.Ldc_I4_4),
                    new(OpCodes.Stloc_S, hotkeyButton.LocalIndex),
                    new(OpCodes.Br_S, breakLabel),

                    // defaultLabel:
                    //
                    // hotkeyButton = 0
                    // break
                    new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(defaultLabel),
                    new(OpCodes.Stloc_S, hotkeyButton.LocalIndex),
                    new(OpCodes.Br_S, breakLabel),

                    // breakLabel:
                    //
                    // Player.Get(this._hub)
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(breakLabel),
                    new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // hotkeyButton
                    new(OpCodes.Ldloc_S, hotkeyButton.LocalIndex),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // ProcessingHotkeyEventArgs ev = new(Player, HotkeyButton, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ProcessingHotkeyEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnProcessingHotkey(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnProcessingHotkey))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ProcessingHotkeyEventArgs), nameof(ProcessingHotkeyEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}