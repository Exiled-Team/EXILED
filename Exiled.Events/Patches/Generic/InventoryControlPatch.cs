// -----------------------------------------------------------------------
// <copyright file="InventoryControlPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Items;
    using API.Features.Pools;
    using Exiled.API.Features.Pickups;
    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    using MEC;

    using static HarmonyLib.AccessTools;

    using Inventory = InventorySystem.Inventory;

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerAddItem"/> to help manage <see cref="Player.Items"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerAddItem))]
    internal static class InventoryControlPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = -2;
            int index = newInstructions.FindIndex(
                i =>
                    (i.opcode == OpCodes.Callvirt) &&
                    ((MethodInfo)i.operand == Method(typeof(ItemBase), nameof(ItemBase.OnAdded)))) + offset;

            // AddItem(Player.Get(inv._hub), itemInstance)
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(inv._hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // itemInstance
                    new(OpCodes.Ldloc_1),

                    // pickup
                    new(OpCodes.Ldarg_3),

                    // AddItem(player, itemInstance, pickup)
                    new(OpCodes.Call, Method(typeof(InventoryControlPatch), nameof(AddItem))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void AddItem(Player player, ItemBase itemBase, ItemPickupBase itemPickupBase)
        {
            Item item = Item.Get(itemBase);
            Pickup pickup = Pickup.Get(itemPickupBase);

            item.ReadPickupInfo(pickup);

            player?.ItemsValue.Add(item);
        }
    }
}