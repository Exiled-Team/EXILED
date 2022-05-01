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

#pragma warning disable SA1118 // Parameter should not span multiple lines

            int offset = -3;
            int index = newInstructions.FindLastIndex(instruction => instruction.LoadsField(Field(typeof(ReferenceHub), nameof(ReferenceHub.inventory)))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new(OpCodes.Ldloc_0),

                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                new(OpCodes.Ldloc_1),

                new(OpCodes.Ldloc_1),

                new(OpCodes.Ldarg_1),

                new(OpCodes.Ldfld, Field(typeof(SelectScp330Message), nameof(SelectScp330Message.CandyID))),

                new(OpCodes.Callvirt, Method(typeof(Scp330Bag), nameof(Scp330Bag.TryRemove))),

                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppingScp330EventArgs))[0]),

                new(OpCodes.Stloc, eventHandler.LocalIndex),

                new(OpCodes.Ldloc, eventHandler.LocalIndex),

                new(OpCodes.Call, Method(typeof(Handlers.Scp330), nameof(Handlers.Scp330.OnDroppingScp330))),

                new(OpCodes.Ldloc, eventHandler.LocalIndex),

                new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingScp330EventArgs), nameof(DroppingScp330EventArgs.IsAllowed))),

                new(OpCodes.Brtrue_S, continueProcessing),

                new(OpCodes.Ret),

                new CodeInstruction(OpCodes.Nop).WithLabels(continueProcessing),
            });

            // Set our location of previous owner
            int jumpOverOffset = 1;
            int jumpOverIndex = newInstructions.FindLastIndex(instruction => instruction.StoresField(Field(typeof(ItemPickupBase), nameof(ItemPickupBase.PreviousOwner)))) + jumpOverOffset;

            // Remove TryRemove candy logic since we did it earlier from current location
            newInstructions.RemoveRange(jumpOverIndex, 6);

            int candyKindIdIndex = 4;

            newInstructions.InsertRange(jumpOverIndex, new[]
            {
                new CodeInstruction(OpCodes.Ldloc, eventHandler.LocalIndex),

                new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingScp330EventArgs), nameof(DroppingScp330EventArgs.Candy))),

                new(OpCodes.Stloc, candyKindIdIndex),

                new(OpCodes.Ldloc, candyKindIdIndex),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
