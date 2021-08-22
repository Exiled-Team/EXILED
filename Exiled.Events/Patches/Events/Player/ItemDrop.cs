// -----------------------------------------------------------------------
// <copyright file="ItemDrop.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using InventorySystem.Items;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    using Player = Exiled.Events.Handlers.Player;

    /// <summary>
    /// Patches <see cref="InventorySystem.Inventory.UserCode_CmdDropItem"/>.
    /// Adds the <see cref="Handlers.Player.DroppingItem"/> events.
    /// </summary>
    [HarmonyPatch(typeof(InventorySystem.Inventory), nameof(InventorySystem.Inventory.UserCode_CmdDropItem))]
    internal static class ItemDrop
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ret) + offset;
            LocalBuilder item = generator.DeclareLocal(typeof(Item));
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(0, new[]
            {
                // Player.Get(this._hub)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(InventorySystem.Inventory), nameof(InventorySystem.Inventory._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Exiled.API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // GetItem(player, itemSerial)
                new CodeInstruction(OpCodes.Ldarg_1),

                // true
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new DroppingItemEventArgs(Player, Item, true)
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

        private static ItemBase GetItem(Exiled.API.Features.Player player, ushort serial) => player.TryGetItem(serial, out ItemBase item) ? item : null;
    }
}
