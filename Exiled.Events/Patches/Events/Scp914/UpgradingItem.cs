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

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp914;

    using global::Scp914;

    using Handlers;

    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp914Upgrader.ProcessPickup" />.
    ///     Adds the <see cref="Scp914.UpgradingPickup" /> event.
    /// </summary>
    [EventPatch(typeof(Scp914), nameof(Scp914.UpgradingPickup))]
    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.ProcessPickup))]
    internal static class UpgradingItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 1;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Stloc_1) + offset;

            LocalBuilder ev = generator.DeclareLocal(typeof(UpgradingPickupEventArgs));
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // pickup
                    new(OpCodes.Ldarg_0),

                    // outputPos
                    new(OpCodes.Ldloc_1),

                    // knobSetting
                    new(OpCodes.Ldarg_3),

                    // UpgradingPickupEventArgs ev = new(pickup, outputPos, knobSetting)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpgradingPickupEventArgs))[0]),

                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp914.OnUpgradingPickup(ev);
                    new(OpCodes.Call, Method(typeof(Scp914), nameof(Scp914.OnUpgradingPickup))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPickupEventArgs), nameof(UpgradingPickupEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // outputPos = ev.OutputPosition
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Dup),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPickupEventArgs), nameof(UpgradingPickupEventArgs.OutputPosition))),
                    new(OpCodes.Stloc_1),

                    // setting = ev.KnobSetting
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPickupEventArgs), nameof(UpgradingPickupEventArgs.KnobSetting))),
                    new(OpCodes.Starg_S, 3),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}