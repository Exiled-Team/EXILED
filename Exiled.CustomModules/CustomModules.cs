// -----------------------------------------------------------------------
// <copyright file="CustomModules.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules
{
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features;
    using Exiled.CustomModules.Events;

    /// <summary>
    /// Handles all custom role API functions.
    /// </summary>
    public class CustomModules : Plugin<Config>
    {
        private KeypressActivator keypressActivator;

        /// <summary>
        /// Gets a static reference to the plugin's instance.
        /// </summary>
        public static CustomModules Instance { get; private set; } = null!;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            if (Config.UseKeypressActivation)
                keypressActivator = new();

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            keypressActivator = null;
            base.OnDisabled();
        }
    }
}