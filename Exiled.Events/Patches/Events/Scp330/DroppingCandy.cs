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
    /// Patches the <see cref="Scp330NetworkHandler.ServerSelectMessageReceived"/> method to add the <see cref="Handlers.Scp330.DroppingScp330"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp330NetworkHandler), nameof(Scp330NetworkHandler.ServerSelectMessageReceived))]
    internal static class DroppingCandy
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label continueProcessing = generator.DefineLabel();

            LocalBuilder eventHandler = generator.DeclareLocal(typeof(DroppingScp330EventArgs));

            // Tested by Yamato and Undid-Iridium
#pragma warning disable SA1118 // Parameter should not span multiple lines

            int offset = -3;
            int index = newInstructions.FindLastIndex(instruction => instruction.LoadsField(Field(typeof(ReferenceHub), nameof(ReferenceHub.inventory)))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Load arg 0 (No param, instance of object) EStack[Referencehub Instance]
                new(OpCodes.Ldloc_0),

                // Using Owner call Player.Get static method with it (Reference hub) and get a Player back  EStack[Player]
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // Load arg 0 (No param, instance of object) EStack[Player Instance, Scp330Bag Instance]
                new(OpCodes.Ldloc_1),

                // EStack[Player Instance, Scp330Bag Instance, Scp330Bag Instance]
                new(OpCodes.Ldloc_1),

                // EStack[Player Instance, Scp330Bag Instance, Scp330Bag Instance, SelectScp330Message Msg]
                new(OpCodes.Ldarg_1),

                // EStack[Player Instance, Scp330Bag Instance, Scp330Bag Instance, CandyID]
                new(OpCodes.Ldfld, Field(typeof(SelectScp330Message), nameof(SelectScp330Message.CandyID))),

                // EStack[Player Instance, Scp330Bag Instance, CandyKindID]
                new(OpCodes.Callvirt, Method(typeof(Scp330Bag), nameof(Scp330Bag.TryRemove))),

                // Pass all 2 variables to DamageScp244 New Object, get a new object in return EStack[DroppingScp330EventArgs Instance]
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppingScp330EventArgs))[0]),

                // EStack[]
                new(OpCodes.Stloc, eventHandler.LocalIndex),

                // EStack[DroppingScp330EventArgs Instance]
                new(OpCodes.Ldloc, eventHandler.LocalIndex),

                // Call Method on Instance EStack[] (pops off so that's why we needed to dup)
                new(OpCodes.Call, Method(typeof(Handlers.Scp330), nameof(Handlers.Scp330.OnDroppingScp330))),

                // EStack[DroppingScp330EventArgs Instance]
                new(OpCodes.Ldloc, eventHandler.LocalIndex),

                // Call its instance field (get; set; so property getter instead of field) EStack[IsAllowed]
                new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingScp330EventArgs), nameof(DroppingScp330EventArgs.IsAllowed))),

                // If isAllowed = 1, jump to continue route, otherwise, false return occurs below // EStack[]
                new(OpCodes.Brtrue, continueProcessing),

                // False Route
                new CodeInstruction(OpCodes.Ret),

                // Good route of is allowed being true
                new CodeInstruction(OpCodes.Nop).WithLabels(continueProcessing),
            });

            // Set our location of previous owner
            int jumpOverOffset = 1;
            int jumpOverIndex = newInstructions.FindLastIndex(instruction => instruction.StoresField(Field(typeof(ItemPickupBase), nameof(ItemPickupBase.PreviousOwner)))) + jumpOverOffset;

            // Remove TryRemove candy logic since we did it earlier from current location
            newInstructions.RemoveRange(jumpOverIndex, 6);

            // Candy local index.
            int candyKindID = 4;

            // Insert our logic in space we just wiped.
            newInstructions.InsertRange(jumpOverIndex, new[]
            {
                // EStack[DroppingScp330EventArgs Instance]
                new CodeInstruction(OpCodes.Ldloc, eventHandler.LocalIndex),

                // EStack[DroppingScp330EventArgs.Candy]
                new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingScp330EventArgs), nameof(DroppingScp330EventArgs.Candy))),

                // EStack[] (The next two lines technically are not needed if I reduce the range from 6 to 4, but I had issues, tiny brain moments.)
                new(OpCodes.Stloc, candyKindID),

                // EStack[Candy] (Next instruction is a brtrue)
                new(OpCodes.Ldloc, candyKindID),
            });

            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return newInstructions[z];
            }

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
