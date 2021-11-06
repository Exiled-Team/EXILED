// -----------------------------------------------------------------------
// <copyright file="AddPlayerPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Dummies
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerManager.AddPlayer(UnityEngine.GameObject, int)"/>.
    /// </summary>
    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.AddPlayer))]
    internal static class AddPlayerPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stsfld);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Dummy), nameof(Dummy.List))),
                new CodeInstruction(OpCodes.Call, Method(typeof(Enumerable), nameof(Enumerable.Count)).MakeGenericMethod(typeof(Dummy))),
                new CodeInstruction(OpCodes.Sub),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
