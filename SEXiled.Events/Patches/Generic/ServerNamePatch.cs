// -----------------------------------------------------------------------
// <copyright file="ServerNamePatch.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Generic
{
    using HarmonyLib;

    using static SEXiled.Events.Events;

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

            ServerConsole._serverName += $"<color=#00000000><size=1>SEXiled {Instance.Version.ToString(3)}</size></color>";
        }
    }
}
