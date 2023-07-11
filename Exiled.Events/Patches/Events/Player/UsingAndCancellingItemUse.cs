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

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Usables;

    using PluginAPI.Events;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="UsableItemsController.ServerReceivedStatus" />.
    ///     Adds the <see cref="Handlers.Player.UsingItem" /> and <see cref="Handlers.Player.CancellingItemUse" />event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.CancellingItemUse))]
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UsingItem))]
    [HarmonyPatch(typeof(UsableItemsController), nameof(UsableItemsController.ServerReceivedStatus))]
    internal static class UsingAndCancellingItemUse
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(UsingItemEventArgs));

            int offset = 2;
            int index = newInstructions.FindIndex(
                instruction => instruction.Calls(Method(typeof(UsableItemsController), nameof(UsableItemsController.GetCooldown)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(referenceHub)
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // usableItem
                    new(OpCodes.Ldloc_1),

                    // cooldown
                    new(OpCodes.Ldloc_S, 4),

                    // UsingItemEventArgs ev = new(Player, UsableItem, float)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingItemEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnUsingItem(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingItem))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingItemEventArgs), nameof(UsingItemEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // cooldown = ev.Cooldown
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UsingItemEventArgs), nameof(UsingItemEventArgs.Cooldown))),
                    new(OpCodes.Stloc_S, 4),
                });

            offset = -16;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Newobj && (ConstructorInfo)instruction.operand == GetDeclaredConstructors(typeof(PlayerCancelUsingItemEvent))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(referenceHub)
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // handler.CurrentUsable.Item
                    new(OpCodes.Ldloc_2),
                    new(OpCodes.Ldflda, Field(typeof(PlayerHandler), nameof(PlayerHandler.CurrentUsable))),
                    new(OpCodes.Ldfld, Field(typeof(CurrentlyUsedItem), nameof(CurrentlyUsedItem.Item))),

                    // CancellingItemUseEventArgs ev = new(Player, UsableItem)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(CancellingItemUseEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnCancellingItemUse(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnCancellingItemUse))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(CancellingItemUseEventArgs), nameof(CancellingItemUseEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}