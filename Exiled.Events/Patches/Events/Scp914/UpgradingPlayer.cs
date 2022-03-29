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

namespace Exiled.Events.Patches.Events.Scp914
{
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
    internal static class UpgradingPlayer
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 0;
            int index = newInstructions.FindIndex(i => i.opcode == OpCodes.Ldarg_0) + offset;
            Label returnLabel = generator.DefineLabel();
            LocalBuilder curSetting = generator.DeclareLocal(typeof(Scp914KnobSetting));
            LocalBuilder ev = generator.DeclareLocal(typeof(UpgradingPlayerEventArgs));

            int removalOffset = -9;
            int removalIndex = newInstructions.FindIndex(i => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OverridePosition))) + removalOffset;
            for (int i = 0; i < 10; i++)
            {
                newInstructions.RemoveAt(removalIndex);
            }

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // curSetting = setting;
                new(OpCodes.Ldarg, 4),
                new(OpCodes.Stloc, curSetting.LocalIndex),

                // Player.Get(ply)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // upgradeInventory
                new(OpCodes.Ldarg_1),

                // heldOnly
                new(OpCodes.Ldarg_2),

                // setting
                new(OpCodes.Ldarg, 4),

                // moveVector
                new(OpCodes.Ldarg_3),

                // var ev = new UpgradingPlayerEventArgs(player, upgradeInventory, heldonly, setting);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpgradingPlayerEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev.LocalIndex),

                // Handlers.Scp914.OnUpgradingPlayer(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp914), nameof(Handlers.Scp914.OnUpgradingPlayer))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, returnLabel),
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Dup),
                new(OpCodes.Dup),

                // upgradeInventory = ev.UpgradeItems
                new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.UpgradeItems))),
                new(OpCodes.Starg, 1),

                // heldOnly = ev.HeldOnly
                new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.HeldOnly))),
                new(OpCodes.Starg, 2),

                // setting = ev.KnobSetting
                new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.KnobSetting))),
                new(OpCodes.Starg, 4),

                // ply.playerMovementSync.OverridePosition(ev.OutputPosition);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(ReferenceHub), nameof(ReferenceHub.playerMovementSync))),
                new(OpCodes.Ldloc, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.OutputPosition))),
                new(OpCodes.Ldc_R4, 0.0f),
                new(OpCodes.Ldc_I4_0),
                new(OpCodes.Callvirt, Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OverridePosition))),
            });

            offset = -4;
            index = newInstructions.FindIndex(i => i.opcode == OpCodes.Callvirt && (MethodInfo)i.operand == Method(typeof(Scp914ItemProcessor), nameof(Scp914ItemProcessor.OnInventoryItemUpgraded))) + offset;
            Label continueLabel = generator.DefineLabel();
            newInstructions[index + 5].labels.Add(continueLabel);
            LocalBuilder ev2 = generator.DeclareLocal(typeof(UpgradingInventoryItemEventArgs));

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // setting = curSetting
                new(OpCodes.Ldloc, curSetting.LocalIndex),
                new(OpCodes.Starg, 4),

                // Player.Get(ply)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // ItemBase item = GetItem
                new(OpCodes.Ldloc, 6),

                // setting
                new(OpCodes.Ldarg, 4),
                new(OpCodes.Ldc_I4_1),

                // var ev = new UpgradingInventoryItemEventArgs(player, item, setting)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpgradingInventoryItemEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc, ev2.LocalIndex),

                // Handlers.Scp914.OnUpgradingInventoryItem(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp914), nameof(Handlers.Scp914.OnUpgradingInventoryItem))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingInventoryItemEventArgs), nameof(UpgradingInventoryItemEventArgs.IsAllowed))),
                new(OpCodes.Brfalse, continueLabel),
                new(OpCodes.Ldloc, ev2.LocalIndex),

                // setting = ev.KnobSetting
                new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingInventoryItemEventArgs), nameof(UpgradingInventoryItemEventArgs.KnobSetting))),
                new(OpCodes.Starg, 4),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }

        private static ItemBase GetItem(KeyValuePair<uint, ItemBase> kvp)
        {
            Log.Error($"{kvp.Key} - {kvp.Value.ItemTypeId} - {kvp.Value.ItemSerial}");
            return kvp.Value;
        }
    }
}
