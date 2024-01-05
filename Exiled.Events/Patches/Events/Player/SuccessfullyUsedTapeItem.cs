// -----------------------------------------------------------------------
// <copyright file="SuccessfullyUsedTapeItem.cs" company="Exiled Team">
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

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="InventorySystem.Items.FlamingoTapePlayer.TapeItem.EquipUpdate"/>
    /// to add <see cref="Handlers.Player.SuccessfullyUsedTapeItem"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.SuccessfullyUsedTapeItem))]
    [HarmonyPatch(typeof(InventorySystem.Items.FlamingoTapePlayer.TapeItem), nameof(InventorySystem.Items.FlamingoTapePlayer.TapeItem.EquipUpdate))]
    internal class SuccessfullyUsedTapeItem
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = -2;
            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldarg_0) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Player.Get(this.Owner);
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(InventorySystem.Items.FlamingoTapePlayer.TapeItem), nameof(InventorySystem.Items.FlamingoTapePlayer.TapeItem.Owner))),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // this
                    new(OpCodes.Ldarg_0),

                    // UsedTapeEventArgs ev = new(Player, ItemBase);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UsedTapeEventArgs))[0]),

                    // Handlers.Player.OnUsedTape(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnUsedTape))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}