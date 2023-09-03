// -----------------------------------------------------------------------
// <copyright file="EarningAchievementEventArgs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.EventArgs.Player
{
    using Achievements;
    using Exiled.API.Features;
    using Interfaces;
    using Respawning.NamingRules;

    /// <summary>
    ///     Contains all information before a player earns an achievement.
    /// </summary>
    public class EarningAchievementEventArgs : IPlayerEvent, IDeniableEvent
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="EarningAchievementEventArgs"/> class.
        /// </summary>
        /// <param name="player">
        ///     <inheritdoc cref="Player" />
        /// </param>
        /// <param name="achievementName">
        ///     <inheritdoc cref="AchievementName" />
        /// </param>
        /// <param name="isAllowed">
        ///     <inheritdoc cref="IsAllowed" />
        /// </param>
        public EarningAchievementEventArgs(Player player, AchievementName achievementName, bool isAllowed = true)
        {
            Player = player;
            AchievementName = achievementName;
            IsAllowed = isAllowed;
        }

        /// <summary>
        ///     Gets the achievement that will be earned.
        /// </summary>
        public AchievementName AchievementName { get; }

        /// <summary>
        ///     Gets or sets a value indicating whether the achievement will be awarded to the player.
        /// </summary>
        public bool IsAllowed { get; set; }

        /// <summary>
        ///     Gets the player who earned the achievement.
        /// </summary>
        public Player Player { get; }
    }
}