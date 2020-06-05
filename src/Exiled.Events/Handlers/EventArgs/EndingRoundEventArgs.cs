// -----------------------------------------------------------------------
// <copyright file="EndingRoundEventArgs.cs" company="Exiled Team">
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
    public class EndingRoundEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="EndingRoundEventArgs"/> class.
        /// </summary>
        /// <param name="leadingTeam"><inheritdoc cref="LeadingTeam"/></param>
        /// <param name="isRoundEnded"><inheritdoc cref="IsRoundEnded"/></param>
        /// <param name="isAllowed"><inheritdoc cref="IsAllowed"/></param>
        public EndingRoundEventArgs(RoundSummary.LeadingTeam leadingTeam, bool isRoundEnded, bool isAllowed = true)
        {
            LeadingTeam = leadingTeam;
            IsRoundEnded = isRoundEnded;
            IsAllowed = isAllowed;
        }

        /// <summary>
        /// Gets or sets the leading team.
        /// </summary>
        public RoundSummary.LeadingTeam LeadingTeam { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the round is going to finish or not.
        /// </summary>
        public bool IsRoundEnded { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}