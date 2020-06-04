// -----------------------------------------------------------------------
// <copyright file="ReportingCheater.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using System;
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="CheaterReport.IssueReport(GameConsoleTransmission, string, string, string, string, string, string, ref string, ref byte[], string, int)"/>.
    /// Adds the <see cref="Server.ReportingCheater"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CheaterReport), nameof(CheaterReport.IssueReport))]
    public class ReportingCheater
    {
        /// <summary>
        /// Prefix of <see cref="CheaterReport.IssueReport(GameConsoleTransmission, string, string, string, string, string, string, ref string, ref byte[], string, int)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="CheaterReport"/> instance.</param>
        /// <param name="reporter">The reporter.</param>
        /// <param name="reporterUserId"><inheritdoc cref="ReportingCheaterEventArgs.ReporterUserId"/></param>
        /// <param name="reportedUserId"><inheritdoc cref="ReportingCheaterEventArgs.ReportedUserId"/></param>
        /// <param name="reportedAuth"><inheritdoc cref="ReportingCheaterEventArgs.ReportedAuthentication"/></param>
        /// <param name="reportedIp"><inheritdoc cref="ReportingCheaterEventArgs.ReportedIPAddress"/></param>
        /// <param name="reporterAuth"><inheritdoc cref="ReportingCheaterEventArgs.ReporterAuthentication"/></param>
        /// <param name="reporterIp"><inheritdoc cref="ReportingCheaterEventArgs.ReporterIPAddress"/></param>
        /// <param name="reason"><inheritdoc cref="ReportingCheaterEventArgs.Reason"/></param>
        /// <param name="signature">The signature.</param>
        /// <param name="reporterPublicKey">The reporter public key.</param>
        /// <param name="reportedId">The reported player id.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(
            CheaterReport __instance,
            GameConsoleTransmission reporter,
            string reporterUserId,
            string reportedUserId,
            string reportedAuth,
            string reportedIp,
            string reporterAuth,
            string reporterIp,
            ref string reason,
            byte[] signature,
            string reporterPublicKey,
            int reportedId)
        {
            if (reportedUserId == reporterUserId)
                reporter.SendToClient(__instance.connectionToClient, "You can't report yourself!" + Environment.NewLine, "yellow");

            var ev = new ReportingCheaterEventArgs(reporterUserId, reportedUserId, reporterAuth, reportedAuth, reporterIp, reportedIp, ServerConsole.Port, reason);

            Server.OnReportingCheater(ev);

            reason = ev.Reason;

            return ev.IsAllowed;
        }
    }
}