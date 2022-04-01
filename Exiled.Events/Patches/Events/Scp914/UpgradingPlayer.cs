// -----------------------------------------------------------------------
// <copyright file="UpgradingPlayer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------
#pragma warning disable
using Scp914KnobSetting = Scp914.Scp914KnobSetting;
using Scp914Upgrader = Scp914.Scp914Upgrader;
#pragma warning restore

namespace Exiled.Events.Patches.Events.Scp914 {
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using global::Scp914.Processors;

    using HarmonyLib;

    using InventorySystem.Items;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp914Upgrader.ProcessPlayer"/> to add the <see cref="Handlers.Scp914.UpgradingPlayer"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.ProcessPlayer))]
    internal static class UpgradingPlayer {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 0;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldarg_0) + offset;
            Label returnLabel = generator.DefineLabel();
            LocalBuilder curSetting = generator.DeclareLocal(typeof(Scp914KnobSetting));
            LocalBuilder ev = generator.DeclareLocal(typeof(UpgradingPlayerEventArgs));

            int removalOffset = -9;
            int removalIndex = newInstructions.FindIndex(i => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OverridePosition))) + removalOffset;
            for (int i = 0; i < 10; i++) {
                newInstructions.RemoveAt(removalIndex);
            }

            newInstructions.InsertRange(index, new[]
            {
                // curSetting = setting;
                new CodeInstruction(OpCodes.Ldarg, 4),
                new CodeInstruction(OpCodes.Stloc, curSetting.LocalIndex),

                // Player.Get(ply)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // upgradeInventory
                new CodeInstruction(OpCodes.Ldarg_1),

                // heldOnly
                new CodeInstruction(OpCodes.Ldarg_2),

                // setting
                new CodeInstruction(OpCodes.Ldarg, 4),

                // moveVector
                new CodeInstruction(OpCodes.Ldarg_3),

                // var ev = new UpgradingPlayerEventArgs(player, upgradeInventory, heldonly, setting);
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpgradingPlayerEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Scp914.OnUpgradingPlayer(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp914), nameof(Handlers.Scp914.OnUpgradingPlayer))),

                // if (!ev.IsAllowed)
                //    return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, returnLabel),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),

                // upgradeInventory = ev.UpgradeItems
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.UpgradeItems))),
                new CodeInstruction(OpCodes.Starg, 1),

                // heldOnly = ev.HeldOnly
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.HeldOnly))),
                new CodeInstruction(OpCodes.Starg, 2),

                // setting = ev.KnobSetting
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.KnobSetting))),
                new CodeInstruction(OpCodes.Starg, 4),

                // ply.playerMovementSync.OverridePosition(ev.OutputPosition);
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.playerMovementSync))),
                new CodeInstruction(OpCodes.Ldloc, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.OutputPosition))),
                new CodeInstruction(OpCodes.Ldc_R4, 0.0f),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OverridePosition))),
            });

            offset = -4;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == Method(typeof(Scp914ItemProcessor), nameof(Scp914ItemProcessor.OnInventoryItemUpgraded))) + offset;
            Label continueLabel = generator.DefineLabel();
            newInstructions[index + 5].WithLabels(continueLabel);
            LocalBuilder ev2 = generator.DeclareLocal(typeof(UpgradingInventoryItemEventArgs));

            newInstructions.InsertRange(index, new[]
            {
                // setting = curSetting
                new CodeInstruction(OpCodes.Ldloc, curSetting.LocalIndex),
                new CodeInstruction(OpCodes.Starg, 4),

                // Player.Get(ply)
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // ItemBase item = GetItem
                new CodeInstruction(OpCodes.Ldloc, 6),

                // setting
                new CodeInstruction(OpCodes.Ldarg, 4),
                new CodeInstruction(OpCodes.Ldc_I4_1),

                // var ev = new UpgradingInventoryItemEventArgs(player, item, setting)
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpgradingInventoryItemEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc, ev2.LocalIndex),

                // Handlers.Scp914.OnUpgradingInventoryItem(ev);
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Scp914), nameof(Handlers.Scp914.OnUpgradingInventoryItem))),

                // if (!ev.IsAllowed)
                //    return;
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingInventoryItemEventArgs), nameof(UpgradingInventoryItemEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse, continueLabel),
                new CodeInstruction(OpCodes.Ldloc, ev2.LocalIndex),

                // setting = ev.KnobSetting
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingInventoryItemEventArgs), nameof(UpgradingInventoryItemEventArgs.KnobSetting))),
                new CodeInstruction(OpCodes.Starg, 4),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static ItemBase GetItem(KeyValuePair<uint, ItemBase> kvp) {
            Log.Error($"{kvp.Key} - {kvp.Value.ItemTypeId} - {kvp.Value.ItemSerial}");
            return kvp.Value;
        }
    }
}
