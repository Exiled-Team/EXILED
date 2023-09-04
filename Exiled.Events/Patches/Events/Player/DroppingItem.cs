// -----------------------------------------------------------------------
// <copyright file="DroppingItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.API.Features;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using Handlers;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items.Pickups;

    using static HarmonyLib.AccessTools;

    using Item = API.Features.Items.Item;

    /// <summary>
    ///     Patches <see cref="Inventory.UserCode_CmdDropItem__UInt16__Boolean" />.
    ///     <br>Adds the <see cref="Handlers.Player.DroppingItem" />, <see cref="Handlers.Player.DroppingNothing" /> and <see cref="Handlers.Player.DroppedItem"/> events.</br>
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.DroppingItem))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.DroppingNothing))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.DroppedItem))]
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropItem__UInt16__Boolean))]
    internal static class DroppingItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            Label notNullLabel = generator.DefineLabel();

            LocalBuilder item = generator.DeclareLocal(typeof(Item));
            LocalBuilder ev = generator.DeclareLocal(typeof(DroppingItemEventArgs));

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                // if (player.TryGetItem(itemSerial, out Item item))
                //    goto notNullLabel;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldloca_S, item.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(API.Features.Player), nameof(API.Features.Player.TryGetItem), new[] { typeof(ushort), typeof(Item).MakeByRefType() })),
                new(OpCodes.Brtrue_S, notNullLabel),

                // Player.Get(this._hub)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // DroppingNothingEventArgs ev = new(Player)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppingNothingEventArgs))[0]),

                // Player.OnDroppingNothing(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDroppingNothing))),

                // return
                new(OpCodes.Br_S, returnLabel),

                // notNulllabel:
                //
                // Player.Get(this._hub)
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(notNullLabel),
                new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // item.Base
                new(OpCodes.Ldloc_S, item.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Item), nameof(Item.Base))),

                // tryThrow
                new(OpCodes.Ldarg_2),

                // true
                new(OpCodes.Ldc_I4_1),

                // DroppingItemEventArgs ev = new(Player, ItemBase, bool, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppingItemEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Player.OnDroppingItem(ev)
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDroppingItem))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, returnLabel),

                // isThrown = ev.IsThrown;
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.IsThrown))),
                new(OpCodes.Starg_S, 2),
            });

            const int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Stloc_1) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // ev.Player
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.Player))),

                // Pickup::Get(ItemPickupBase)
                new(OpCodes.Ldloc_1),
                new(OpCodes.Call, Method(typeof(Pickup), nameof(Pickup.Get), new[] { typeof(ItemPickupBase) })),

                // ev.IsThrown
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.IsThrown))),

                // Player::OnDroppedItem(new DroppedItemEventArgs(ev.Player, Player::Get(ItemPickupBase), ev.IsThrown))
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppedItemEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDroppedItem))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}