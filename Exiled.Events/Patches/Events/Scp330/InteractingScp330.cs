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

    public static class InteractingScp330
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label continueProcessing = generator.DefineLabel();

            Label shouldNotSever = generator.DefineLabel();

            LocalBuilder eventHandler = generator.DeclareLocal(typeof(InteractingScp330EventArgs));

            // Remove original "No scp can touch" logic.
            newInstructions.RemoveRange(0, 5);

            // Find ServerPickupProcess, insert before it.
            int offset = -3;
            int index = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(Scp330Bag), nameof(Scp330Bag.ServerProcessPickup)))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),

                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                new(OpCodes.Ldloc_2),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(InteractingScp330EventArgs))[0]),

                new(OpCodes.Dup),

                new(OpCodes.Stloc, eventHandler.LocalIndex),

                new(OpCodes.Ldloc, eventHandler.LocalIndex),

                new(OpCodes.Call, Method(typeof(Handlers.Scp330), nameof(Handlers.Scp330.OnInteractingScp330))),

                new(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.IsAllowed))),

                new(OpCodes.Brtrue, continueProcessing),

                new(OpCodes.Ret),

                new CodeInstruction(OpCodes.Nop).WithLabels(continueProcessing),
            });

            // Logic to find the only ServerProcessPickup and replace with our own.
            int removeServerProcessOffset = -2;
            int removeServerProcessIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(Scp330Bag), nameof(Scp330Bag.ServerProcessPickup)))) + removeServerProcessOffset;

            newInstructions.RemoveRange(removeServerProcessIndex, 3);

            Label ignoreOverlay = generator.DefineLabel();

            // Remove NW server process logic.
            newInstructions.InsertRange(removeServerProcessIndex, new[]
            {
                new CodeInstruction(OpCodes.Ldloc, eventHandler),

                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.Candy))),

                new CodeInstruction(OpCodes.Ldloca_S, 3),

                new CodeInstruction(OpCodes.Call, Method(typeof(InteractingScp330), nameof(InteractingScp330.ServerProcessPickup), new[] { typeof(ReferenceHub), typeof(CandyKindID), typeof(Scp330Bag).MakeByRefType() })),
            });

            // This is to find the location of RpcMakeSound to remove the original code and add a new sever logic structure (Start point)
            int addShouldSeverOffset = 1;
            int addShouldSeverIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(Scp330Interobject), nameof(Scp330Interobject.RpcMakeSound)))) + addShouldSeverOffset;

            // This is to find the location of the next return (End point)
            int includeSameLine = 1;
            int nextReturn = newInstructions.FindIndex(addShouldSeverIndex, instruction => instruction.opcode == OpCodes.Ret) + includeSameLine;

            // Remove original code from after RpcMakeSound to next return and then fully replace it.
            newInstructions.RemoveRange(addShouldSeverIndex, nextReturn - addShouldSeverIndex);

            addShouldSeverIndex = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(Scp330Interobject), nameof(Scp330Interobject.RpcMakeSound)))) + addShouldSeverOffset;

            newInstructions.InsertRange(addShouldSeverIndex, new[]
            {
                new CodeInstruction(OpCodes.Ldloc, eventHandler.LocalIndex),

                new (OpCodes.Callvirt, PropertyGetter(typeof(InteractingScp330EventArgs), nameof(InteractingScp330EventArgs.ShouldSever))),

                new (OpCodes.Brfalse, shouldNotSever),

                new CodeInstruction(OpCodes.Ldarg_1),

                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.playerEffectsController))),

                new CodeInstruction(OpCodes.Ldstr, nameof(SeveredHands)),

                new CodeInstruction(OpCodes.Ldc_R4, 0f),

                new CodeInstruction(OpCodes.Ldc_I4_0),

                new CodeInstruction(OpCodes.Callvirt, Method(typeof(PlayerEffectsController), nameof(PlayerEffectsController.EnableByString), new[] { typeof(string), typeof(float), typeof(bool) })),

                new CodeInstruction(OpCodes.Pop),

                new CodeInstruction(OpCodes.Ret),
            });

            // This will let us jump to the taken candies code and lock until ldarg_0, meaning we allow base game logic handle candy adding.
            int addTakenCandiesOffset = -1;

            int addTakenCandiesIndex = newInstructions.FindLastIndex(instruction => instruction.LoadsField(Field(typeof(Scp330Interobject), nameof(Scp330Interobject._takenCandies)))) + addTakenCandiesOffset;
            newInstructions[addTakenCandiesIndex].WithLabels(shouldNotSever);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static bool ServerProcessPickup(ReferenceHub ply, CandyKindID candy, out Scp330Bag bag)
        {
            if (!Scp330Bag.TryGetBag(ply, out bag))
            {
                return ply.inventory.ServerAddItem(ItemType.SCP330, ushort.MinValue) != null;
            }

            bool result = bag.TryAddSpecific(candy);

            if (bag.AcquisitionAlreadyReceived)
            {
                bag.ServerRefreshBag();
            }

            return result;
        }
    }
}
