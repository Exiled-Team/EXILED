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
    public class Example : Plugin<Config>
    {
        private Handlers.Server server;
        private Handlers.Player player;
        private Handlers.Warhead warhead;

        /// <inheritdoc/>
        public override PluginPriority Priority { get; } = PluginPriority.Medium;

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            base.OnEnabled();

            RegisterEvents();

            Log.Warn($"I correctly read the string config, its value is: {Config.String}");
            Log.Warn($"I correctly read the int config, its value is: {Config.Int}");
            Log.Warn($"I correctly read the float config, its value is: {Config.Float}");
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();

            UnregisterEvents();
        }

        /// <summary>
        /// Registers the plugin events.
        /// </summary>
        private void RegisterEvents()
        {
            server = new Handlers.Server();
            player = new Handlers.Player();
            warhead = new Handlers.Warhead();

            Events.Handlers.Server.WaitingForPlayers += server.OnWaitingForPlayers;
            Events.Handlers.Server.EndingRound += server.OnEndingRound;

            Events.Handlers.Player.Died += player.OnDied;
            Events.Handlers.Player.ChangingRole += player.OnChangingRole;
            Events.Handlers.Player.ChangingItem += player.OnChangingItem;

            Events.Handlers.Warhead.Stopping += warhead.OnStopping;
            Events.Handlers.Warhead.Starting += warhead.OnStarting;

            Events.Handlers.Scp106.Teleporting += player.OnTeleporting;
            Events.Handlers.Scp106.Containing += player.OnContaining;
            Events.Handlers.Scp106.CreatingPortal += player.OnCreatingPortal;
        }

        /// <summary>
        /// Unregisters the plugin events.
        /// </summary>
        private void UnregisterEvents()
        {
            Events.Handlers.Server.WaitingForPlayers -= server.OnWaitingForPlayers;
            Events.Handlers.Server.EndingRound -= server.OnEndingRound;

            Events.Handlers.Player.Died -= player.OnDied;
            Events.Handlers.Player.ChangingRole -= player.OnChangingRole;
            Events.Handlers.Player.ChangingItem -= player.OnChangingItem;

            Events.Handlers.Warhead.Stopping -= warhead.OnStopping;
            Events.Handlers.Warhead.Starting -= warhead.OnStarting;

            Events.Handlers.Scp106.Teleporting -= player.OnTeleporting;
            Events.Handlers.Scp106.Containing -= player.OnContaining;
            Events.Handlers.Scp106.CreatingPortal -= player.OnCreatingPortal;

            server = null;
            player = null;
            warhead = null;
        }
    }
}
