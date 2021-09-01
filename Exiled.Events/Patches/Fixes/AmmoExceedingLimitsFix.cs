// -----------------------------------------------------------------------
// <copyright file="AmmoExceedingLimitsFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

    using InventorySystem.Items.Firearms;

    using NorthwoodLib.Pools;

    /// <summary>
    /// Fixes player dropping ammo, when it exceeds ammo limit.
    /// </summary>
    [HarmonyPatch(typeof(Firearm), nameof(Firearm.OnHolstered))]
    internal static class AmmoExceedingLimitsFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            const int offset = 1;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Callvirt) + offset;

            newInstructions.RemoveRange(index, 15);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
