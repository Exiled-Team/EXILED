// -----------------------------------------------------------------------
// <copyright file="RestartingRound.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
#pragma warning disable SA1313
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="PlayerStats.Roundrestart"/>.
    /// Adds the RestartingRound event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.Roundrestart))]
    internal static class RestartingRound
    {
        private static void Prefix()
        {
            API.Features.Log.Debug("Round restarting", Loader.Loader.ShouldDebugBeShown);

            Handlers.Server.OnRestartingRound();
        }
    }
}
