// -----------------------------------------------------------------------
// <copyright file="ChangingItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Items;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Inventory.CurInstance" />.
    /// Adds the <see cref="Handlers.Player.ChangingItem" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingItem))]
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.ServerSelectItem))]
    internal static class ChangingItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();
            Label nullLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingItemEventArgs));

            const int offset = 3;
            int index = newInstructions.FindLastIndex(
                instruction => instruction.Calls(Method(typeof(EquipDequipModifierExtensions), nameof(EquipDequipModifierExtensions.CanEquip)))) + offset;

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

                    // ChangingItemEventArgs ev = new(Player, ItemBase)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingItemEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnChangingItem(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingItem))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingItemEventArgs), nameof(ChangingItemEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // if (ev.NewItem == null)
                    //    goto nullLabel;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingItemEventArgs), nameof(ChangingItemEventArgs.Item))),
                    new(OpCodes.Brfalse_S, nullLabel),

                    // itemBase = ev.NewItem.Base;
                    // itemSerial = ev.NewItem.Serial;
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingItemEventArgs), nameof(ChangingItemEventArgs.Item))),
                    new(OpCodes.Dup),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Item), nameof(Item.Base))),
                    new(OpCodes.Stloc_1),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Item), nameof(Item.Serial))),
                    new(OpCodes.Starg_S, 1),

                    // goto continueLabel
                    new(OpCodes.Br_S, continueLabel),

                    // nullLabel:
                    //
                    // itemSerial = 0
                    new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(nullLabel),
                    new(OpCodes.Starg_S, 1),

                    // continueLabel:
                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}