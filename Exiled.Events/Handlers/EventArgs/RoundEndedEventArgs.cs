// -----------------------------------------------------------------------
// <copyright file="RoundEndedEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.EventArgs
{
    using System;

    /// <summary>
    /// Contains all informations before ending a round.
    /// </summary>
    public class RoundEndedEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RoundEndedEventArgs"/> class.
        /// </summary>
        /// <param name="leadingTeam"><inheritdoc cref="LeadingTeam"/></param>
        /// <param name="classList"><inheritdoc cref="ClassList"/></param>
        /// <param name="timeToRestart"><inheritdoc cref="TimeToRestart"/></param>
        public RoundEndedEventArgs(RoundSummary.LeadingTeam leadingTeam, RoundSummary.SumInfo_ClassList classList, int timeToRestart)
        {
            LeadingTeam = leadingTeam;
            ClassList = classList;
            TimeToRestart = timeToRestart;
        }

        /// <summary>
        /// Gets the leading team.
        /// </summary>
        public RoundSummary.LeadingTeam LeadingTeam { get; private set; }

        /// <summary>
        /// Gets or sets the round summary class list.
        /// </summary>
        public RoundSummary.SumInfo_ClassList ClassList { get; set; }

        /// <summary>
        /// Gets or sets the time to restart the next round.
        /// </summary>
        public int TimeToRestart { get; set; }
    }
}
