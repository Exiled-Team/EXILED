// -----------------------------------------------------------------------
// <copyright file="RemovingItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items;
    using MEC;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerRemoveItem"/>
    /// to add <see cref="Handlers.Player.RemovingItem"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.RemovingItem))]
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerRemoveItem))]
    public class RemovingItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder item = generator.DeclareLocal(typeof(ItemBase));
            LocalBuilder ev = generator.DeclareLocal(typeof(RemovingItemEventArgs));

            Label retLabel = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Throw) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(inv._hub)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),

                    // itemSerial
                    new(OpCodes.Ldarg_1),

                    // RemoveItem(Player.Get(inv._hub), itemSerial)
                    new(OpCodes.Call, Method(typeof(RemovingItem), nameof(RemoveItem))),

                    // if (!Player::Inventory.UserInventory.TryGetValue(serial, out ItemBase item)
                    //    return;
                    new(OpCodes.Ldloc_S, player.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Inventory))),
                    new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory.UserInventory))),
                    new(OpCodes.Ldfld, Field(typeof(InventoryInfo), nameof(InventoryInfo.Items))),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldloca_S, item.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Dictionary<ushort, ItemBase>), nameof(Dictionary<ushort, ItemBase>.TryGetValue))),
                    new(OpCodes.Brfalse_S, retLabel),

                    // Player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // ItemBase
                    new(OpCodes.Ldloc_S, item.LocalIndex),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // RemovingItemEventArgs ev = new(Player, ItemBase, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemovingItemEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnRemovingItem(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnRemovingItem))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RemovingItemEventArgs), nameof(RemovingItemEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    // serial = ev.Item.Serial;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RemovingItemEventArgs), nameof(RemovingItemEventArgs.Item))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Item), nameof(Item.Serial))),
                    new(OpCodes.Starg_S, 1),
                });

            /*index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RemovingItemEventArgs), nameof(RemovingItemEventArgs.Item))),
                    new(OpCodes.Stloc_1)
                });*/

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];
        }

        private static void RemoveItem(Player player, ushort serial)
        {
#if DEBUG
            Log.Debug($"Removing item ({serial}) from a player (before null check)");
#endif
            if (player is null)
            {
#if DEBUG
                Log.Debug("Attempted to remove item from null player, returning.");
#endif
                return;
            }

            if (!player.Inventory.UserInventory.Items.ContainsKey(serial))
            {
#if DEBUG
                Log.Debug("Attempted to remove an item the player doesn't own, returning.");
#endif
                return;
            }
#if DEBUG
            Log.Debug(
                $"Inventory Info (before): {player.Nickname} - {player.Items.Count} ({player.Inventory.UserInventory.Items.Count})");
            foreach (Item item in player.Items)
                Log.Debug($"{item})");
#endif
            ItemBase itemBase = player.Inventory.UserInventory.Items[serial];

            player.ItemsValue.Remove(Item.Get(itemBase));

            Item.BaseToItem.Remove(itemBase);

            Timing.CallDelayed(0.15f, () =>
            {
#if DEBUG
                Log.Debug($"Item ({serial}) removed from {player.Nickname}");
                Log.Debug($"Inventory Info (after): {player.Nickname} - {player.Items.Count} ({player.Inventory.UserInventory.Items.Count})");

                foreach (Item item in player.Items)
                    Log.Debug($"{item})");
#endif
            });
        }
    }
}