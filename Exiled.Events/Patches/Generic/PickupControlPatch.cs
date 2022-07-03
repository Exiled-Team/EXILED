// -----------------------------------------------------------------------
// <copyright file="PickupControlPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1402
#pragma warning disable SA1649
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    using MEC;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Inventory = InventorySystem.Inventory;

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerCreatePickup(Inventory, ItemBase, PickupSyncInfo, bool)"/> to save scale for pickups.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup))]
    internal static class PickupControlPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int offset = -1;
            int index = newInstructions.FindIndex(i =>
                i.opcode == OpCodes.Ldarg_3) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // item
                new(OpCodes.Ldarg_1),

                // pickup
                new(OpCodes.Ldloc_0),

                // spawn
                new(OpCodes.Ldarg_3),

                new(OpCodes.Callvirt, Method(typeof(PickupControlPatch), nameof(PickupControlPatch.SetPickupInfo))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void SetPickupInfo(ItemBase itemBase, ItemPickupBase itemPickupBase, bool spawned)
        {
            Item item = Item.Get(itemBase);
            Pickup pickup = Pickup.Get(itemPickupBase);
            pickup.Scale = item.Scale;
            pickup.Spawned = spawned;
        }
    }
}
