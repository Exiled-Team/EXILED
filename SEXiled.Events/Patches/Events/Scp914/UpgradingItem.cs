// -----------------------------------------------------------------------
// <copyright file="UpgradingItem.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Events.Scp914
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using SEXiled.Events.EventArgs;
    using SEXiled.Events.Handlers;

    using global::Scp914;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp914Upgrader.ProcessPickup"/>.
    /// Adds the <see cref="Scp914.UpgradingItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.ProcessPickup))]
    internal static class UpgradingItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            // The first index offset.
            const int offset = 0;

            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_1) + offset;
            LocalBuilder ev = generator.DeclareLocal(typeof(UpgradingItemEventArgs));
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // pickup
                new CodeInstruction(OpCodes.Ldarg_0),

                // outputPos
                new CodeInstruction(OpCodes.Ldloc_0),

                // knobSetting
                new CodeInstruction(OpCodes.Ldarg_3),
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new UpgradingItemEventArgs(pickup, outputPos, knobSetting)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpgradingItemEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Scp914.OnUpgradingItem(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Scp914), nameof(Scp914.OnUpgradingItem))),

                // if (!ev.IsAllowed)
                //    return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingItemEventArgs), nameof(UpgradingItemEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Dup),

                // outputPos = ev.OutputPosition
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingItemEventArgs), nameof(UpgradingItemEventArgs.OutputPosition))),
                new CodeInstruction(OpCodes.Stloc_0),

                // setting = ev.KnobSetting
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingItemEventArgs), nameof(UpgradingItemEventArgs.KnobSetting))),
                new CodeInstruction(OpCodes.Starg, 3),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
