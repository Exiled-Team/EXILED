// -----------------------------------------------------------------------
// <copyright file="EndingRoundEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Server
{
    using Exiled.API.Enums;
    using Exiled.Events.EventArgs.Interfaces;

    /// <summary>
    ///     Contains all information before ending a round.
    /// </summary>
    public class EndingRoundEventArgs : IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EndingRoundEventArgs" /> class.
        /// </summary>
        /// <param name="classList">
        ///     <inheritdoc cref="RoundSummary.SumInfo_ClassList" />
        /// </param>
        /// <param name="leadingTeam">
        ///     <inheritdoc cref="LeadingTeam" />
        /// </param>
        /// <param name="isRoundEnded">
        ///     <inheritdoc cref="IsRoundEnded" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public EndingRoundEventArgs(LeadingTeam leadingTeam, RoundSummary.SumInfo_ClassList classList, bool isRoundEnded, bool isAllowed = true)
        {
            ClassList = classList;
            LeadingTeam = leadingTeam;
            IsRoundEnded = isRoundEnded;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets or sets the round summary class list.
        /// </summary>
        public RoundSummary.SumInfo_ClassList ClassList { get; set; }

        /// <summary>
        ///     Gets or sets the leading team.
        /// </summary>
        public LeadingTeam LeadingTeam { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the round is going to finish or not.
        /// </summary>
        public bool IsRoundEnded { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether the event can be executed or not.
        /// </summary>
        public bool IsAllowed { get; set; }
    }
}
