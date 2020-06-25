// -----------------------------------------------------------------------
// <copyright file="SendingConsoleCommand.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
#pragma warning disable SA1313
    using System;
    using System.Linq;
    using Exiled.API.Extensions;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="RemoteAdmin.QueryProcessor.ProcessGameConsoleQuery(string, bool)"/>.
    /// Adds the <see cref="Server.SendingConsoleCommand"/> event.
    /// </summary>
    [HarmonyPatch(typeof(RemoteAdmin.QueryProcessor), nameof(RemoteAdmin.QueryProcessor.ProcessGameConsoleQuery), new Type[] { typeof(string), typeof(bool) })]
    public class SendingConsoleCommand
    {
        /// <summary>
        /// Prefix of <see cref="RemoteAdmin.QueryProcessor.ProcessGameConsoleQuery(string, bool)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="RemoteAdmin.QueryProcessor"/> instance.</param>
        /// <param name="query">The query to be executed.</param>
        /// <param name="encrypted"><inheritdoc cref="SendingConsoleCommandEventArgs.IsEncrypted"/></param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(RemoteAdmin.QueryProcessor __instance, ref string query, bool encrypted)
        {
            (string name, string[] arguments) = query.ExtractCommand();
            var ev = new SendingConsoleCommandEventArgs(API.Features.Player.Get(__instance.gameObject), name, arguments.ToList(), encrypted);

            Server.OnSendingConsoleCommand(ev);

            if (!string.IsNullOrEmpty(ev.ReturnMessage))
                __instance.GCT.SendToClient(__instance.connectionToClient, ev.ReturnMessage, ev.Color);

            return false;
        }
    }
}
