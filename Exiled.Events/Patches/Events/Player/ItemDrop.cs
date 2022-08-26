// -----------------------------------------------------------------------
// <copyright file="ItemDrop.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using InventorySystem;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Item = Exiled.API.Features.Items.Item;

    /// <summary>
    ///     Patches <see cref="InventorySystem.Inventory.UserCode_CmdDropItem" />.
    ///     Adds the <see cref="Player.DroppingItem" /> and <see cref="Player.DroppingNothing" /> events.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.DroppingNull))]
    [HarmonyPatch(typeof(InventorySystem.Inventory), nameof(InventorySystem.Inventory.UserCode_CmdDropItem))]
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.UserCode_CmdDropItem))]
    internal static class ItemDrop
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder item = generator.DeclareLocal(typeof(Item));
            LocalBuilder ev = generator.DeclareLocal(typeof(DroppingItemEventArgs));
            Label returnLabel = generator.DefineLabel();
            Label notNullLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                // if (!player.TryGetItem(itemSerial, out Item item)) {
                //    Handlers.Player.OnDroppingNull(new DroppingNullEventArgs(Player));
                //    return;
                // }
                //
                // var ev = new DroppingItemEventArgs(player, item, isThrow, true);
                // Handlers.Player.OnDroppingItem(ev);
                // if (!ev.IsAllowed)
                //     return;
                // isThrow = ev.IsThrown;
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldarg_1),
                new(OpCodes.Ldloca_S, item.LocalIndex),
                new(OpCodes.Callvirt, Method(typeof(API.Features.Player), nameof(API.Features.Player.TryGetItem), new[] { typeof(ushort), typeof(Item).MakeByRefType() })),
                new(OpCodes.Brtrue_S, notNullLabel),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppingNothingEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnDroppingNothing))),
                new(OpCodes.Ret),
                new CodeInstruction(OpCodes.Ldarg_0).WithLabels(notNullLabel),
                new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldloc_S, item.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Item), nameof(Item.Base))),
                new(OpCodes.Ldarg_2),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DroppingItemEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnDroppingItem))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, returnLabel),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(DroppingItemEventArgs), nameof(DroppingItemEventArgs.IsThrown))),
                new(OpCodes.Starg_S, 2),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
