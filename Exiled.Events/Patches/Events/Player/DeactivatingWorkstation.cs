// -----------------------------------------------------------------------
// <copyright file="DeactivatingWorkstation.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player {
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem.Items.Firearms.Attachments;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="WorkstationController.NetworkStatus"/>.
    /// Adds the <see cref="Handlers.Player.DeactivatingWorkstation"/> event.
    /// </summary>
    [HarmonyPatch(typeof(WorkstationController), nameof(WorkstationController.NetworkStatus), MethodType.Setter)]
    internal static class DeactivatingWorkstation {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Brtrue_S) + offset;
            Label returnLabel = generator.DefineLabel();
            Label valueLabel = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(DeactivatingWorkstationEventArgs));

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldc_I4_2),
                new CodeInstruction(OpCodes.Bne_Un_S, valueLabel),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(DeactivatingWorkstationEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDeactivatingWorkstation))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DeactivatingWorkstationEventArgs), nameof(DeactivatingWorkstationEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DeactivatingWorkstationEventArgs), nameof(DeactivatingWorkstationEventArgs.NewStatus))),
                new CodeInstruction(OpCodes.Starg_S, 1),
            });

            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldfld) + offset;

            newInstructions[index].WithLabels(valueLabel);

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
