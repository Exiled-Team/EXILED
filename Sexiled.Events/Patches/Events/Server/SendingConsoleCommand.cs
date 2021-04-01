// -----------------------------------------------------------------------
// <copyright file="SendingConsoleCommand.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.Events.EventArgs;
using Sexiled.Events.Handlers;

namespace Sexiled.Events.Patches.Events.Server
{
#pragma warning disable SA1313
    using System;
    using System.Linq;

    using Sexiled.API.Extensions;
    using Sexiled.Events.EventArgs;
    using Sexiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="RemoteAdmin.QueryProcessor.ProcessGameConsoleQuery(string, bool)"/>.
    /// Adds the <see cref="Server.SendingConsoleCommand"/> event.
    /// </summary>
    [HarmonyPatch(typeof(RemoteAdmin.QueryProcessor), nameof(RemoteAdmin.QueryProcessor.ProcessGameConsoleQuery), new Type[] { typeof(string), typeof(bool) })]
    internal static class SendingConsoleCommand
    {
        private static bool Prefix(RemoteAdmin.QueryProcessor __instance, ref string query, bool encrypted)
        {
            (string name, string[] arguments) = query.ExtractCommand();
            var ev = new SendingConsoleCommandEventArgs(API.Features.Player.Get(__instance.gameObject), name, arguments.ToList(), encrypted);

            Handlers.Server.OnSendingConsoleCommand(ev);

            if (!string.IsNullOrEmpty(ev.ReturnMessage))
                __instance.GCT.SendToClient(__instance.connectionToClient, ev.ReturnMessage, ev.Color);

            return ev.IsAllowed;
        }
    }
}
