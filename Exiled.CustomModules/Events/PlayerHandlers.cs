// -----------------------------------------------------------------------
// <copyright file="PlayerHandlers.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomModules.Events
{
    using Exiled.Events.EventArgs.Player;

    /// <summary>
    /// Handles general events for players.
    /// </summary>
    public class PlayerHandlers
    {
        private readonly CustomRoles plugin;

        /// <summary>
        /// Initializes a new instance of the <see cref="PlayerHandlers"/> class.
        /// </summary>
        /// <param name="plugin">The <see cref="CustomRoles"/> plugin instance.</param>
        public PlayerHandlers(CustomRoles plugin)
        {
            this.plugin = plugin;
        }

        /// <inheritdoc cref="Exiled.Events.Handlers.Player.SpawningRagdoll"/>
        internal void OnSpawningRagdoll(SpawningRagdollEventArgs ev)
        {
            if (plugin.StopRagdollPlayers.Contains(ev.Player))
            {
                ev.IsAllowed = false;
                plugin.StopRagdollPlayers.Remove(ev.Player);
            }
        }
    }
}