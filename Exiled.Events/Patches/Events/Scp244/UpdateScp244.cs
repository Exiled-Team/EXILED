// -----------------------------------------------------------------------
// <copyright file="UpdateScp244.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp244
{
#pragma warning disable SA1313
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Usables.Scp244;
    using InventorySystem.Searching;

    using Mirror;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp244DeployablePickup"/> to add missing event handler to the <see cref="Scp244DeployablePickup"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.UpdateRange))]
    internal static class UpdateScp244
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnFalse = generator.DefineLabel();
            Label normalProcessing = generator.DefineLabel();
            Label cont = generator.DefineLabel();

            Label isAllowed = generator.DefineLabel();
            Label notAllowed = generator.DefineLabel();

            LocalBuilder resultOfDotCheck = generator.DeclareLocal(typeof(int));

            LocalBuilder dot = generator.DeclareLocal(typeof(float));
            LocalBuilder activation = generator.DeclareLocal(typeof(float));

            // Tested by Yamato and Undid-Iridium
#pragma warning disable SA1118 // Parameter should not span multiple lines

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.LoadsField(Field(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup._activationDot)))) + offset;

            newInstructions.RemoveAt(index);
            // FYI this gets called A LOT, and I mean A LOT. UpdateRange might be a bad idea for an event catch but.. I'll defer to Nao or Joker.
            // However, it seems to be functional, I guess.
            newInstructions.InsertRange(index, new[]
            {

                new(OpCodes.Stloc, dot.LocalIndex),
                new(OpCodes.Stloc, activation.LocalIndex),

                new(OpCodes.Ldloc, dot.LocalIndex),
                new(OpCodes.Ldloc, activation.LocalIndex),

                new CodeInstruction(OpCodes.Clt),

                new(OpCodes.Stloc, resultOfDotCheck.LocalIndex),

                new(OpCodes.Ldarg_0),

                new(OpCodes.Ldloc, resultOfDotCheck.LocalIndex),


                // Pass all 2 variables to OpeningScp244EventArgs New Object, get a new object in return EStack[OpeningScp244EventArgs Instance]
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(OpeningScp244EventArgs))[0]),

                // Copy it for later use again EStack[OpeningScp244EventArgs Instance, PickingUpScp244EventArgs Instance]
                new(OpCodes.Dup),

                // Call Method on Instance EStack[OpeningScp244EventArgs Instance] (pops off so that's why we needed to dup)
                new(OpCodes.Call, Method(typeof(Handlers.Scp244), nameof(Handlers.Scp244.OnOpeningScp244))),

                // Call its instance field (get; set; so property getter instead of field) EStack[IsAllowed]
                new(OpCodes.Callvirt, PropertyGetter(typeof(OpeningScp244EventArgs), nameof(OpeningScp244EventArgs.IsAllowed))),

                // If isAllowed = 1, Continue route, otherwise, false return occurs below EStack[]
                new(OpCodes.Brfalse, normalProcessing),

            });

            int continueOffset = -1;
            int continueIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(PropertyGetter(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.State)))) + continueOffset;

            // Jumping over original NW logic.
            newInstructions[continueIndex].WithLabels(normalProcessing).MoveLabelsFrom(newInstructions[continueIndex]);

            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }

            // index = newInstructions.FindIndex(instruction => instruction.Calls(PropertyGetter(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.State)))) + offset;

            // continueIndex = newInstructions.FindIndex(index + 4, instruction => instruction.Calls(PropertyGetter(typeof(Scp244DeployablePickup), nameof(Scp244DeployablePickup.State)))) + continueOffset;

            // Log.Info($" New index {index} and continueIndex {continueIndex}");
            // int count = 0;
            // int il_pos = 0;
            // foreach (CodeInstruction instr in newInstructions)
            // {
            //    Log.Info($"Current op code: {instr.opcode} and index {count} and {instr.operand} and {il_pos} and {instr.opcode.OperandType}");
            //    il_pos += instr.opcode.Size;
            //    count++;
            // }
            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
