// -----------------------------------------------------------------------
// <copyright file="LocalReportEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using System;

    /// <summary>
    ///     Contains information about the report to local administrators.
    /// </summary>
    public class LocalReportEventArgs : EventArgs
    {
        /// <param name="issuer"><inheritdoc cref="Issuer"/></param>
        /// <param name="target"><inheritdoc cref="Target"/></param>
        /// <param name="reason"><inheritdoc cref="Reason"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public LocalReportEventArgs(API.Features.Player issuer, API.Features.Player target, string reason, bool isAllowed = true)
        {
            Issuer = issuer;
            Target = target;
            Reason = reason;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Report sender.
        /// </summary>
        public API.Features.Player Issuer { get; internal set; }

        /// <summary>
        ///     Target of the report.
        /// </summary>
        public API.Features.Player Target { get; internal set; }

        /// <summary>
        ///     Gets or sets the report reason.
        /// </summary>
        public string Reason { get; internal set; }

        /// <summary>
        ///     Returns or sets a value indicating whether the process can be processed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}