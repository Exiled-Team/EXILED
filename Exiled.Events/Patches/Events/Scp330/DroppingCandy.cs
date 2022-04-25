// -----------------------------------------------------------------------
// <copyright file="DroppingCandy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp330
{
#pragma warning disable SA1313

    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using Footprinting;

    using global::Utils.Networking;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables;
    using InventorySystem.Items.Usables.Scp330;

    using Mirror;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="Scp330NetworkHandler.ServerSelectMessageReceived"/> method to add the <see cref="Handlers.Scp330.DroppingUpScp330"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp330NetworkHandler), nameof(Scp330NetworkHandler.ServerSelectMessageReceived))]
    internal static class DroppingCandy
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnFalse = generator.DefineLabel();
            Label continueProcessing = generator.DefineLabel();

            LocalBuilder eventHandler = generator.DeclareLocal(typeof(DroppingUpScp330EventArgs));

            int offset = -3;
            int index = newInstructions.FindLastIndex(instruction => instruction.LoadsField(Field(typeof(ReferenceHub), nameof(ReferenceHub.inventory)))) + offset;

#pragma warning disable SA1118 // Parameter should not span multiple lines
            newInstructions.InsertRange(index, new[]
            {
                // Load arg 0 (No param, instance of object) EStack[Referencehub Instance]
                new CodeInstruction(OpCodes.Ldloc_0),

                // Using Owner call Player.Get static method with it (Reference hub) and get a Player back  EStack[Player]
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Load arg 0 (No param, instance of object) EStack[Player Instance, Scp330Bag Instance]
                new CodeInstruction(OpCodes.Ldloc_1),

                // EStack[Player Instance, Scp330Bag Instance, Scp330Bag Instance]
                new CodeInstruction(OpCodes.Ldloc_1),

                // EStack[Player Instance, Scp330Bag Instance, Scp330Bag Instance, SelectScp330Message Msg]
                new CodeInstruction(OpCodes.Ldarg_1),

                // EStack[Player Instance, Scp330Bag Instance, Scp330Bag Instance, CandyID]
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(SelectScp330Message), nameof(SelectScp330Message.CandyID))),

                // EStack[Player Instance, Scp330Bag Instance, CandyKindID]
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(Scp330Bag), nameof(Scp330Bag.TryRemove))),

                // Pass all 2 variables to DamageScp244 New Object, get a new object in return EStack[DroppingUpScp330EventArgs Instance]
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppingUpScp330EventArgs))[0]),

                // EStack[]
                new CodeInstruction(OpCodes.Stloc, eventHandler.LocalIndex),

                // EStack[DroppingUpScp330EventArgs Instance]
                new CodeInstruction(OpCodes.Ldloc, eventHandler.LocalIndex),

                // Call Method on Instance EStack[] (pops off so that's why we needed to dup)
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp330), nameof(Handlers.Scp330.OnDroppingUpScp330))),

                // EStack[DroppingUpScp330EventArgs Instance]
                new CodeInstruction(OpCodes.Ldloc, eventHandler.LocalIndex),

                // Call its instance field (get; set; so property getter instead of field) EStack[IsAllowed]
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DroppingUpScp330EventArgs), nameof(DroppingUpScp330EventArgs.IsAllowed))),

                // If isAllowed = 1, jump to continue route, otherwise, false return occurs below // EStack[]
                new CodeInstruction(OpCodes.Brtrue, continueProcessing),

                // False Route
                new CodeInstruction(OpCodes.Nop).WithLabels(returnFalse),
                new CodeInstruction(OpCodes.Ret),

                // Good route of is allowed being true
                new CodeInstruction(OpCodes.Nop).WithLabels(continueProcessing),
            });

            int jumpOverOffset = 1;
            int jumpOverIndex = newInstructions.FindLastIndex(instruction => instruction.StoresField(Field(typeof(ItemPickupBase), nameof(ItemPickupBase.PreviousOwner)))) + jumpOverOffset;

            // Remove TryRemove candy logic since we did it earlier. 
            newInstructions.RemoveRange(jumpOverIndex, 6);

            int CandyKindID = 4;

            newInstructions.InsertRange(jumpOverIndex, new[]
            {
                // EStack[DroppingUpScp330EventArgs Instance]
                new CodeInstruction(OpCodes.Ldloc, eventHandler.LocalIndex),

                //  EStack[DroppingUpScp330EventArgs.Candy]
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DroppingUpScp330EventArgs), nameof(DroppingUpScp330EventArgs.Candy))),
                Handlers.Scp330.OnDroppingUpScp330(ev);

                // EStack[]
                new CodeInstruction(OpCodes.Stloc, CandyKindID),

                // EStack[Candy] (Next instruction is a brtrue)
                new CodeInstruction(OpCodes.Ldloc, CandyKindID),
            });

            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
