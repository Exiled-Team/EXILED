// -----------------------------------------------------------------------
// <copyright file="GetAmmoLimitFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;
    using HarmonyLib;
    using InventorySystem.Configs;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches the <see cref="InventoryLimits.GetAmmoLimit(InventorySystem.Items.Armor.BodyArmor, ItemType)"/> delegate.
    /// Sync <see cref="API.Features.Player.SetAmmoLimit(API.Enums.AmmoType, ushort)"/>.
    /// Changes <see cref="ushort.MaxValue"/> to <see cref="ushort.MinValue"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryLimits), nameof(InventoryLimits.GetAmmoLimit), new Type[] { typeof(InventorySystem.Items.Armor.BodyArmor), typeof(ItemType), })]
    internal class GetAmmoLimitFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            int offset = 1;
            int index = newInstructions.FindIndex(x => x.opcode == OpCodes.Endfinally) + offset;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Call, Method(typeof(GetAmmoLimitFix), nameof(GetCustomAmmoLimit))),
                new(OpCodes.Stloc_0),
            });

            foreach (CodeInstruction instruction in newInstructions)
            {
                if (instruction.operand == (object)ushort.MaxValue)
                    instruction.operand = ushort.MinValue;
            }

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private int GetCustomAmmoLimit(API.Features.Player player, int value, ItemType ammotype)
        {
            if (player.ammoLimits is null)
                return value;

            return player.ammoLimits.Find(x => x.AmmoType == ammotype).Limit;
        }
    }
}
