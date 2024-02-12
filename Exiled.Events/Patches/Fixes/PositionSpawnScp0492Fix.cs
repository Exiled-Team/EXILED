// -----------------------------------------------------------------------
// <copyright file="PositionSpawnScp0492Fix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using PlayerRoles.PlayableScps.Scp049;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp049ResurrectAbility.ServerComplete"/> delegate.
    /// Removes useless position setter for Scp0492.
    /// </summary>
    [HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.ServerComplete))]
    internal static class PositionSpawnScp0492Fix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int toRemove = 7;

            const int offset = -1;
            int index = newInstructions.FindLastIndex(instruction => instruction.Calls(PropertyGetter(typeof(Component), nameof(Component.transform)))) + offset;

            newInstructions[index + toRemove].MoveLabelsFrom(newInstructions[index]);

            newInstructions.RemoveRange(index, toRemove);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
