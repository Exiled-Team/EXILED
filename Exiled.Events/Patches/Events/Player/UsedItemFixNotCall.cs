// -----------------------------------------------------------------------
// <copyright file="UsedItemFixNotCall.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using InventorySystem.Items.Usables;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Consumable.EquipUpdate" />.
    ///     Adds the <see cref="Handlers.Player.UsedItem" /> event for when NW don't call the <see cref="UsableItemsController.ServerOnUsingCompleted"/>.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UsedItem))]
    [HarmonyPatch(typeof(Consumable), nameof(Consumable.EquipUpdate))]
    internal class UsedItemFixNotCall
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(UsedItemEventArgs));

            const int offset = -1;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(Consumable), nameof(Consumable.ActivateEffects)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(base.Owner)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Consumable), nameof(Consumable.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this
                    new(OpCodes.Ldarg_0),

                    // UsedItemEventArgs ev = new(ReferenceHub, UsableItem)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsedItemEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnUsedItem(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsedItem))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
