// -----------------------------------------------------------------------
// <copyright file="RankKind.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CreditTags.Enums
{
    /// <summary>
    /// Represents all existing ranks.
    /// </summary>
    public enum RankType
    {
        /// <summary>
        /// No EXILED Roles.
        /// </summary>
        None,

        /// <summary>
        /// Exiled Developer.
        /// </summary>
        Dev = 1,

        /// <summary>
        /// Exiled Contributor.
        /// </summary>
        Contributor = 2,

        /// <summary>
        /// Exiled Plugin Developer.
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

        /// <summary>
        /// EXILED Donator.
        /// </summary>
        Donator = 6,
    }
}