// -----------------------------------------------------------------------
// <copyright file="RoundStarted.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Sexiled.Events.Patches.Events.Server
{
#pragma warning disable SA1313
    using Sexiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.CallRpcRoundStarted"/>.
    /// Adds the RoundStarted event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.CallRpcRoundStarted))]
    internal static class RoundStarted
    {
        private static void Postfix(CharacterClassManager __instance)
        {
            if (__instance._hub.isDedicatedServer)
                Handlers.Server.OnRoundStarted();
        }
    }
}
