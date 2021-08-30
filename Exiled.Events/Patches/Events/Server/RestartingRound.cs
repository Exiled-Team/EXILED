// -----------------------------------------------------------------------
// <copyright file="RestartingRound.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Reflection.Emit;

using Exiled.API.Features;

using NorthwoodLib.Pools;

namespace Exiled.Events.Patches.Events.Server
{
    using HarmonyLib;
    using static HarmonyLib.AccessTools;
    #pragma warning disable

    /// <summary>
    /// Patches <see cref="PlayerStats.Roundrestart"/>.
    /// Adds the RestartingRound event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.Roundrestart))]
    internal static class RestartingRound
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions,
            ILGenerator generator)
        {
            var newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);
            newInstructions.InsertRange(0, new []
            {
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Server), nameof(Handlers.Server.OnRestartingRound))),
                new CodeInstruction(OpCodes.Call, Method(typeof(RestartingRound), nameof(RestartingRound.ShowDebugLine)))
            });
            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);

        }

        private static void ShowDebugLine()
        {
            API.Features.Log.Debug("Round restarting", Loader.Loader.ShouldDebugBeShown);
        }
    }
}
