// -----------------------------------------------------------------------
// <copyright file="RoleChangedPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;
    using InventorySystem;
    using NorthwoodLib.Pools;

    /// <summary>
    /// Patches <see cref="InventoryItemProvider.RoleChanged"/> to help override in <see cref="EventArgs.ChangingRoleEventArgs.Items"/> and <see cref="EventArgs.ChangingRoleEventArgs.Ammo"/>.
    /// </summary>
    [HarmonyPatch(typeof(InventoryItemProvider), nameof(InventoryItemProvider.RoleChanged))]
    internal static class RoleChangedPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.Clear();
            newInstructions.Add(new CodeInstruction(OpCodes.Ret));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
