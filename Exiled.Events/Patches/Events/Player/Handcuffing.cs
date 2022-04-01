// -----------------------------------------------------------------------
// <copyright file="Handcuffing.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player {
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using InventorySystem;
    using InventorySystem.Disarming;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="DisarmedPlayers.SetDisarmedStatus"/>.
    /// Adds the <see cref="Handlers.Player.Handcuffing"/> and <see cref="Handlers.Player.RemovingHandcuffs"/> events.
    /// </summary>
    [HarmonyPatch(typeof(DisarmedPlayers), nameof(DisarmedPlayers.SetDisarmedStatus))]
    internal static class Handcuffing {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 1;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Bne_Un_S) + offset;

            Label returnLabel = generator.DefineLabel();

            // var ev = new RemovingHandcuffsEventArgs(Player, Player, true);
            // Handlers.Player.OnRemovingHandcuffs(ev);
            //
            // if (!ev.IsAllowed)
            //     return false;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(DisarmedPlayers), nameof(DisarmedPlayers.Entries))),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(List<DisarmedPlayers.DisarmedEntry>), "get_Item", new[] { typeof(int) })),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(DisarmedPlayers.DisarmedEntry), nameof(DisarmedPlayers.DisarmedEntry.Disarmer))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(uint) })),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(RemovingHandcuffsEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnRemovingHandcuffs))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(RemovingHandcuffsEventArgs), nameof(RemovingHandcuffsEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldsfld);

            // var ev = new HandcuffingEventArgs(Player, Player, true);
            // Handlers.Player.OnHandcuffing(ev);
            //
            // if (!ev.IsAllowed)
            //     return false;
            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Inventory), nameof(Inventory._hub))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(HandcuffingEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnHandcuffing))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HandcuffingEventArgs), nameof(HandcuffingEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
