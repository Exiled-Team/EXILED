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
    using Exiled.Example.Events;

    /// <summary>
    /// The example plugin.
    /// </summary>
    public class Example : Plugin<Config>
    {
        private static readonly Example Singleton = new Example();

        private ServerHandler serverHandler;
        private PlayerHandler playerHandler;
        private WarheadHandler warheadHandler;
        private MapHandler mapHandler;
        private ItemHandler itemHandler;
        private Scp914Handler scp914Handler;
        private Scp096Handler scp096Handler;

        private Example()
        {
        }

        /// <summary>
        /// Gets the only existing instance of this plugin.
        /// </summary>
        public static Example Instance => Singleton;

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
            serverHandler = new ServerHandler();
            playerHandler = new PlayerHandler();
            warheadHandler = new WarheadHandler();
            mapHandler = new MapHandler();
            itemHandler = new ItemHandler();
            scp914Handler = new Scp914Handler();
            scp096Handler = new Scp096Handler();

            Exiled.Events.Handlers.Server.WaitingForPlayers += serverHandler.OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.EndingRound += serverHandler.OnEndingRound;

            Exiled.Events.Handlers.Player.Destroying += playerHandler.OnDestroying;
            Exiled.Events.Handlers.Player.Dying += playerHandler.OnDying;
            Exiled.Events.Handlers.Player.Died += playerHandler.OnDied;
            Exiled.Events.Handlers.Player.ChangingRole += playerHandler.OnChangingRole;
            Exiled.Events.Handlers.Player.ChangingItem += playerHandler.OnChangingItem;
            Exiled.Events.Handlers.Player.Verified += playerHandler.OnVerified;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension += playerHandler.OnFailingEscapePocketDimension;
            Exiled.Events.Handlers.Player.EscapingPocketDimension += playerHandler.OnEscapingPocketDimension;
            Exiled.Events.Handlers.Player.UnlockingGenerator += playerHandler.OnUnlockingGenerator;

            Exiled.Events.Handlers.Warhead.Stopping += warheadHandler.OnStopping;
            Exiled.Events.Handlers.Warhead.Starting += warheadHandler.OnStarting;

            Exiled.Events.Handlers.Scp106.Teleporting += playerHandler.OnTeleporting;
            Exiled.Events.Handlers.Scp106.Containing += playerHandler.OnContaining;
            Exiled.Events.Handlers.Scp106.CreatingPortal += playerHandler.OnCreatingPortal;

            Exiled.Events.Handlers.Scp914.Activating += playerHandler.OnActivating;
            Exiled.Events.Handlers.Scp914.ChangingKnobSetting += playerHandler.OnChangingKnobSetting;

            Exiled.Events.Handlers.Map.ExplodingGrenade += mapHandler.OnExplodingGrenade;
            Exiled.Events.Handlers.Map.GeneratorActivated += mapHandler.OnGeneratorActivated;

            Exiled.Events.Handlers.Item.ChangingDurability += itemHandler.OnChangingDurability;
            Exiled.Events.Handlers.Item.ChangingAttachments += itemHandler.OnChangingAttachments;

            Exiled.Events.Handlers.Scp914.UpgradingItems += scp914Handler.OnUpgradingItems;

            Exiled.Events.Handlers.Scp096.AddingTarget += scp096Handler.OnAddingTarget;
        }

        /// <summary>
        /// Unregisters the plugin events.
        /// </summary>
        private void UnregisterEvents()
        {
            Exiled.Events.Handlers.Server.WaitingForPlayers -= serverHandler.OnWaitingForPlayers;
            Exiled.Events.Handlers.Server.EndingRound -= serverHandler.OnEndingRound;

            Exiled.Events.Handlers.Player.Destroying -= playerHandler.OnDestroying;
            Exiled.Events.Handlers.Player.Dying -= playerHandler.OnDying;
            Exiled.Events.Handlers.Player.Died -= playerHandler.OnDied;
            Exiled.Events.Handlers.Player.ChangingRole -= playerHandler.OnChangingRole;
            Exiled.Events.Handlers.Player.ChangingItem -= playerHandler.OnChangingItem;
            Exiled.Events.Handlers.Player.Verified -= playerHandler.OnVerified;
            Exiled.Events.Handlers.Player.FailingEscapePocketDimension -= playerHandler.OnFailingEscapePocketDimension;
            Exiled.Events.Handlers.Player.EscapingPocketDimension -= playerHandler.OnEscapingPocketDimension;
            Exiled.Events.Handlers.Player.UnlockingGenerator -= playerHandler.OnUnlockingGenerator;

            Exiled.Events.Handlers.Warhead.Stopping -= warheadHandler.OnStopping;
            Exiled.Events.Handlers.Warhead.Starting -= warheadHandler.OnStarting;

            Exiled.Events.Handlers.Scp106.Teleporting -= playerHandler.OnTeleporting;
            Exiled.Events.Handlers.Scp106.Containing -= playerHandler.OnContaining;
            Exiled.Events.Handlers.Scp106.CreatingPortal -= playerHandler.OnCreatingPortal;

            Exiled.Events.Handlers.Scp914.Activating -= playerHandler.OnActivating;
            Exiled.Events.Handlers.Scp914.ChangingKnobSetting -= playerHandler.OnChangingKnobSetting;

            Exiled.Events.Handlers.Map.ExplodingGrenade -= mapHandler.OnExplodingGrenade;
            Exiled.Events.Handlers.Map.GeneratorActivated -= mapHandler.OnGeneratorActivated;

            Exiled.Events.Handlers.Item.ChangingDurability -= itemHandler.OnChangingDurability;
            Exiled.Events.Handlers.Item.ChangingAttachments -= itemHandler.OnChangingAttachments;

            Exiled.Events.Handlers.Scp914.UpgradingItems -= scp914Handler.OnUpgradingItems;

            Exiled.Events.Handlers.Scp096.AddingTarget -= scp096Handler.OnAddingTarget;

            serverHandler = null;
            playerHandler = null;
            warheadHandler = null;
            mapHandler = null;
            itemHandler = null;
            scp914Handler = null;
            scp096Handler = null;
        }
    }
}
