// -----------------------------------------------------------------------
// <copyright file="ServerNamePatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using HarmonyLib;

    using static Exiled.Events.Events;

    /// <summary>
    /// Patch the <see cref="ServerConsole.ReloadServerName"/>.
    /// </summary>
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.ReloadServerName))]
    internal static class ServerNamePatch
    {
        private static void Postfix()
        {
            if (!Instance.Config.IsNameTrackingEnabled)
                return;

            ServerConsole._serverName += $"<color=#00000000><size=1>Exiled {Instance.RequiredExiledVersion.ToString(3)}</size></color>";
        }
    }
}
