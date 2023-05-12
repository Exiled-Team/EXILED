// -----------------------------------------------------------------------
// <copyright file="Scp2176Fix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using API.Features.Pools;

    using HarmonyLib;
    using InventorySystem.Items.ThrowableProjectiles;

    /// <summary>
    /// Patch the Scp2176Projectile.ServerFuseEnd.
    /// Fix Pickup being destroy too early.
    /// </summary>
    [HarmonyPatch(typeof(Scp2176Projectile), nameof(Scp2176Projectile.ServerFuseEnd))]
    internal static class Scp2176Fix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int offset = 1;
            int index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Stfld) + offset;
            newInstructions.InsertRange(index + 4, newInstructions.GetRange(index, 2));
            newInstructions.RemoveRange(index, 2);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}