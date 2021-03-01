// -----------------------------------------------------------------------
// <copyright file="CustomItems.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems
{
    using Exiled.API.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// Handles all CustomItem API.
    /// </summary>
    public class CustomItems : Plugin<Config>
    {
        private static readonly CustomItems Singleton = new CustomItems();

        private RoundHandler roundHandler;

        private CustomItems()
        {
        }

        /// <summary>
        /// Gets the static reference to this <see cref="CustomItems"/> class.
        /// </summary>
        public static CustomItems Instance => Singleton;

        /// <inheritdoc />
        public override void OnEnabled()
        {
            roundHandler = new RoundHandler();

            Events.Handlers.Server.RoundStarted += roundHandler.OnRoundStarted;

            base.OnEnabled();
        }

        /// <inheritdoc />
        public override void OnDisabled()
        {
            Events.Handlers.Server.RoundStarted -= roundHandler.OnRoundStarted;

            roundHandler = null;

            base.OnDisabled();
        }
    }
}
