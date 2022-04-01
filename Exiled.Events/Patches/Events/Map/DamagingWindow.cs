// -----------------------------------------------------------------------
// <copyright file="DamagingWindow.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map {
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using NorthwoodLib.Pools;
    using static HarmonyLib.AccessTools;

#pragma warning disable SA1118 // The parameter spans multiple lines
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="BreakableWindow.ServerDamageWindow(float)"/>.
    /// Adds the <see cref="Map.DamagingWindow"/> event.
    /// </summary>
    [HarmonyPatch(typeof(BreakableWindow), nameof(BreakableWindow.ServerDamageWindow))]
    internal static class DamagingWindow {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // this
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                // damage
                new CodeInstruction(OpCodes.Ldarg_1),

                // var ev = new DamagingWindowEventArgs(this, damage);
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(DamagingWindowEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),

                // Map.OnDamagingWindow(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Map), nameof(Map.OnDamagingWindow))),

                // damage = ev.Damage;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DamagingWindowEventArgs), nameof(DamagingWindowEventArgs.Damage))),
                new CodeInstruction(OpCodes.Starg_S, 1),
            });

            for (int i = 0; i < newInstructions.Count; i++)
                yield return newInstructions[i];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
