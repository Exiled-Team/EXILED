// -----------------------------------------------------------------------
// <copyright file="FixOnAddedBeingCallAfterOnRemoved.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.API.Features;
    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerAddItem(Inventory, ItemType, ushort, ItemPickupBase)"/>.
    /// Fix than NW call <see cref="InventoryExtensions.OnItemRemoved"/> before <see cref="InventoryExtensions.OnItemAdded"/> for AmmoItem.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerAddItem))]
    internal class FixOnAddedBeingCallAfterOnRemoved
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            /*
                // Modify this
                itemBase2.OnAdded(pickup);
                Action<ReferenceHub, ItemBase, ItemPickupBase> onItemAdded = InventoryExtensions.OnItemAdded;
                if (onItemAdded != null)
                {
                    onItemAdded(inv._hub, itemBase2, pickup);
                }
                // To this
                Action<ReferenceHub, ItemBase, ItemPickupBase> onItemAdded = InventoryExtensions.OnItemAdded;
                if (onItemAdded != null)
                {
                    onItemAdded(inv._hub, itemBase2, pickup);
                }
                itemBase2.OnAdded(pickup);
             */
            int opCodesToMove = 3;
            int offset = -2;
            int indexOnAdded = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(ItemBase), nameof(ItemBase.OnAdded)))) + offset;

            offset = 1;
            int indexInvoke = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(Action<ReferenceHub, ItemBase, ItemPickupBase>), nameof(Action<ReferenceHub, ItemBase, ItemPickupBase>.Invoke)))) + offset;

            newInstructions.InsertRange(indexInvoke, newInstructions.GetRange(indexOnAdded, opCodesToMove));
            newInstructions[indexInvoke].MoveLabelsFrom(newInstructions[indexInvoke + opCodesToMove]);
            newInstructions.RemoveRange(indexOnAdded, opCodesToMove);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];
            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
