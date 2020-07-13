// -----------------------------------------------------------------------
// <copyright file="WaitingForPlayers.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using Exiled.Events.Handlers;
    using Exiled.Events.Patches.Events.Map;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="ServerConsole.AddLog(string, System.ConsoleColor)"/>.
    /// Adds the WaitingForPlayers event.
    /// </summary>
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.AddLog))]
    internal static class WaitingForPlayers
    {
        private static void Prefix(string q)
        {
            if (q == "Waiting for players...")
            {
                AnnouncingDecontamination.StopAnnouncing = false;
                Server.OnWaitingForPlayers();
            }
        }
    }
}
