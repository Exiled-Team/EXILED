// -----------------------------------------------------------------------
// <copyright file="ActiveAbilitySettings.cs" company="Exiled Team">
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
    public class ActiveAbilitySettings : AbilitySettings
    {
        /// <summary>
        /// Gets or sets a value indicating whether <see cref="AbilityInputComponent"/> should be used with this ability.
        /// </summary>
        public virtual bool UseAbilityInputComponent { get; set; }

        /// <summary>
        /// Gets or sets the required cooldown before using the ability again.
        /// </summary>
        public virtual float Cooldown { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the cooldown should be forced when the ability is added, making it already usable.
        /// </summary>
        public virtual bool ForceCooldownOnAdded { get; set; }

        /// <summary>
        /// Gets or sets the time to wait before the ability is activated.
        /// </summary>
        public virtual float WindupTime { get; set; }

        /// <summary>
        /// Gets or sets the duration of the ability.
        /// </summary>
        public virtual float Duration { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability is used.
        /// </summary>
        public virtual TextDisplay Activated { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability activation is denied regardless any conditions.
        /// </summary>
        public virtual TextDisplay CannotBeUsed { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability activation in on cooldown.
        /// </summary>
        public virtual TextDisplay OnCooldown { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability is expired.
        /// </summary>
        public virtual TextDisplay Expired { get; set; }

        /// <summary>
        /// Gets or sets the message to display when the ability is ready.
        /// </summary>
        public virtual TextDisplay OnReady { get; set; }
    }
}