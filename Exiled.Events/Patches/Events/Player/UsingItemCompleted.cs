// -----------------------------------------------------------------------
// <copyright file="UsingItemCompleted.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Pools;

    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items.Usables;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1600 // Elements should be documented

    /// <summary>
    ///     Patches <see cref="UsableItemsController.Update" />
    ///     Adds the <see cref="Handlers.Player.UsedItem" /> event.
    /// </summary>
    [HarmonyPatch(typeof(UsableItemsController), nameof(UsableItemsController.Update))]
    internal static class UsingItemCompleted
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label ret = generator.DefineLabel();
            Label continueLabel = generator.DefineLabel();

            int offset = -2;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Callvirt && x.operand == (object)Method(typeof(UsableItem), nameof(UsableItem.ServerOnUsingCompleted))) + offset;

            newInstructions[index].labels.Add(continueLabel);

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(keyValuePair.Key)
                new CodeInstruction(OpCodes.Ldloca_S, 1).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Call, PropertyGetter(typeof(KeyValuePair<ReferenceHub, PlayerHandler>), nameof(KeyValuePair<ReferenceHub, PlayerHandler>.Key))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // currentUsable.Item
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldfld, Field(typeof(CurrentlyUsedItem), nameof(CurrentlyUsedItem.Item))),

                // UsingItemCompletedEventArgs ev = new(Player, UsableItem)
                // Handlers.Player.OnUsingItemCompleted(ev)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingItemCompletedEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingItemCompleted))),

                // if (ev.IsAllowed) goto continueLabel;
                new(OpCodes.Call, PropertyGetter(typeof(UsingItemCompletedEventArgs), nameof(UsingItemCompletedEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                // currentUsable.Item.OnUsingCancelled();
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldfld, Field(typeof(CurrentlyUsedItem), nameof(CurrentlyUsedItem.Item))),
                new(OpCodes.Callvirt, Method(typeof(UsableItem), nameof(UsableItem.OnUsingCancelled))),

                // keyValuePair.Value.CurrentUsable = CurrentlyUsedItem.None;
                new(OpCodes.Ldloca_S,  1),
                new(OpCodes.Call, PropertyGetter(typeof(KeyValuePair<ReferenceHub, PlayerHandler>), nameof(KeyValuePair<ReferenceHub, PlayerHandler>.Value))),
                new(OpCodes.Ldsfld, Field(typeof(CurrentlyUsedItem), nameof(CurrentlyUsedItem.None))),
                new(OpCodes.Stfld, Field(typeof(PlayerHandler), nameof(PlayerHandler.CurrentUsable))),

                // keyValuePair.Key.inventory.connectionToClient.Send(new StatusMessage(StatusMessage.StatusType.Cancel, currentUsable.ItemSerial), 0);
                //
                // keyValuePair.Key.inventory.connectionToClient
                new(OpCodes.Ldloca_S,  1),
                new(OpCodes.Call, PropertyGetter(typeof(KeyValuePair<ReferenceHub, PlayerHandler>), nameof(KeyValuePair<ReferenceHub, PlayerHandler>.Key))),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.inventory))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Mirror.NetworkBehaviour), nameof(Mirror.NetworkBehaviour.connectionToClient))),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldfld, Field(typeof(CurrentlyUsedItem), nameof(CurrentlyUsedItem.ItemSerial))),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StatusMessage))[0]),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Callvirt, typeof(Mirror.NetworkConnection).GetMethods().First(x => x.Name == nameof(Mirror.NetworkConnection.Send))),

                // goto ret;
                new(OpCodes.Br_S, ret),
            });

            offset = 1;
            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldfld && x.operand == (object)Field(typeof(PlayerHandler), nameof(PlayerHandler.CurrentUsable))) + offset;
            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}