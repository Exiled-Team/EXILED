// -----------------------------------------------------------------------
// <copyright file="UsingAndCancellingItemUse.cs" company="Exiled Team">
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
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Usables;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="UsableItemsController.ServerReceivedStatus" />.
    ///     Adds the <see cref="Handlers.Player.UsingItem" /> event.
    /// </summary>
    [HarmonyPatch(typeof(UsableItemsController), nameof(UsableItemsController.ServerReceivedStatus))]
    internal static class UsingAndCancellingItemUse
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            int offset = 2;
            int index = newInstructions.FindIndex(i => (i.opcode == OpCodes.Call) && ((MethodInfo)i.operand == Method(typeof(UsableItemsController), nameof(UsableItemsController.GetCooldown)))) + offset;
            LocalBuilder ev = generator.DeclareLocal(typeof(UsingItemEventArgs));
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Ldloc_S, 4),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingItemEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingItem))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingItemEventArgs), nameof(UsingItemEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingItemEventArgs), nameof(UsingItemEventArgs.Cooldown))),
                    new(OpCodes.Stloc_S, 4),
                });

            offset = -3;
            index = newInstructions.FindIndex(i => (i.opcode == OpCodes.Callvirt) && ((MethodInfo)i.operand == Method(typeof(UsableItem), nameof(UsableItem.OnUsingCancelled)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Ldloc_2),
                    new(OpCodes.Ldflda, Field(typeof(PlayerHandler), nameof(PlayerHandler.CurrentUsable))),
                    new(OpCodes.Ldfld, Field(typeof(CurrentlyUsedItem), nameof(CurrentlyUsedItem.Item))),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(CancellingItemUseEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnCancellingItemUse))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(CancellingItemUseEventArgs), nameof(CancellingItemUseEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}