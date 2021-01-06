// -----------------------------------------------------------------------
// <copyright file="RoundStarted.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
#pragma warning disable SA1313
    using Exiled.Events.Handlers;

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
                Server.OnRoundStarted();
        }
    }
}
