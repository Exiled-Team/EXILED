// -----------------------------------------------------------------------
// <copyright file="UsedItem.cs" company="Exiled Team">
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

    using InventorySystem.Items;
    using InventorySystem.Items.Usables;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Consumable.ServerOnUsingCompleted" />
    ///     Adds the <see cref="Handlers.Player.UsedItem" /> event.
    /// </summary>
    [HarmonyPatch(typeof(UsableItem), nameof(UsableItem.ServerOnUsingCompleted))]
    internal static class UsedItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int index = 0;

            newInstructions.InsertRange(index, new[]
            {
                // Player.Get(this.Owner)
                new CodeInstruction(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, PropertyGetter(typeof(ItemBase), nameof(ItemBase.Owner))),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // this
                new(OpCodes.Ldarg_0),

                // UsedItemEventArgs ev = new(Player, UsableItem)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsedItemEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsedItem))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}