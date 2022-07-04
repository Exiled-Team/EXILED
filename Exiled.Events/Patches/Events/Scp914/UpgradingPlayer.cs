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

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Scp914KnobSetting = global::Scp914.Scp914KnobSetting;
    using Scp914Upgrader = global::Scp914.Scp914Upgrader;

    /// <summary>
    /// Patches <see cref="Scp914Upgrader.ProcessPlayer"/> to add the <see cref="Handlers.Scp914.UpgradingPlayer"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp914Upgrader), nameof(Scp914Upgrader.ProcessPlayer))]
    internal static class UpgradingPlayer
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            int index = 0;

            Label returnLabel = generator.DefineLabel();

            LocalBuilder curSetting = generator.DeclareLocal(typeof(Scp914KnobSetting));
            LocalBuilder ev = generator.DeclareLocal(typeof(UpgradingPlayerEventArgs));

            newInstructions.RemoveRange(index, 15);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // Player.Get(ply)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // upgradeInventory
                new(OpCodes.Ldarg_1),

                // heldOnly
                new(OpCodes.Ldarg_2),

                // setting
                new(OpCodes.Ldarg_S, 4),

                // moveVector
                new(OpCodes.Ldarg_3),

                // var ev = new UpgradingPlayerEventArgs(player, upgradeInventory, heldonly, setting, moveVector);
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpgradingPlayerEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Handlers.Scp914.OnUpgradingPlayer(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp914), nameof(Handlers.Scp914.OnUpgradingPlayer))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, returnLabel),
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

                // curSetting = setting;
                new(OpCodes.Ldarg, 4),
                new(OpCodes.Stloc_S, curSetting.LocalIndex),

                // ev.Player.Teleport(ev.OutputPosition);
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.Player))),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingPlayerEventArgs), nameof(UpgradingPlayerEventArgs.OutputPosition))),
                new(OpCodes.Callvirt, Method(typeof(Player), nameof(Player.Teleport), new[] { typeof(Vector3) })),
            });

            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev2 = generator.DeclareLocal(typeof(UpgradingInventoryItemEventArgs));

            int addOffset = 2;
            index = newInstructions.FindLastIndex(instr => instr.Calls(Method(typeof(Scp914Upgrader), nameof(Scp914Upgrader.TryGetProcessor)))) + addOffset;

            int cntIndex = newInstructions.FindLastIndex(index, i => i.opcode == OpCodes.Br_S);

            newInstructions[cntIndex].labels.Add(continueLabel);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // setting = curSetting
                new CodeInstruction(OpCodes.Ldloc_S, curSetting.LocalIndex).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Starg_S, 4),

                // Player.Get(ply)
                new(OpCodes.Ldarg_0),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                // ItemBase item = GetItem
                new(OpCodes.Ldloc_S, 7),

                // setting
                new(OpCodes.Ldarg_S, 4),
                new(OpCodes.Ldc_I4_1),

                // var ev = new UpgradingInventoryItemEventArgs(player, item, setting)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UpgradingInventoryItemEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev2.LocalIndex),

                // Handlers.Scp914.OnUpgradingInventoryItem(ev);
                new(OpCodes.Call, Method(typeof(Handlers.Scp914), nameof(Handlers.Scp914.OnUpgradingInventoryItem))),

                // if (!ev.IsAllowed)
                //    return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingInventoryItemEventArgs), nameof(UpgradingInventoryItemEventArgs.IsAllowed))),
                new(OpCodes.Brfalse_S, continueLabel),
                new(OpCodes.Ldloc_S, ev2.LocalIndex),

                // setting = ev.KnobSetting
                new(OpCodes.Callvirt, PropertyGetter(typeof(UpgradingInventoryItemEventArgs), nameof(UpgradingInventoryItemEventArgs.KnobSetting))),
                new(OpCodes.Starg_S, 4),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
