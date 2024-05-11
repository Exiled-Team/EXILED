// -----------------------------------------------------------------------
// <copyright file="UsingConsumable.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.Usables;
    using Mirror;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1600 // Elements should be documented

    /// <summary>
    /// Patches <see cref="UsableItemsController.Update" />
    /// Adds the <see cref="Handlers.Player.UsedItem" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingConsumable))]
    [HarmonyPatch(typeof(UsableItemsController), nameof(UsableItemsController.Update))]
    internal static class UsingConsumable
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label continueLabel = generator.DefineLabel();
            Label retLabel = generator.DefineLabel();

            int offset = -1;
            int index = newInstructions.FindIndex(i => i.Calls(Method(typeof(Dictionary<ReferenceHub, PlayerHandler>.Enumerator), nameof(Dictionary<ReferenceHub, PlayerHandler>.Enumerator.MoveNext)))) + offset;

            newInstructions[index].labels.Add(retLabel);

            offset = -2;
            index = newInstructions.FindIndex(x => x.Calls(Method(typeof(UsableItem), nameof(UsableItem.ServerOnUsingCompleted)))) + offset;

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
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsingConsumableEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsingConsumable))),

                // if (ev.IsAllowed) goto continueLabel;
                new(OpCodes.Call, PropertyGetter(typeof(UsingConsumableEventArgs), nameof(UsingConsumableEventArgs.IsAllowed))),
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
                new(OpCodes.Callvirt, PropertyGetter(typeof(NetworkBehaviour), nameof(NetworkBehaviour.connectionToClient))),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Ldloc_2),
                new(OpCodes.Ldfld, Field(typeof(CurrentlyUsedItem), nameof(CurrentlyUsedItem.ItemSerial))),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StatusMessage))[0]),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Callvirt, FirstMethod(
                    typeof(NetworkConnection),
                    m =>
                    {
                        return m.IsGenericMethod && m.Name == nameof(NetworkConnection.Send);
                    }).MakeGenericMethod(typeof(StatusMessage))),

                // goto ret;
                new(OpCodes.Br_S, retLabel),
                new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}