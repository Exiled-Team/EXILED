// -----------------------------------------------------------------------
// <copyright file="BanningEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using System.Reflection;

    using API.Features;

    /// <summary>
    /// Contains all information before banning a player from the server.
    /// </summary>
    public class BanningEventArgs : KickingEventArgs
    {
        private long duration;

        /// <summary>
        /// Initializes a new instance of the <see cref="BanningEventArgs" /> class.
        /// </summary>
        /// <param name="target">The ban target.</param>
        /// <param name="issuer">The ban issuer.</param>
        /// <param name="duration">The ban seconds duration.</param>
        /// <param name="reason">The ban reason.</param>
        /// <param name="fullMessage">The ban full message.</param>
        /// <param name="isAllowed">Indicates whether the event can be executed or not.</param>
        public BanningEventArgs(Player target, Player issuer, long duration, string reason, string fullMessage, bool isAllowed = true)
            : base(target, issuer, reason, fullMessage, isAllowed)
        {
            Duration = duration;
        }

        /// <summary>
        /// Gets or sets the ban duration.
        /// </summary>
        public long Duration
        {
            get => duration;
            set
            {
                if (duration == value)
                    return;

                if (Events.Instance.Config.ShouldLogBans)
                    LogBanChange(Assembly.GetCallingAssembly().GetName().Name, $" changed Ban duration: {duration} to {value} for ID: {Target.UserId}");

                duration = value;
            }
        }
    }
}