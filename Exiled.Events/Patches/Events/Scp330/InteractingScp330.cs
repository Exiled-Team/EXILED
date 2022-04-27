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
    using System.Reflection;
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

            Label shouldSever = generator.DefineLabel();
            Label shouldNotSever = generator.DefineLabel();

            LocalBuilder eventHandler = generator.DeclareLocal(typeof(InteractingScp330EventArgs));

            LocalBuilder playerEffect = generator.DeclareLocal(typeof(PlayerEffect));

            int offset = -3;
            int index = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(Scp330Bag), nameof(Scp330Bag.ServerProcessPickup)))) + offset;

            // I can't confirmed this works thus far. I don't seem to know how to get this to get called. Unless its the same as Scp244 where the event dies but.. I inserted call at
            // start of function and that's the ONLY one getting called. Seems possible its the same.
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldstr, "Woahhhh InteractingScp330 ").MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, Method(typeof(Log), nameof(Log.Info), new[] { typeof(string) })),
                // Load arg 0 (No param, instance of object) EStack[ReferenceHub Instance]
                new(OpCodes.Ldarg_1),

                // Using Owner call Player.Get static method with it (Reference hub) and get a Player back  EStack[Player ]
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Get random candy EStack[Player, Candy]
                new(OpCodes.Call, Method(typeof(Scp330Candies), nameof(Scp330Candies.GetRandom))),

                // num2 EStack[Player, Candy, num2]
                new(OpCodes.Ldloc_2),

                // EStack[Player, Candy, num2, ReferenceHub Instance]
                new(OpCodes.Ldarg_1),

                // EStack[Player, Candy, num2, characterClassManager]
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.characterClassManager))),

                // EStack[Player, Candy, num2, IsHuman]
                new(OpCodes.Callvirt, Method(typeof(CharacterClassManager), nameof(CharacterClassManager.IsHuman))),

                // Pass all 4 variables to InteractingScp330EventArgs  New Object, get a new object in return EStack[InteractingScp330EventArgs  Instance]
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingScp330EventArgs))[0]),

                 // Copy it for later use again EStack[InteractingScp330EventArgs Instance, InteractingScp330EventArgs Instance]
                new(OpCodes.Dup),

                // EStack[InteractingScp330EventArgs Instance]
                new(OpCodes.Stloc, eventHandler.LocalIndex),

                // EStack[InteractingScp330EventArgs Instance, InteractingScp330EventArgs Instance]
                new(OpCodes.Ldloc, eventHandler.LocalIndex),

                // Call Method on Instance EStack[InteractingScp330EventArgs Instance] (pops off so that's why we needed to dup)
                new(OpCodes.Call, Method(typeof(Handlers.Scp330), nameof(Handlers.Scp330.OnInteractingScp330))),

                // Call its instance field (get; set; so property getter instead of field) EStack[IsAllowed]
                new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.IsAllowed))),

                // If isAllowed = 1, jump to continue route, otherwise, return occurs below EStack[]
                new(OpCodes.Brtrue, continueProcessing),

                // False Route
                new CodeInstruction(OpCodes.Nop).WithLabels(returnFalse),
                new(OpCodes.Ret),

                // Good route of is allowed being true 
                new CodeInstruction(OpCodes.Nop).WithLabels(continueProcessing),
            });

            int addShouldSeverOffset = 1;
            int addShouldSeverIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(Scp330Interobject), nameof(Scp330Interobject.RpcMakeSound)))) + addShouldSeverOffset;

            int includeSameLine = 0;
            int nextReturn = newInstructions.FindIndex(addShouldSeverIndex, instruction => instruction.opcode == OpCodes.Ret) + includeSameLine;

            Log.Info($" newInstructions.Count  {newInstructions.Count } and nextReturn {nextReturn} and overwriteIndex {addShouldSeverIndex}");
            newInstructions.RemoveRange(addShouldSeverIndex, 14); //nextReturn - overwriteIndex, get rid of blt.s, 3 , 14

            addShouldSeverIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(Scp330Interobject), nameof(Scp330Interobject.RpcMakeSound)))) + addShouldSeverOffset;

            newInstructions.InsertRange(addShouldSeverIndex, new[]
            {
                new CodeInstruction(OpCodes.Ldloc, eventHandler.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.ShouldSever))),

                new CodeInstruction(OpCodes.Brfalse, shouldNotSever),

                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.playerEffectsController))),
                new CodeInstruction(OpCodes.Ldstr, nameof(SeveredHands)),
                new CodeInstruction(OpCodes.Ldc_R4, 0f),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                //PlayerEffectsController.Ena
                ////new CodeInstruction(OpCodes.Pop),
                ////new CodeInstruction(OpCodes.Pop),
                ////new CodeInstruction(OpCodes.Pop), Namespace.Type1.Type2:MethodName
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(PlayerEffectsController), nameof(PlayerEffectsController.EnableByString), new[] { typeof(string), typeof(float), typeof(bool) })),
                new CodeInstruction(OpCodes.Pop),
                new CodeInstruction(OpCodes.Ret),
            });


            /*
             *               new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(InventoryItemLoader), nameof(InventoryItemLoader.AvailableItems))).WithLabels(dontResetLabel),
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingIntoGrenadeEventArgs), nameof(ChangingIntoGrenadeEventArgs.Type))),
                new(OpCodes.Ldloca_S, 0),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ItemType, ItemBase>), nameof(Dictionary<ItemType, ItemBase>.TryGetValue))),
                */

            int nextSectionToRemove = newInstructions.FindIndex(addShouldSeverIndex + 3, instruction => instruction.opcode == OpCodes.Ldarg_0);

            //newInstructions.RemoveRange(nextSectionToRemove, 6);

            int addTakenCandiesOffset = -1;

            int addTakenCandiesIndex = newInstructions.FindLastIndex(instruction => instruction.LoadsField(Field(typeof(Scp330Interobject), nameof(Scp330Interobject._takenCandies)))) + addTakenCandiesOffset;

            newInstructions.InsertRange(addTakenCandiesIndex, new[]
                {
                new CodeInstruction(OpCodes.Nop).WithLabels(shouldNotSever).MoveLabelsFrom(newInstructions[addTakenCandiesIndex]),
                });

            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }

            Log.Info($" Index {index} overwriteIndex {addShouldSeverIndex}  newInstructions.Count { newInstructions.Count}");

            int count = 0;
            int il_pos = 0;
            foreach (CodeInstruction instr in newInstructions)
            {
                Log.Info($"Current op code: {instr.opcode} and index {count} and {instr.operand} and {il_pos} and {instr.opcode.OperandType}");
                il_pos += instr.opcode.Size;
                count++;
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
