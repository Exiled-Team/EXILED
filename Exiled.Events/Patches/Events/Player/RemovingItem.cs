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
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Items;

    using static HarmonyLib.AccessTools;

    [HarmonyPatch(typeof(InventoryExtensions), nameof(InventoryExtensions.ServerRemoveItem))]
    public class RemovingItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder item = generator.DeclareLocal(typeof(ItemBase));
            LocalBuilder ev = generator.DeclareLocal(typeof(RemovingItemEventArgs));

            Label retLabel = generator.DefineLabel();

            int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Throw) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Inventory))),
                    new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory.UserInventory))),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldloca_S, item.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Dictionary<ushort, ItemBase>), nameof(Dictionary<ushort, ItemBase>.TryGetValue))),
                    new(OpCodes.Brfalse_S, retLabel),

                    new(OpCodes.Ldloc_S, item.LocalIndex),

                    new(OpCodes.Ldc_I4_1),

                    new(OpCodes.Call, GetDeclaredConstructors(typeof(RemovingItemEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnRemovingItem))),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(RemovingItemEventArgs), nameof(RemovingItemEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, retLabel),

                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(RemovingItemEventArgs), nameof(RemovingItemEventArgs.Item))),
                    new(OpCodes.Dup),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Item), nameof(Item.Base))),
                    new(OpCodes.Stloc_S, item.LocalIndex),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(Item), nameof(Item.Serial))),
                    new(OpCodes.Starg_S, 1),
                });

            index = newInstructions.FindIndex(x => x.opcode == OpCodes.Ret) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, item.LocalIndex),
                    new(OpCodes.Stloc_1)
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];
        }
    }
}