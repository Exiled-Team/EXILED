// -----------------------------------------------------------------------
// <copyright file="ChangedItem.cs" company="Exiled Team">
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

    using InventorySystem;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Inventory.CurInstance" />.
    ///     Adds the <see cref="Handlers.Player.ChangedItem" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.CurInstance), MethodType.Setter)]
    internal static class ChangedItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            int index = newInstructions.Count - 1;
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(this._hub);
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // itemBase
                    new(OpCodes.Ldloc_0),

                    // ChangingItemEventArgs ev = new(Player, ItemBase)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ChangedItemEventArgs))[0]),

                    // Handlers.Player.OnChangingItem(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnChangedItem))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}