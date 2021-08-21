// -----------------------------------------------------------------------
// <copyright file="InventoryControlPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1118
#pragma warning disable SA1402
#pragma warning disable SA1649

    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Inventory = InventorySystem.Inventory;

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerAddItem"/> to help manage <see cref="API.Features.Player.Items"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerAddItem))]
    internal static class InventoryControlAddPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int offset = -3;
            int index = newInstructions.FindIndex(i =>
                i.opcode == OpCodes.Callvirt &&
                (MethodInfo)i.operand == Method(typeof(ItemBase), nameof(ItemBase.OnAdded))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(inv._hub)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // itemInstance
                new CodeInstruction(OpCodes.Ldloc_0),

                // AddItem(player, itemInstance)
                new CodeInstruction(OpCodes.Call, Method(typeof(InventoryControlAddPatch), nameof(AddItem))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void AddItem(Player player, ItemBase item) => player?.ItemsValue.Add(Item.Get(item));
    }

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerDropItem"/> to help manage <see cref="API.Features.Player.Items"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerDropItem))]
    internal static class InventoryControlRemovePatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int offset = -3;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Call && (MethodInfo)i.operand ==
                Method(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerRemoveItem))) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(InventoryControlRemovePatch), nameof(RemoveItem))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static void RemoveItem(Player player, ItemBase item) => player?.ItemsValue.Remove(Item.Get(item));
    }
}
