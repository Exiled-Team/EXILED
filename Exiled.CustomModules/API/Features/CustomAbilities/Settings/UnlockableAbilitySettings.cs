// -----------------------------------------------------------------------
// <copyright file="UnlockableAbilitySettings.cs" company="Exiled Team">
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
    public class UnlockableAbilitySettings : LevelAbilitySettings
    {
        /// <summary>
        /// Gets or sets the message to display when the ability is unlocked.
        /// </summary>
        [Description("The message to display when the ability is unlocked.")]
        public virtual TextDisplay Unlocked { get; set; }
    }
}