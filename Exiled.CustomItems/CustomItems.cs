// -----------------------------------------------------------------------
// <copyright file="CustomItems.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems
{
    using System;
    using System.Collections.Generic;
    using Exiled.API.Features;
    using Exiled.CustomItems.API;
    using ServerEvents = Exiled.Events.Handlers.Server;

    /// <summary>
    /// Handles all CustomItem API.
    /// </summary>
    public class CustomItems : Plugin<Config>
    {
        /// <summary>
        /// Gets the static reference to this <see cref="CustomItems"/> class.
        /// </summary>
        public static CustomItems Singleton { get; private set; }

        /// <summary>
        /// Gets the Random object used for random number generation.
        /// </summary>
        public Random Rng { get; } = new Random();

        /// <summary>
        /// Gets the EventHandlers class.
        /// </summary>
        public EventHandlers EventHandlers { get; private set; }

        /// <summary>
        /// Gets the list of current Item Managers.
        /// </summary>
        public List<CustomItem> ItemManagers { get; } = new List<CustomItem>();

        /// <inheritdoc />
        public override void OnEnabled()
        {
            Singleton = this;
            EventHandlers = new EventHandlers(this);

            base.OnEnabled();
        }

        /// <inheritdoc />
        public override void OnDisabled()
        {
            EventHandlers = null;

            base.OnDisabled();
        }
    }
}
