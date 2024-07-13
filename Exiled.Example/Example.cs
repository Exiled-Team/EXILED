// -----------------------------------------------------------------------
// <copyright file="Example.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example
{
    using Exiled.API.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// The example plugin.
    /// </summary>
    public class Example : Plugin<Config, Translation>
    {
        /// <summary>
        /// Gets the only existing instance of this plugin.
        /// </summary>
        public static Example Instance { get; private set; }

        /// <inheritdoc/>
        public override PluginPriority Priority { get; } = PluginPriority.Last;

        /// <summary>
        /// Gets the current instance of the event handler.
        /// </summary>
        public EventHandler EventHandler { get; private set; }

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            // Set the instance to the current one
            Instance = this;

            // Create new instance of the event handler
            EventHandler = new();

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            // Finishes the event handler
            EventHandler = null;

            base.OnDisabled();
        }
    }
}