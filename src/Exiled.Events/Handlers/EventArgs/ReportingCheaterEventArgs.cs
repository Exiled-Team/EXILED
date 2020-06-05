// -----------------------------------------------------------------------
// <copyright file="ReportingCheaterEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using System;

    /// <summary>
    /// Contains all informations before reporting a cheater.
    /// </summary>
    public class ReportingCheaterEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingCheaterEventArgs"/> class.
        /// </summary>
        /// <param name="reporterUserId"><inheritdoc cref="ReporterUserId"/></param>
        /// <param name="reportedUserId"><inheritdoc cref="ReportedUserId"/></param>
        /// <param name="reportedAuthentication"><inheritdoc cref="ReportedAuthentication"/></param>
        /// <param name="reporterAuthentication"><inheritdoc cref="ReporterAuthentication"/></param>
        /// <param name="reporterIPAddress"><inheritdoc cref="ReporterIPAddress"/></param>
        /// <param name="reportedIPAddress"><inheritdoc cref="ReportedIPAddress"/></param>
        /// <param name="serverPort"><inheritdoc cref="ServerPort"/></param>
        /// <param name="reason"><inheritdoc cref="Reason"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ReportingCheaterEventArgs(
            string reporterUserId,
            string reportedUserId,
            string reporterAuthentication,
            string reportedAuthentication,
            string reporterIPAddress,
            string reportedIPAddress,
            int serverPort,
            string reason,
            bool isAllowed = true)
        {
            ReporterUserId = reporterUserId;
            ReportedUserId = reportedUserId;
            ReporterAuthentication = reporterAuthentication;
            ReportedAuthentication = reportedAuthentication;
            ReporterIPAddress = reporterIPAddress;
            ReportedIPAddress = reportedIPAddress;
            ServerPort = serverPort;
            Reason = reason;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the reporter user id.
        /// </summary>
        public string ReporterUserId { get; private set; }

        /// <summary>
        /// Gets the reported user id.
        /// </summary>
        public string ReportedUserId { get; private set; }

        /// <summary>
        /// Gets the reporter authentication.
        /// </summary>
        public string ReporterAuthentication { get; private set; }

        /// <summary>
        /// Gets the reported authentication.
        /// </summary>
        public string ReportedAuthentication { get; private set; }

        /// <summary>
        /// Gets the reporter ip address.
        /// </summary>
        public string ReporterIPAddress { get; private set; }

        /// <summary>
        /// Gets the reported ip address.
        /// </summary>
        public string ReportedIPAddress { get; private set; }

        /// <summary>
        /// Gets the server id.
        /// </summary>
        public int ServerPort { get; private set; }

        /// <summary>
        /// Gets or sets the report reason.
        /// </summary>
        public string Reason { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}