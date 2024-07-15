// -----------------------------------------------------------------------
// <copyright file="UpgradingPlayer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp914
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;
    using API.Features.Items;
    using Exiled.API.Features.Scp914Processors;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp914;
    using global::Scp914;
    using global::Scp914.Processors;
    using HarmonyLib;
    using InventorySystem.Items;
    using PlayerRoles.FirstPersonControl;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Scp914 = Handlers.Scp914;

    /// <summary>
    /// Patches <see cref="Scp914Upgrader.ProcessPlayer(ReferenceHub, bool, bool, Vector3, Scp914KnobSetting)" />
    /// to add the <see cref="Scp914.UpgradingPlayer" />, <see cref="Scp914.UpgradedInventoryItem"/> and <see cref="Scp914.UpgradingInventoryItem"/> events.
    /// </summary>
    [EventPatch(typeof(Scp914), nameof(Scp914.UpgradingPlayer))]
    [EventPatch(typeof(Scp914), nameof(Scp914.UpgradedInventoryItem))]
    [EventPatch(typeof(Scp914), nameof(Scp914.UpgradingInventoryItem))]
    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.ProcessPlayer))]
    internal static class UpgradingPlayer
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            // Find override position
            int offset = -3;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(FpcExtensionMethods), nameof(FpcExtensionMethods.TryOverridePosition)))) + offset;

            Label returnLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(UpgradingPlayerEventArgs));

            // Move labels from override - 3 position (Right after a branch)
            List<Label> labels = newInstructions[index].labels;

            // Remove TryOverride, and !upgradeInventory
            newInstructions.RemoveRange(index, 5);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(ply)
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(labels),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // upgradeInventory
                    new(OpCodes.Ldarg_1),

                    // heldOnly
                    new(OpCodes.Ldarg_2),

                    // setting
                    new(OpCodes.Ldarg_S, 4),

                    // moveVector
                    new(OpCodes.Ldarg_3),

                    // UpgradingPlayerEventArgs ev = new(player, upgradeInventory, heldonly, setting, moveVector);
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpgradingPlayerEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Scp914.OnUpgradingPlayer(ev);
                    new(OpCodes.Call, Method(typeof(Scp914), nameof(Scp914.OnUpgradingPlayer))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),

                    // load "ev" three times
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // upgradeInventory = ev.UpgradeItems
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.UpgradeItems))),
                    new(OpCodes.Starg_S, 1),

                    // heldOnly = ev.HeldOnly
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.HeldOnly))),
                    new(OpCodes.Starg_S, 2),

                    // setting = ev.KnobSetting
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.KnobSetting))),
                    new(OpCodes.Starg_S, 4),

                    // ev.Player.Teleport(ev.OutputPosition);
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.Player))),
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.OutputPosition))),
                    new(OpCodes.Callvirt, Method(typeof(Player), nameof(Player.Teleport), new[] { typeof(Vector3) })),
                });

            Label continueLabel = generator.DefineLabel();

            const int continueOffset = -4;

            // Find the call of InventoryItemUpgraded
            int continueIndex = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(Scp914ItemProcessor), nameof(Scp914ItemProcessor.OnInventoryItemUpgraded)))) + continueOffset;

            LocalBuilder ev2 = generator.DeclareLocal(typeof(UpgradingInventoryItemEventArgs));

            newInstructions.InsertRange(
                continueIndex,
                new CodeInstruction[]
                {
                    // Player.Get(ply)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // itemBase
                    new(OpCodes.Ldloc_S, 8),

                    // setting
                    new(OpCodes.Ldarg_S, 4),

                    // processor
                    new(OpCodes.Ldloc_S, 9),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // UpgradingInventoryItemEventArgs ev = new(player, itemBase, setting, processor)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpgradingInventoryItemEventArgs))[0]),
                    new(OpCodes.Dup),

                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev2.LocalIndex),

                    // Handlers.Scp914.OnUpgradingInventoryItem(ev);
                    new(OpCodes.Call, Method(typeof(Scp914), nameof(Scp914.OnUpgradingInventoryItem))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingInventoryItemEventArgs), nameof(UpgradingInventoryItemEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // loading ev2 2 times
                    new(OpCodes.Ldloc_S, ev2.LocalIndex),

                    new(OpCodes.Dup),

                    // setting = ev.KnobSetting
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingInventoryItemEventArgs), nameof(UpgradingInventoryItemEventArgs.KnobSetting))),
                    new(OpCodes.Starg_S, 4),

                    // processor = ev.Processor
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingInventoryItemEventArgs), nameof(UpgradingInventoryItemEventArgs.Processor))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp914Processor), nameof(Scp914Processor.Base))),
                    new(OpCodes.Stloc_S, 9),
                });

            offset = 3;
            index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Newobj) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(ply)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // Item.Get(itemBase2)
                    new(OpCodes.Ldloc_S, 10),
                    new(OpCodes.Call, Method(typeof(Item), nameof(Item.Get), new[] { typeof(ItemBase) })),

                    // knobSetting
                    new(OpCodes.Ldarg_S, 4),

                    // UpgradedInventoryItemEventArgs ev = new(Player, Item, Scp914KnobSetting)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpgradedInventoryItemEventArgs))[0]),

                    // Scp914.OnUpgradedInventoryItem(ev)
                    new(OpCodes.Call, Method(typeof(Scp914), nameof(Scp914.OnUpgradedInventoryItem))),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);
            newInstructions[newInstructions.Count - 1].labels.Add(continueLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}