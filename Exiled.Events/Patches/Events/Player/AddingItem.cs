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
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerAddItem"/>
    /// to add <see cref="Handlers.Player.AddingItem"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.AddingItem))]
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerAddItem))]
    internal class AddingItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(AddingItemEventArgs));

            Label continueLabel = generator.DefineLabel();

            int offset = -1;
            int index = newInstructions.FindLastIndex(x => x.Is(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory.UserInventory)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(this._hub);
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // itemBase
                    new(OpCodes.Ldloc_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // AddingItemEventArgs ev = new(Player, ItemBase, true);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AddingItemEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnAddingItem(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnAddingItem))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AddingItemEventArgs), nameof(AddingItemEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, continueLabel),

                    new(OpCodes.Ret),

                    // item = ev.Item.Base;
                    // serial = ev.Item.Serial;
                    new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex).WithLabels(continueLabel),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AddingItemEventArgs), nameof(AddingItemEventArgs.Item))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Items.Item), nameof(API.Features.Items.Item.Base))),
                    new(OpCodes.Stloc_1),

                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(AddingItemEventArgs), nameof(AddingItemEventArgs.Item))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Items.Item), nameof(API.Features.Items.Item.Serial))),
                    new(OpCodes.Starg_S, 2),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}