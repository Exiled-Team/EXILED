// -----------------------------------------------------------------------
// <copyright file="RankKind.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.CreditTags.Enums
{
    /// <summary>
    /// Represents all existing ranks.
    /// </summary>
    public enum RankType
    {
        /// <summary>
        /// Nothing.
        /// </summary>
        None,

        /// <summary>
        /// SEXiled Developer.
        /// </summary>
        Dev = 1,

        /// <summary>
        /// SEXiled Contributor.
        /// </summary>
        Contributor = 2,

        /// <summary>
        /// SEXiled Plugin Developer.
        /// </summary>
        PluginDev = 3,

        /// <summary>
        /// EXILED Tournament Participant.
        /// </summary>
        TournamentParticipant = 4,

        /// <summary>
        /// EXILED Tournament Champion.
        /// </summary>
        TournamentChampion = 5,
    }
}
