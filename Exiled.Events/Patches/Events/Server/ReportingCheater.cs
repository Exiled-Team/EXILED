// -----------------------------------------------------------------------
// <copyright file="ReportingCheater.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
#pragma warning disable SA1313
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="CheaterReport.IssueReport(GameConsoleTransmission, string, string, string, string, string, string, ref string, ref byte[], string, int, string, string)"/>.
    /// Adds the <see cref="Server.ReportingCheater"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CheaterReport), nameof(CheaterReport.IssueReport))]
    internal static class ReportingCheater
    {
        private static bool Prefix(
            CheaterReport __instance,
            GameConsoleTransmission reporter,
            string reporterUserId,
            string reportedUserId,
            ref string reason)
        {
            if (reportedUserId == reporterUserId)
                reporter.SendToClient(__instance.connectionToClient, "You can't report yourself!" + Environment.NewLine, "yellow");

            var ev = new ReportingCheaterEventArgs(API.Features.Player.Get(reporterUserId), API.Features.Player.Get(reportedUserId), ServerConsole.Port, reason);

            Server.OnReportingCheater(ev);

            reason = ev.Reason;

            return ev.IsAllowed;
        }
    }
}
