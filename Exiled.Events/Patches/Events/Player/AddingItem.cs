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

    using API.Features.Core.Generic.Pools;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Pickups;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items;
    using InventorySystem.Items.Pickups;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="InventoryExtensions.ServerAddItem(Inventory, ItemType, ItemAddReason, ushort, ItemPickupBase)"/>.
    /// Adds the <see cref="Handlers.Player.AddingItem"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.AddingItem))]
    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerAddItem))]
    internal static class AddingItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = -7;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldarg_3) + offset;

            Label ret = generator.DefineLabel();
            LocalBuilder ev = generator.DeclareLocal(typeof(AddingItemEventArgs));

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new(OpCodes.Ldloc_1),
                new(OpCodes.Call, Method(typeof(Item), nameof(Item.Get), new[] { typeof(ItemBase) })),
                new(OpCodes.Ldarg_S, 4),
                new(OpCodes.Call, Method(typeof(Pickup), nameof(Pickup.Get), new[] { typeof(ItemPickupBase) })),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(AddingItemEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnAddingItem))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AddingItemEventArgs), nameof(AddingItemEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(AddingItemEventArgs), nameof(AddingItemEventArgs.AddReason))),
                new(OpCodes.Starg_S, 3),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}