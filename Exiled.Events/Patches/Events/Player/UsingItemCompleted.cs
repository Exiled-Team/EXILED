// -----------------------------------------------------------------------
// <copyright file="UsingItemCompleted.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
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

            int offset = -2;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Callvirt && x.operand == (object)Method(typeof(UsableItem), nameof(UsableItem.ServerOnUsingCompleted))) + offset;

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

                // if (!ev.IsAllowed) return;
                new(OpCodes.Call, PropertyGetter(typeof(UsingItemCompletedEventArgs), nameof(UsingItemCompletedEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, ret),
            });

            offset = -3;
            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldfld && x.operand == (object)Field(typeof(PlayerHandler), nameof(PlayerHandler.CurrentUsable))) + offset;
            newInstructions[index].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}