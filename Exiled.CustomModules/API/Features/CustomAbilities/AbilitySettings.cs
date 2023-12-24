// -----------------------------------------------------------------------
// <copyright file="AbilitySettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomAbilities
{
    using Exiled.API.Features;
    using Exiled.API.Features.Core;
    using Exiled.API.Features.Core.Interfaces;

    /// <summary>
    /// Represents the base class for player-specific ability behaviors.
    /// </summary>
    public class AbilitySettings : IAdditiveProperty
    {
        /// <summary>
        /// Gets or sets the required cooldown before using the ability again.
        /// </summary>
        public float Cooldown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cooldown should be forced when the ability is added, making it already usable.
        /// </summary>
        public bool ForceCooldownOnAdded { get; set; }

        /// <summary>
        /// Gets or sets the time to wait before the ability is activated.
        /// </summary>
        public float WindupTime { get; set; }

        /// <summary>
        /// Gets or sets the duration of the ability.
        /// </summary>
        public float Duration { get; set; }

        /// <summary>
        /// Gets or sets the default level the ability should start from.
        /// </summary>
        public byte DefaultLevel { get; set; }

        /// <summary>
        /// Gets or sets the maxiumum level the ability cannot exceed.
        /// </summary>
        public byte MaxLevel { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability is used.
        /// </summary>
        public Hint Activated { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability activation is denied regardless any conditions.
        /// </summary>
        public Hint CannotBeUsed { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability activation in on cooldown.
        /// </summary>
        public Hint OnCooldown { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability is expired.
        /// </summary>
        public Hint Expired { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability is unlocked.
        /// </summary>
        public Hint Unlocked { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability returns to a previous level.
        /// </summary>
        public Hint PreviousLevel { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability reaches a new level.
        /// </summary>
        public Hint NextLevel { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability reached the maximum level.
        /// </summary>
        public Hint MaxLevelReached { get; set; }
    }
}