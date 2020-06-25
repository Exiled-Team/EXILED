// -----------------------------------------------------------------------
// <copyright file="ServerSendingConsoleCommand.cs" company="Exiled Team">
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
    using Console = GameCore.Console;

    /// <summary>
    /// Patches <see cref="Console.TypeCommand(string, CommandSender)"/>.
    /// Adds the <see cref="Server.SendingRemoteAdminCommand"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Console), nameof(Console.TypeCommand), new Type[] { typeof(string), typeof(CommandSender) })]
    public class ServerSendingConsoleCommand
    {
        /// <summary>
        /// Prefix of <see cref="Console.TypeCommand(string, CommandSender)"/>.
        /// </summary>
        /// <param name="cmd">The command sent to with the Remote Admin console.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(string cmd)
        {
            (string name, string[] arguments) = cmd.ExtractCommand();

            API.Features.Player player = API.Features.Player.Get(Console._ccs.SenderId);

            var ev = new SendingRemoteAdminCommandEventArgs(player?.CommandSender ?? API.Features.Server.Host.CommandSender, player ?? API.Features.Server.Host, name, arguments.ToList());

            Server.OnSendingRemoteAdminCommand(ev);

            return ev.IsAllowed;
        }
    }
}
