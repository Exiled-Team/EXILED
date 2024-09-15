// -----------------------------------------------------------------------
// <copyright file="LevelAbilitySettings.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.API.Features.CustomAbilities.Settings
{
    using System.ComponentModel;

    using Exiled.API.Features;

    /// <summary>
    /// Represents the base class for player-specific ability behaviors.
    /// </summary>
    public class LevelAbilitySettings : ActiveAbilitySettings
    {
        /// <summary>
        /// Gets or sets the default level the ability should start from.
        /// </summary>
        [Description("The default level at which the ability starts.")]
        public virtual byte DefaultLevel { get; set; }

        /// <summary>
        /// Gets or sets the maximum level the ability cannot exceed.
        /// </summary>
        [Description("The highest level that the ability can reach.")]
        public virtual byte MaxLevel { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability returns to a previous level.
        /// </summary>
        [Description("The message shown when the ability reverts to a previous level.")]
        public virtual TextDisplay PreviousLevel { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability reaches a new level.
        /// </summary>
        [Description("The message shown when the ability advances to a new level.")]
        public virtual TextDisplay NextLevel { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability has reached the maximum level.
        /// </summary>
        [Description("The message shown when the ability reaches its maximum level.")]
        public virtual TextDisplay MaxLevelReached { get; set; }
    }
}