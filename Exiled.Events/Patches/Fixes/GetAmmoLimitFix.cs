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

    using API.Features.Pools;
    using Footprinting;
    using HarmonyLib;
    using InventorySystem;
    using InventorySystem.Configs;
    using InventorySystem.Items.Firearms.Ammo;
    using InventorySystem.Items.Pickups;
    using VoiceChat;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="InventoryLimits.GetAmmoLimit(InventorySystem.Items.Armor.BodyArmor, ItemType)"/> delegate.
    /// Fix than NW return <see cref="ushort.MaxValue"/> instead of <see cref="ushort.MinValue"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryLimits), nameof(InventoryLimits.GetAmmoLimit), new Type[] { typeof(InventorySystem.Items.Armor.BodyArmor), typeof(ItemType), })]
    internal class GetAmmoLimitFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            foreach (CodeInstruction instruction in newInstructions)
            {
                if (instruction.operand == (object)ushort.MaxValue)
                    instruction.operand = ushort.MinValue;
            }

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
