// -----------------------------------------------------------------------
// <copyright file="Example.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Example
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;

    /// <summary>
    /// The example plugin.
    /// </summary>
    public class Example : Plugin<Config>
    {
        private static readonly Lazy<Example> LazyInstance = new Lazy<Example>(() => new Example());

        private Handlers.Server server;
        private Handlers.Player player;
        private Handlers.Warhead warhead;
        private Handlers.Map map;
        private Handlers.Item item;

        private Example()
        {
        }

        /// <summary>
        /// Gets the lazy instance.
        /// </summary>
        public static Example Instance => LazyInstance.Value;

        /// <inheritdoc/>
        public override PluginPriority Priority { get; } = PluginPriority.Last;

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
            map = new Handlers.Map();
            item = new Handlers.Item();

            Events.Handlers.Server.WaitingForPlayers += server.OnWaitingForPlayers;
            Events.Handlers.Server.EndingRound += server.OnEndingRound;

            Events.Handlers.Player.Died += player.OnDied;
            Events.Handlers.Player.ChangingRole += player.OnChangingRole;
            Events.Handlers.Player.ChangingItem += player.OnChangingItem;
            Events.Handlers.Player.Joined += player.OnJoined;
            Events.Handlers.Player.FailingEscapePocketDimension += player.OnFailingEscapePocketDimension;
            Events.Handlers.Player.EscapingPocketDimension += player.OnEscapingPocketDimension;
            Events.Handlers.Player.UnlockingGenerator += player.OnUnlockingGenerator;

            Events.Handlers.Warhead.Stopping += warhead.OnStopping;
            Events.Handlers.Warhead.Starting += warhead.OnStarting;

            Events.Handlers.Scp106.Teleporting += player.OnTeleporting;
            Events.Handlers.Scp106.Containing += player.OnContaining;
            Events.Handlers.Scp106.CreatingPortal += player.OnCreatingPortal;

            Events.Handlers.Scp914.Activating += player.OnActivating;
            Events.Handlers.Scp914.ChangingKnobSetting += player.OnChangingKnobSetting;

            Events.Handlers.Map.ExplodingGrenade += map.OnExplodingGrenade;

            Events.Handlers.Item.ChangingDurability += item.OnChangingDurability;
            Events.Handlers.Item.ChangingAttachments += item.OnChangingAttachments;
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
            Events.Handlers.Player.Joined -= player.OnJoined;
            Events.Handlers.Player.FailingEscapePocketDimension -= player.OnFailingEscapePocketDimension;
            Events.Handlers.Player.EscapingPocketDimension -= player.OnEscapingPocketDimension;
            Events.Handlers.Player.UnlockingGenerator -= player.OnUnlockingGenerator;

            Events.Handlers.Warhead.Stopping -= warhead.OnStopping;
            Events.Handlers.Warhead.Starting -= warhead.OnStarting;

            Events.Handlers.Scp106.Teleporting -= player.OnTeleporting;
            Events.Handlers.Scp106.Containing -= player.OnContaining;
            Events.Handlers.Scp106.CreatingPortal -= player.OnCreatingPortal;

            Events.Handlers.Scp914.Activating -= player.OnActivating;
            Events.Handlers.Scp914.ChangingKnobSetting -= player.OnChangingKnobSetting;

            Events.Handlers.Map.ExplodingGrenade -= map.OnExplodingGrenade;

            Events.Handlers.Item.ChangingDurability -= item.OnChangingDurability;
            Events.Handlers.Item.ChangingAttachments -= item.OnChangingAttachments;

            server = null;
            player = null;
            warhead = null;
            map = null;
            item = null;
        }
    }
}
