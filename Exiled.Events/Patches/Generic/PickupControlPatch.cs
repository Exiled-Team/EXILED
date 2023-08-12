// -----------------------------------------------------------------------
// <copyright file="PickupControlPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Items;
    using API.Features.Pickups;
    using API.Features.Pools;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    using MapGeneration.Distributors;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1402 /// File may only contain a single type.
    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerCreatePickup(ItemBase, PickupSyncInfo, Vector3, Quaternion, bool, Action{ItemPickupBase})"/> to save scale for pickups and control <see cref="Pickup.IsSpawned"/> property.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerCreatePickup), typeof(ItemBase), typeof(PickupSyncInfo), typeof(Vector3), typeof(Quaternion), typeof(bool), typeof(Action<ItemPickupBase>))]
    internal static class PickupControlPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(
            IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 0;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldarg_S) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Item.Get(itemBase);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Item), nameof(Item.Get), new[] { typeof(ItemBase) })),

                // pickup = Pickup.Get(pickupBase);
                new(OpCodes.Ldloc_0),
                new(OpCodes.Call, Method(typeof(Pickup), nameof(Pickup.Get), new[] { typeof(ItemPickupBase) })),

                // pickup.GetItemInfo(item);
                new(OpCodes.Callvirt, Method(typeof(Pickup), nameof(Pickup.GetItemInfo))),

                // pickup.IsSpawned = spawn
                new(OpCodes.Ldarg_S, 4),
                new(OpCodes.Callvirt, PropertySetter(typeof(Pickup), nameof(Pickup.IsSpawned))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="ItemDistributor.SpawnPickup"/> to control <see cref="Pickup.IsSpawned"/> property for delayed spawned pickup.
    /// </summary>
    [HarmonyPatch(typeof(ItemDistributor), nameof(ItemDistributor.SpawnPickup))]
    internal static class TriggerPickupControlPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(newInstructions.Count - 1, new CodeInstruction[]
            {
                // Pickup.Get(pickupBase).IsSpawned = true
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Pickup), nameof(Pickup.Get), new[] { typeof(ItemPickupBase) })),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Callvirt, PropertySetter(typeof(Pickup), nameof(Pickup.IsSpawned))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
