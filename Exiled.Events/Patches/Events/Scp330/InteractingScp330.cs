// -----------------------------------------------------------------------
// <copyright file="InteractingScp330.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp330
{
    using System;
#pragma warning disable SA1118
#pragma warning disable SA1313

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using CustomPlayerEffects;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using Footprinting;

    using HarmonyLib;

    using Interactables.Interobjects;

    using InventorySystem;
    using InventorySystem.Items.Usables.Scp330;
    using InventorySystem.Searching;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="Scp330Interobject.ServerInteract"/> method to add the <see cref="Handlers.Scp330.InteractingScp330"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp330Interobject), nameof(Scp330Interobject.ServerInteract))]

    internal static class InteractingScp330
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnFalse = generator.DefineLabel();
            Label continueProcessing = generator.DefineLabel();

            LocalBuilder eventHandler = generator.DeclareLocal(typeof(DroppingUpScp330EventArgs));


            // Remove first isHuman check
            newInstructions.RemoveRange(0, 5);

            int offset = -3;
            int index = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(Scp330Bag), nameof(Scp330Bag.ServerProcessPickup)))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Load arg 0 (No param, instance of object) EStack[ReferenceHub Instance]
                new CodeInstruction(OpCodes.Ldarg_1),

                // Using Owner call Player.Get static method with it (Reference hub) and get a Player back  EStack[Player ]
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Get random candy EStack[Player, Candy]
                new CodeInstruction(OpCodes.Call, Method(typeof(Scp330Candies), nameof(Scp330Candies.GetRandom))),

                // num2 EStack[Player, Candy, num2]
                new CodeInstruction(OpCodes.Ldloc_2),

                // EStack[Player, Candy, num2, ReferenceHub Instance]
                new CodeInstruction(OpCodes.Ldarg_1),

                // EStack[Player, Candy, num2, characterClassManager]
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.characterClassManager))),

                // EStack[Player, Candy, num2, IsHuman]
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(CharacterClassManager), nameof(CharacterClassManager.IsHuman))),

                // Pass all 4 variables to InteractingScp330EventArgs  New Object, get a new object in return EStack[InteractingScp330EventArgs  Instance]
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingScp330EventArgs))[0]),

                 // Copy it for later use again EStack[InteractingScp330EventArgs Instance, InteractingScp330EventArgs Instance]
                new CodeInstruction(OpCodes.Dup),

                // Call Method on Instance EStack[DamagingScp244EventArgs Instance] (pops off so that's why we needed to dup)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp330), nameof(Handlers.Scp330.OnInteractingScp330))),

                // Call its instance field (get; set; so property getter instead of field) EStack[IsAllowed]
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.IsAllowed))),

                // If isAllowed = 1, jump to continue route, otherwise, return occurs below
                new CodeInstruction(OpCodes.Brtrue, continueProcessing),

                // False Route
                new CodeInstruction(OpCodes.Nop).WithLabels(returnFalse),
                new CodeInstruction(OpCodes.Ret),

                // Good route of is allowed being true 
                new CodeInstruction(OpCodes.Nop).WithLabels(continueProcessing),
            });

            int overwriteOffset = 1;
            int overwriteIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(Scp330Interobject), nameof(Scp330Interobject.RpcMakeSound)))) + overwriteOffset;

            int includeSameLine = -2;
            int nextReturn = newInstructions.FindIndex(overwriteIndex, instruction => instruction.opcode == OpCodes.Ret) + includeSameLine;
            Log.Info($"overwriteIndex {overwriteIndex} and nextReturn {nextReturn} and {newInstructions.Count}");
            newInstructions.RemoveRange(overwriteIndex, newInstructions.Count - nextReturn);

            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }

            Log.Info($" Index {index} ");

            int count = 0;
            int il_pos = 0;
            foreach (CodeInstruction instr in newInstructions)
            {
                Log.Info($"Current op code: {instr.opcode} and index {count} and {instr.operand} size {instr.opcode.Size} and {il_pos}");
                il_pos += instr.opcode.Size;
                count++;
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
