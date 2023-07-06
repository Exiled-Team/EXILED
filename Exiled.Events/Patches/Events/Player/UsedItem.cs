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
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using InventorySystem.Items;
    using InventorySystem.Items.Usables;
    using InventorySystem.Items.Usables.Scp1576;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable SA1402 // File may only contain a single type

    /// <summary>
    ///     Patches <see cref="Consumable.ServerOnUsingCompleted" />
    ///     Adds the <see cref="Handlers.Player.UsedItem" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UsedItem))]
    [HarmonyPatch(typeof(Consumable), nameof(Consumable.ServerOnUsingCompleted))]
    internal static class UsedItem
    {
        internal static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int index = 0;

            newInstructions.InsertRange(index, InstructionsToInject());

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        internal static List<CodeInstruction> InstructionsToInject() => new List<CodeInstruction>
        {
            // Player.Get(this.Owner)
            new(OpCodes.Ldarg_0),
            new(OpCodes.Callvirt, PropertyGetter(typeof(ItemBase), nameof(ItemBase.Owner))),
            new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

            // this
            new(OpCodes.Ldarg_0),

            // UsedItemEventArgs ev = new(Player, UsableItem)
            new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsedItemEventArgs))[0]),
            new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsedItem))),
        };
    }

    /// <summary>
    ///     Patches <see cref="Scp268.ServerOnUsingCompleted" />
    ///     Adds the <see cref="Handlers.Player.UsedItem" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UsedItem))]
    [HarmonyPatch(typeof(Scp268), nameof(Scp268.ServerOnUsingCompleted))]
    internal static class UsedItem268
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int index = 0;

            newInstructions.InsertRange(index, UsedItem.InstructionsToInject());

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    ///     Patches <see cref="Scp1576Item.ServerOnUsingCompleted" />
    ///     Adds the <see cref="Handlers.Player.UsedItem" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.UsedItem))]
    [HarmonyPatch(typeof(Scp1576Item), nameof(Scp1576Item.ServerOnUsingCompleted))]
    internal static class UsedItem1576
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int index = 0;

            newInstructions.InsertRange(index, UsedItem.InstructionsToInject());

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}