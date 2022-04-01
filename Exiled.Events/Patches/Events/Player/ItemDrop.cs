// -----------------------------------------------------------------------
// <copyright file="ItemDrop.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player {
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using InventorySystem.Items;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="InventorySystem.Inventory.UserCode_CmdDropItem"/>.
    /// Adds the <see cref="Player.DroppingItem"/> and <see cref="Player.DroppingNull"/> events.
    /// </summary>
    [HarmonyPatch(typeof(InventorySystem.Inventory), nameof(InventorySystem.Inventory.UserCode_CmdDropItem))]
    internal static class ItemDrop {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder item = generator.DeclareLocal(typeof(Item));
            Label returnLabel = generator.DefineLabel();
            Label notNullLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                // Item item = GetItem(player, itemSerial)
                // if (item is null)
                //    Handlers.Player.OnDroppingNull(new DroppingNullEventArgs(Player));
                //    return;
                //
                // Handlers.Player.OnDroppingNull(new DroppingItemEventArgs(Player, Item, true));
                // if (!ev.IsAllowed)
                //     return;
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(InventorySystem.Inventory), nameof(InventorySystem.Inventory._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Call, Method(typeof(ItemDrop), nameof(ItemDrop.GetItem))),
                new CodeInstruction(OpCodes.Stloc_S, item.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, item.LocalIndex),
                new CodeInstruction(OpCodes.Brtrue_S, notNullLabel),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(InventorySystem.Inventory), nameof(InventorySystem.Inventory._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppingNullEventArgs))[0]),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.OnDroppingNull))),
                new CodeInstruction(OpCodes.Ret),
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(notNullLabel),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(InventorySystem.Inventory), nameof(InventorySystem.Inventory._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldloc_S, item.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppingItemEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.OnDroppingItem))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static ItemBase GetItem(API.Features.Player player, ushort serial) => player.TryGetItem(serial, out ItemBase item) ? item : null;
    }
}
