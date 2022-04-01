// -----------------------------------------------------------------------
// <copyright file="CustomRoles.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomRoles {
    using System.Collections.Generic;

    using Exiled.API.Features;
    using Exiled.CustomRoles.Events;

    /// <summary>
    /// Handles all custom role API functions.
    /// </summary>
    public class CustomRoles : Plugin<Config> {
        private PlayerHandlers playerHandlers;

        /// <summary>
        /// Gets a static reference to the plugin's instance.
        /// </summary>
        public static CustomRoles Instance { get; private set; }

        /// <summary>
        /// Gets a list of players to stop spawning ragdolls for.
        /// </summary>
        internal List<Player> StopRagdollPlayers { get; } = new List<Player>();

        /// <inheritdoc/>
        public override void OnEnabled() {
            Instance = this;
            playerHandlers = new PlayerHandlers(this);

            Exiled.Events.Handlers.Player.SpawningRagdoll += playerHandlers.OnSpawningRagdoll;
            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled() {
            Exiled.Events.Handlers.Player.SpawningRagdoll -= playerHandlers.OnSpawningRagdoll;
            playerHandlers = null;
            Instance = null;
            base.OnDisabled();
        }
    }
}
