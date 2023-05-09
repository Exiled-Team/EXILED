// -----------------------------------------------------------------------
// <copyright file="PickupListRemove.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1402
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pickups;
    using API.Features.Pools;

    using HarmonyLib;

    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.ThrowableProjectiles;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ItemPickupBase.OnDestroy"/>.
    /// </summary>
    [HarmonyPatch(typeof(ItemPickupBase), nameof(ItemPickupBase.OnDestroy))]
    internal static class PickupListRemove
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(0, new[]
            {
                // _ = Pickup.BaseToPickup.Remove(this);
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(Pickup), nameof(Pickup.BaseToPickup))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, Method(typeof(Dictionary<ItemPickupBase, Pickup>), nameof(Dictionary<ItemPickupBase, Pickup>.Remove), new[] { typeof(ItemPickupBase) })),
                new(OpCodes.Pop),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
