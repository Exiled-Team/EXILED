// -----------------------------------------------------------------------
// <copyright file="LockerList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;
    using API.Features.Core.Generic.Pools;

    using HarmonyLib;

    using MapGeneration.Distributors;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Locker.Start"/>.
    /// </summary>
    [HarmonyPatch(typeof(Locker), nameof(Locker.Start))]
    internal class LockerList
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(codeInstructions);

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // API.Features.Lockers.Locker.Get(this)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, Method(typeof(API.Features.Lockers.Locker), nameof(API.Features.Lockers.Locker.Get), new[] { typeof(Locker) })),
                    new(OpCodes.Pop),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}