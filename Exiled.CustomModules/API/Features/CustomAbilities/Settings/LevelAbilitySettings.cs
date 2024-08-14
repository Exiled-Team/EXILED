// -----------------------------------------------------------------------
// <copyright file="LevelAbilitySettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomAbilities.Settings
{
    using Exiled.API.Features;

    /// <summary>
    /// Represents the base class for player-specific ability behaviors.
    /// </summary>
    public class LevelAbilitySettings : ActiveAbilitySettings
    {
        /// <summary>
        /// Gets or sets the default level the ability should start from.
        /// </summary>
        public virtual byte DefaultLevel { get; set; }

        /// <summary>
        /// Gets or sets the maxiumum level the ability cannot exceed.
        /// </summary>
        public virtual byte MaxLevel { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability returns to a previous level.
        /// </summary>
        public virtual TextDisplay PreviousLevel { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability reaches a new level.
        /// </summary>
        public virtual TextDisplay NextLevel { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability reached the maximum level.
        /// </summary>
        public virtual TextDisplay MaxLevelReached { get; set; }
    }
}