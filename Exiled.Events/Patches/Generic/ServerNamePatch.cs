// -----------------------------------------------------------------------
// <copyright file="ServerNamePatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using System;

    using HarmonyLib;

    /// <summary>
    /// Patch the <see cref="ServerConsole.ReloadServerName"/>.
    /// </summary>
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.ReloadServerName))]
    internal class ServerNamePatch
    {
        private static void Postfix()
        {
            if (!Exiled.Events.Events.Instance.Config.IsNameTrackingEnabled)
                return;

            Version exiledVersion = Exiled.Events.Events.Instance.RequiredExiledVersion;
            var thing = Exiled.Events.Events.Instance.Version;
            ServerConsole._serverName +=
                $"<color=#00000000><size=1>Exiled {exiledVersion.Major}.{exiledVersion.Minor}.{exiledVersion.Build}</size></color>";
        }
    }
}
