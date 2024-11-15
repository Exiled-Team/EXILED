// -----------------------------------------------------------------------
// <copyright file="ActiveAbilitySettings.cs" company="Exiled Team">
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
    public class ActiveAbilitySettings : AbilitySettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether <see cref="AbilityInputComponent"/> should be used with this ability.
        /// </summary>
        [Description("Indicates whether the AbilityInputComponent should be used with this ability.")]
        public virtual bool UseAbilityInputComponent { get; set; }

        /// <summary>
        /// Gets or sets the required cooldown before using the ability again.
        /// </summary>
        [Description("The required cooldown time in seconds before the ability can be used again.")]
        public virtual float Cooldown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cooldown should be forced when the ability is added, making it already usable.
        /// </summary>
        [Description("Indicates whether the cooldown should be forced when the ability is added, making it immediately usable.")]
        public virtual bool ForceCooldownOnAdded { get; set; }

        /// <summary>
        /// Gets or sets the time to wait before the ability is activated.
        /// </summary>
        [Description("The time to wait before the ability is activated.")]
        public virtual float WindupTime { get; set; }

        /// <summary>
        /// Gets or sets the duration of the ability.
        /// </summary>
        [Description("The duration of the ability's effect.")]
        public virtual float Duration { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability is used.
        /// </summary>
        [Description("The message to display when the ability is activated.")]
        public virtual TextDisplay Activated { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability activation is denied regardless of any conditions.
        /// </summary>
        [Description("The message to display when the ability activation is denied regardless of any conditions.")]
        public virtual TextDisplay CannotBeUsed { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability activation is on cooldown.
        /// </summary>
        [Description("The message to display when the ability is on cooldown.")]
        public virtual TextDisplay OnCooldown { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability is expired.
        /// </summary>
        [Description("The message to display when the ability is expired.")]
        public virtual TextDisplay Expired { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability is ready.
        /// </summary>
        [Description("The message to display when the ability is ready for use.")]
        public virtual TextDisplay OnReady { get; set; }
    }
}