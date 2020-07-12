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
    public class WaitingForPlayers
    {
        /// <summary>
        /// Prefix of <see cref="ServerConsole.AddLog(string, System.ConsoleColor)"/>.
        /// </summary>
        /// <param name="q">The message from the server console.</param>
        public static void Prefix(ref string q)
        {
            if (q == "Waiting for players...")
            {
                Server.OnWaitingForPlayers();
            }
        }
    }
}
