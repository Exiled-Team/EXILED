// -----------------------------------------------------------------------
// <copyright file="RoundStarted.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
#pragma warning disable SA1313
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.Handlers;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    /// <summary>
    /// Patches <see cref="RoundSummary.SetStartClassList"/>.
    /// Adds the RoundStarted event.
    /// </summary>
    [HarmonyPatch(typeof(RoundSummary), nameof(RoundSummary.SetStartClassList))]
    internal static class RoundStarted
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstruction = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstruction.InsertRange(newInstruction.Count - 1, new[]
            {
                new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(Server), nameof(Server.OnRoundStarted))),
            });

            for (int z = 0; z < newInstruction.Count; z++)
                yield return newInstruction[z];

            ListPool<CodeInstruction>.Shared.Return(newInstruction);
        }
    }
}
