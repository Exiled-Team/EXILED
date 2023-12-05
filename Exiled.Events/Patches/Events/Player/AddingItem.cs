// -----------------------------------------------------------------------
// <copyright file="AddingItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    using static HarmonyLib.AccessTools;

    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerAddItem))]
    internal class AddingItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(AddingItemEventArgs));

            Label continueLabel = generator.DefineLabel();

            int offset = -1;
            int index = newInstructions.FindIndex(x => x.Is(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory.UserInventory)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    new(OpCodes.Ldloc_1),

                    new(OpCodes.Ldc_I4_1),

                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AddingItemEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnAddingItem))),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(AddingItemEventArgs), nameof(AddingItemEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    new(OpCodes.Ret),

                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AddingItemEventArgs), nameof(AddingItemEventArgs.Item))),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_1),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Items.Item), nameof(API.Features.Items.Item.Serial))),
                    new(OpCodes.Starg_S, 2)
                });

            offset = -2;
            index = newInstructions.FindIndex(x => x.Is(OpCodes.Callvirt, Method(typeof(ItemBase), nameof(ItemBase.OnAdded)))) + offset;

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
                    new(OpCodes.Call, Method(typeof(AddingItem), nameof(AddItem))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void AddItem(Player player, ItemBase itemBase, ItemPickupBase itemPickupBase)
        {
            API.Features.Items.Item item = API.Features.Items.Item.Get(itemBase);
            Pickup pickup = Pickup.Get(itemPickupBase);

            item.ReadPickupInfo(pickup);

            player?.ItemsValue.Add(item);
        }
    }
}