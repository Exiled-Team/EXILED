// -----------------------------------------------------------------------
// <copyright file="ProcessingHotkey.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="Inventory.UserCode_CmdProcessHotkey"/>.
    /// Adds the <see cref="Handlers.Player.ProcessingHotkey"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdProcessHotkey))]
    internal static class ProcessingHotkey
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = generator.DefineLabel();
            Label defaultLabel = generator.DefineLabel();
            Label breakLabel = generator.DefineLabel();

            Label[] switchLabels = new Label[]
            {
                generator.DefineLabel(),
                generator.DefineLabel(),
                generator.DefineLabel(),
                generator.DefineLabel(),
            };

            LocalBuilder hotkeyButton = generator.DeclareLocal(typeof(byte));

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldc_I4_S, 20),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse_S, switchLabels[0]),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Stloc_S, hotkeyButton.LocalIndex),
                new CodeInstruction(OpCodes.Br_S, breakLabel),
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(switchLabels[0]),
                new CodeInstruction(OpCodes.Ldc_I4_S, 24),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse_S, switchLabels[1]),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Stloc_S, hotkeyButton.LocalIndex),
                new CodeInstruction(OpCodes.Br_S, breakLabel),
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(switchLabels[1]),
                new CodeInstruction(OpCodes.Ldc_I4_S, 25),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse_S, switchLabels[2]),
                new CodeInstruction(OpCodes.Ldc_I4_2),
                new CodeInstruction(OpCodes.Stloc_S, hotkeyButton.LocalIndex),
                new CodeInstruction(OpCodes.Br_S, breakLabel),
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(switchLabels[2]),
                new CodeInstruction(OpCodes.Ldc_I4_S, 26),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse_S, switchLabels[3]),
                new CodeInstruction(OpCodes.Ldc_I4_3),
                new CodeInstruction(OpCodes.Stloc_S, hotkeyButton.LocalIndex),
                new CodeInstruction(OpCodes.Br_S, breakLabel),
                new CodeInstruction(OpCodes.Ldarg_1).WithLabels(switchLabels[3]),
                new CodeInstruction(OpCodes.Ldc_I4_S, 29),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brfalse_S, defaultLabel),
                new CodeInstruction(OpCodes.Ldc_I4_4),
                new CodeInstruction(OpCodes.Stloc_S, hotkeyButton.LocalIndex),
                new CodeInstruction(OpCodes.Br_S, breakLabel),
                new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(defaultLabel),
                new CodeInstruction(OpCodes.Stloc_S, hotkeyButton.LocalIndex),
                new CodeInstruction(OpCodes.Br_S, breakLabel),
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(breakLabel),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldloc_S, hotkeyButton.LocalIndex),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ProcessingHotkeyEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnProcessingHotkey))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ProcessingHotkeyEventArgs), nameof(ProcessingHotkeyEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
