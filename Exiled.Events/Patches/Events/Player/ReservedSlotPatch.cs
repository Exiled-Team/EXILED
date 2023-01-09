// -----------------------------------------------------------------------
// <copyright file="ReservedSlotPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Enums;
    using Exiled.Events.EventArgs.Player;

    using Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using PluginAPI.Events;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="ReservedSlot.HasReservedSlot(string, out bool)" />.
    ///     Adds the <see cref="Player.ReservedSlot" /> event.
    /// </summary>
    [HarmonyPatch(typeof(ReservedSlot), nameof(ReservedSlot.HasReservedSlot))]
    internal static class ReservedSlotPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder jumpConditions = generator.DeclareLocal(typeof(ReservedSlotEventResult));

            Label continueConditions = generator.DefineLabel();

            Label allowUnconditional = generator.DefineLabel();
            Label returnTrue = generator.DefineLabel();
            Label returnFalse = generator.DefineLabel();

            int offset = -1;
            int index = newInstructions.FindLastIndex(
                instruction => instruction.LoadsField(Field(typeof(PlayerCheckReservedSlotCancellationData), nameof(PlayerCheckReservedSlotCancellationData.HasReservedSlot)))) + offset;

            newInstructions[index].WithLabels(continueConditions);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Grab user-id, copy label from current newInstruction[index] to ensure jumps to the label come to this instr instead
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),

                    // Grab reserved slot bool
                    new(OpCodes.Ldarg_1),

                    // Instantiate object with previous vars
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ReservedSlotsCheckEventArgs))[0]),

                    // Duplicate for future use
                    new(OpCodes.Dup),

                    // Pass event to be invoked
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnReservedSlot))),

                    // Using duped value from before, grab result from event
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ReservedSlotsCheckEventArgs), nameof(ReservedSlotsCheckEventArgs.Result))),

                    // Store result value in local variable.
                    new(OpCodes.Stloc_S, jumpConditions.LocalIndex),

                    // Let normal NW code proceed. UseBaseGameSystem - 0 -> Allow base game check
                    new(OpCodes.Ldloc_S, jumpConditions.LocalIndex),
                    new(OpCodes.Brfalse_S, continueConditions),

                    // Allow use of reserved slots, returning true CanUseReservedSlots - 1 - return true
                    new(OpCodes.Ldloc_S, jumpConditions.LocalIndex),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Beq_S, returnTrue),

                    // Reserved slot rejection - CannotUseReservedSlots - 2 - return false
                    new(OpCodes.Ldloc_S, jumpConditions.LocalIndex),
                    new(OpCodes.Ldc_I4_2),
                    new(OpCodes.Beq_S, returnFalse),

                    // Allow unconditional connection - AllowConnectionUnconditionally - 3 - return true with bypass to true
                    new(OpCodes.Ldloc_S, jumpConditions.LocalIndex),
                    new(OpCodes.Ldc_I4_3),
                    new(OpCodes.Beq_S, allowUnconditional),

                    // Return true, but set bypass to true.
                    new CodeInstruction(OpCodes.Ldc_I4_1).WithLabels(allowUnconditional),
                    new CodeInstruction(OpCodes.Starg_S, 1),

                    // Return True
                    new CodeInstruction(OpCodes.Ldc_I4_1).WithLabels(returnTrue),
                    new(OpCodes.Ret),

                    // Return false
                    new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(returnFalse),
                    new(OpCodes.Ret),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}