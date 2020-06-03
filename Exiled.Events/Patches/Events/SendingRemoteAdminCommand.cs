// -----------------------------------------------------------------------
// <copyright file="SendingRemoteAdminCommand.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using System.Linq;
    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using RemoteAdmin;

    /// <summary>
    /// Patches <see cref="CommandProcessor.ProcessQuery(string, CommandSender)"/>.
    /// Adds the <see cref="Handlers.Server.SendingRemoteAdminCommand"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CommandProcessor), nameof(CommandProcessor.ProcessQuery), typeof(string), typeof(CommandSender))]
    public class SendingRemoteAdminCommand
    {
        /// <summary>
        /// Prefix of <see cref="CommandProcessor.ProcessQuery(string, CommandSender)"/>.
        /// </summary>
        /// <param name="q">The command query.</param>
        /// <param name="sender">The command sender.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(ref string q, ref CommandSender sender)
        {
            QueryProcessor queryProcessor = sender is PlayerCommandSender playerCommandSender ? playerCommandSender.Processor : null;

            (string name, string[] arguments) = q.ExtractCommand();
            var ev = new SendingRemoteAdminCommandEventArgs(string.IsNullOrEmpty(sender.SenderId) ? null : Player.Get(sender.SenderId), name, arguments.ToList());

            if (q.ToLower().StartsWith("gban-kick"))
            {
                if (queryProcessor == null || !queryProcessor._sender.SR.RaEverywhere)
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

            if (q.Contains("REQUEST_DATA PLAYER_LIST SILENT"))
                return true;

            Handlers.Server.OnSendingRemoteAdminCommand(ev);

            return ev.IsAllowed;
        }
    }
}