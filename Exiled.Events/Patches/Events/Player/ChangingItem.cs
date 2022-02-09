// -----------------------------------------------------------------------
// <copyright file="ChangingItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Items;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Inventory.CurInstance"/>.
    /// Adds the <see cref="Handlers.Player.ChangingItem"/> event.
    /// </summary>
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
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // ib2
                new CodeInstruction(OpCodes.Ldloc_1),

                // var ev = new ChangingItemEventArgs(Player, ItemBase)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangingItemEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Player.OnChangingItem(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangingItem))),

                // if (!ev.IsAllowed)
                //    return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingItemEventArgs), nameof(ChangingItemEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),

                // if (ev.NewItem == null)
                // {
                //    ip2 = null;
                //    itermSerial = 0;
                // }
                // else
                // {
                //    ip2 = ev.NewItem.Base;
                //    itemSerial = ev.NewItem.Serial;
                // }
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingItemEventArgs), nameof(ChangingItemEventArgs.NewItem))),
                new CodeInstruction(OpCodes.Brfalse, nullLable),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ChangingItemEventArgs), nameof(ChangingItemEventArgs.NewItem))),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Item), nameof(Item.Base))),
                new CodeInstruction(OpCodes.Stloc_1),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Item), nameof(Item.Serial))),
                new CodeInstruction(OpCodes.Starg, 1),
                new CodeInstruction(OpCodes.Br, continueLabel),
                new CodeInstruction(OpCodes.Ldc_I4_0).WithLabels(nullLable),
                new CodeInstruction(OpCodes.Starg, 1),
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
