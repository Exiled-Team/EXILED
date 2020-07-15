// -----------------------------------------------------------------------
// <copyright file="ReportingCheaterEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs
{
    using System;

    using Exiled.API.Features;

    /// <summary>
    /// Contains all informations before reporting a cheater.
    /// </summary>
    public class ReportingCheaterEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ReportingCheaterEventArgs"/> class.
        /// </summary>
        /// <param name="reporter"><inheritdoc cref="Reporter"/></param>
        /// <param name="reported"><inheritdoc cref="Reported"/></param>
        /// <param name="serverPort"><inheritdoc cref="ServerPort"/></param>
        /// <param name="reason"><inheritdoc cref="Reason"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public ReportingCheaterEventArgs(
            Player reporter,
            Player reported,
            int serverPort,
            string reason,
            bool isAllowed = true)
        {
            Reporter = reporter;
            Reported = reported;
            ServerPort = serverPort;
            Reason = reason;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets the reporter player.
        /// </summary>
        public Player Reporter { get; private set; }

        /// <summary>
        /// Gets the reported player.
        /// </summary>
        public Player Reported { get; private set; }

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
