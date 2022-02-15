// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.CustomRoles
{
    using System.ComponentModel;
    using SEXiled.API.Features;
    using SEXiled.API.Interfaces;
    using SEXiled.CustomRoles.API.Features;

    /// <summary>
    /// The plugin's config.
    /// </summary>
    public class Config : IConfig
    {
        /// <inheritdoc/>
        [Description("Whether or not the plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets or sets a value indicating whether debug messages should be printed to the console.
        /// </summary>
        /// <returns><see cref="bool"/>.</returns>
        [Description("Whether or not debug messages should be shown.")]
        public bool Debug { get; set; } = false;

        /// <summary>
        /// Gets the hint that is shown when someone gets a <see cref="CustomRole"/>.
        /// </summary>
        [Description("The hint that is shown when someone gets a custom role.")]
        public Broadcast GotRoleHint { get; private set; } = new Broadcast("You have spawned as a {0}\n{1}", 6);

        /// <summary>
        /// Gets the hint that is shown when someone used a <see cref="CustomAbility"/>.
        /// </summary>
        [Description("The hint that is shown when someone used a custom ability.")]
        public Broadcast UsedAbilityHint { get; private set; } = new Broadcast("Ability {0} has been activated.\n{1}", 5);
    }
}
