// -----------------------------------------------------------------------
// <copyright file="CustomModules.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules
{
    using Exiled.API.Features;
    using Exiled.CustomModules.API.Features;
    using Exiled.CustomModules.EventHandlers;

    /// <summary>
    /// Handles all custom role API functions.
    /// </summary>
    public class CustomModules : Plugin<Config>
    {
        /// <summary>
        /// Gets a static reference to the plugin's instance.
        /// </summary>
        public static CustomModules Instance { get; private set; }

        /// <summary>
        /// Gets the <see cref="EventHandlers.PlayerHandler"/>.
        /// </summary>
        internal PlayerHandler PlayerHandler { get; private set; }

        /// <summary>
        /// Gets the <see cref="EventHandlers.ServerHandler"/>.
        /// </summary>
        internal ServerHandler ServerHandler { get; private set; }

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Instance = this;

            Exiled.Events.Patches.Events.Player.Joined.BasePlayerType = typeof(Pawn);

            SubscribeEvents();

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            UnsubscribeEvents();

            Exiled.Events.Patches.Events.Player.Joined.BasePlayerType = typeof(Player);

            base.OnDisabled();
        }

        private void SubscribeEvents()
        {
            PlayerHandler = new();
            ServerHandler = new();

            Exiled.Events.Handlers.Player.ChangingItem += PlayerHandler.OnChangingItem;
            Exiled.Events.Handlers.Server.RoundStarted += ServerHandler.OnRoundStarted;
        }

        private void UnsubscribeEvents()
        {
            Exiled.Events.Handlers.Player.ChangingItem -= PlayerHandler.OnChangingItem;
            Exiled.Events.Handlers.Server.RoundStarted -= ServerHandler.OnRoundStarted;

            PlayerHandler = null;
            ServerHandler = null;
        }
    }
}