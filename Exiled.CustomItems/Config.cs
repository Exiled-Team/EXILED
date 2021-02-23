// -----------------------------------------------------------------------
// <copyright file="Config.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems
{
    using System.ComponentModel;

    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.CustomItems.API.Features;

    /// <summary>
    /// The plugin's config class.
    /// </summary>
    public class Config : IConfig
    {
        /// <inheritdoc/>
        [Description("Whether or not this plugin is enabled.")]
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Gets the broadcast that is shown when someone pickups a <see cref="CustomItem"/>.
        /// </summary>
        public Broadcast PickedUpBroadcast { get; private set; } = new Broadcast("You have picked up a {0}\n{1}");

        /// <summary>
        /// Gets a value indicating whether if debug mode is enabled.
        /// </summary>
        [Description("Whether or not debug messages should be displayed in the server console.")]
        public bool Debug { get; private set; } = false;
    }
}
