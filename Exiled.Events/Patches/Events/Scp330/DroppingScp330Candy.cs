// -----------------------------------------------------------------------
// <copyright file="DroppingScp330Candy.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp330
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp330;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp330Bag.OnRemoved"/>.
    /// Adds the <see cref="Handlers.Scp330.DroppingScp330Candy"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp330Bag), nameof(Scp330Bag.OnRemoved))]
    internal static class DroppingScp330Candy
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {

        }
    }
}
