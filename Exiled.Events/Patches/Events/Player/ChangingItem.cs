// -----------------------------------------------------------------------
// <copyright file="ChangingItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Inventory.CurInstance" />.
    ///     Adds the <see cref="Handlers.Player.ChangingItem" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.ChangingItem))]
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.ServerSelectItem))]
    internal static class ChangingItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 3;
            int index = newInstructions.FindLastIndex(i =>
                i.opcode == OpCodes.Call && (MethodInfo)i.operand == Method(typeof(EquipDequipModifierExtensions), nameof(EquipDequipModifierExtensions.CanEquip))) + offset;
            LocalBuilder ev = generator.DeclareLocal(typeof(ChangingItemEventArgs));
            Label returnLabel = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();
            Label nullLable = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this._hub);
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // ib2
                new(OpCodes.Ldloc_1),

                // var ev = new ChangingItemEventArgs(Player, ItemBase)
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

                // if (ev.NewItem is null)
                // {
                //    ip2 = null;
                //    itermSerial = 0;
                // }
                // else
                // {
                //    ip2 = ev.NewItem.Base;
                //    itemSerial = ev.NewItem.Serial;
                // }
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingItemEventArgs), nameof(ChangingItemEventArgs.NewItem))),
                new(OpCodes.Brfalse_S, nullLable),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ChangingItemEventArgs), nameof(ChangingItemEventArgs.NewItem))),
                new(OpCodes.Dup),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Item), nameof(Item.Base))),
                new(OpCodes.Stloc_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Item), nameof(Item.Serial))),
                new(OpCodes.Starg_S, 1),
                new(OpCodes.Br_S, continueLabel),
                new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(nullLable),
                new(OpCodes.Starg_S, 1),
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
