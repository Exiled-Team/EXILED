// -----------------------------------------------------------------------
// <copyright file="SendingRemoteAdminCommand.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
#pragma warning disable SA1313
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using RemoteAdmin;

    /// <summary>
    /// Patches <see cref="CommandProcessor.ProcessQuery(string, CommandSender)"/>.
    /// Adds the <see cref="Handlers.Server.SendingRemoteAdminCommand"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery), typeof(string), typeof(CommandSender))]
    internal static class SendingRemoteAdminCommand
    {
        private static bool Prefix(ref string q, ref CommandSender sender)
        {
            QueryProcessor queryProcessor = sender is PlayerCommandSender playerCommandSender ? playerCommandSender.Processor : null;

            (string name, string[] arguments) = q.ExtractCommand();
            var ev = new SendingRemoteAdminCommandEventArgs(sender, string.IsNullOrEmpty(sender.SenderId) ? Server.Host : Player.Get(sender.SenderId) ?? Server.Host, name, arguments.ToList());

            IdleMode.PreauthStopwatch.Restart();
            IdleMode.SetIdleMode(false);

            if (q.StartsWith("gban-kick", System.StringComparison.OrdinalIgnoreCase))
            {
                if (queryProcessor == null || (!queryProcessor._sender.ServerRoles.RaEverywhere || !queryProcessor._sender.ServerRoles.Staff))
                {
                    sender.RaReply(
                        $"GBAN-KICK# Permission to run command denied by the server. If this is an unexpected error, contact EXILED developers.",
                        false,
                        true,
                        string.Empty);

                    Log.Error($"A user {sender.Nickname} attempted to run GBAN-KICK and was denied permission. If this is an unexpected error, contact EXILED developers.");

                    ev.IsAllowed = false;
                }
            }

            if (q == "REQUEST_DATA PLAYER_LIST SILENT")
                return true;

            Handlers.Server.OnSendingRemoteAdminCommand(ev);

            if (!string.IsNullOrEmpty(ev.ReplyMessage))
                sender.RaReply(ev.ReplyMessage, ev.Success, true, string.Empty);

            return ev.IsAllowed;
        }
    }
}
