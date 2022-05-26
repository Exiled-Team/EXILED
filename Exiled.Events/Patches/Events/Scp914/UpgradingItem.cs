// -----------------------------------------------------------------------
// <copyright file="UpgradingItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp914
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

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

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // pickup
                new(OpCodes.Ldarg_0),

                // outputPos
                new(OpCodes.Ldloc_0),

                // knobSetting
                new(OpCodes.Ldarg_3),
                new(OpCodes.Ldc_I4_1),

                // var ev = new UpgradingItemEventArgs(pickup, outputPos, knobSetting)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpgradingItemEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Scp914.OnUpgradingItem(ev);
                new(OpCodes.Call, Method(typeof(Scp914), nameof(Scp914.OnUpgradingItem))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingItemEventArgs), nameof(UpgradingItemEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, returnLabel),
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Dup),

                // outputPos = ev.OutputPosition
                new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingItemEventArgs), nameof(UpgradingItemEventArgs.OutputPosition))),
                new(OpCodes.Stloc_0),

                // setting = ev.KnobSetting
                new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingItemEventArgs), nameof(UpgradingItemEventArgs.KnobSetting))),
                new(OpCodes.Starg, 3),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
