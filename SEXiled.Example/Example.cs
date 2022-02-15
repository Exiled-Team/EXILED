// -----------------------------------------------------------------------
// <copyright file="Example.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Example
{
    using SEXiled.API.Enums;
    using SEXiled.API.Features;
    using SEXiled.Example.Events;

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
            RegisterEvents();

            Log.Warn($"I correctly read the string config, its value is: {Config.String}");
            Log.Warn($"I correctly read the int config, its value is: {Config.Int}");
            Log.Warn($"I correctly read the float config, its value is: {Config.Float}");

            base.OnEnabled();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            UnregisterEvents();
            base.OnDisabled();
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

            SEXiled.Events.Handlers.Server.WaitingForPlayers += serverHandler.OnWaitingForPlayers;
            SEXiled.Events.Handlers.Server.RoundStarted += serverHandler.OnRoundStarted;

            SEXiled.Events.Handlers.Player.Destroying += playerHandler.OnDestroying;
            SEXiled.Events.Handlers.Player.Spawning += playerHandler.OnSpawning;
            SEXiled.Events.Handlers.Player.Escaping += playerHandler.OnEscaping;
            SEXiled.Events.Handlers.Player.Hurting += playerHandler.OnHurting;
            SEXiled.Events.Handlers.Player.Dying += playerHandler.OnDying;
            SEXiled.Events.Handlers.Player.Died += playerHandler.OnDied;
            SEXiled.Events.Handlers.Player.ChangingRole += playerHandler.OnChangingRole;
            SEXiled.Events.Handlers.Player.ChangingItem += playerHandler.OnChangingItem;
            SEXiled.Events.Handlers.Player.UsingItem += playerHandler.OnUsingItem;
            SEXiled.Events.Handlers.Player.PickingUpItem += playerHandler.OnPickingUpItem;
            SEXiled.Events.Handlers.Player.DroppingItem += playerHandler.OnDroppingItem;
            SEXiled.Events.Handlers.Player.Verified += playerHandler.OnVerified;
            SEXiled.Events.Handlers.Player.FailingEscapePocketDimension += playerHandler.OnFailingEscapePocketDimension;
            SEXiled.Events.Handlers.Player.EscapingPocketDimension += playerHandler.OnEscapingPocketDimension;
            SEXiled.Events.Handlers.Player.UnlockingGenerator += playerHandler.OnUnlockingGenerator;
            SEXiled.Events.Handlers.Player.PreAuthenticating += playerHandler.OnPreAuthenticating;
            SEXiled.Events.Handlers.Player.Shooting += playerHandler.OnShooting;
            SEXiled.Events.Handlers.Player.ReloadingWeapon += playerHandler.OnReloading;
            SEXiled.Events.Handlers.Player.ReceivingEffect += playerHandler.OnReceivingEffect;

            SEXiled.Events.Handlers.Warhead.Stopping += warheadHandler.OnStopping;
            SEXiled.Events.Handlers.Warhead.Starting += warheadHandler.OnStarting;

            SEXiled.Events.Handlers.Scp106.Teleporting += playerHandler.OnTeleporting;
            SEXiled.Events.Handlers.Scp106.Containing += playerHandler.OnContaining;
            SEXiled.Events.Handlers.Scp106.CreatingPortal += playerHandler.OnCreatingPortal;

            SEXiled.Events.Handlers.Scp914.Activating += playerHandler.OnActivating;
            SEXiled.Events.Handlers.Scp914.ChangingKnobSetting += playerHandler.OnChangingKnobSetting;
            SEXiled.Events.Handlers.Scp914.UpgradingPlayer += playerHandler.OnUpgradingPlayer;

            SEXiled.Events.Handlers.Map.ExplodingGrenade += mapHandler.OnExplodingGrenade;
            SEXiled.Events.Handlers.Map.GeneratorActivated += mapHandler.OnGeneratorActivated;

            SEXiled.Events.Handlers.Item.ChangingDurability += itemHandler.OnChangingDurability;
            SEXiled.Events.Handlers.Item.ChangingAttachments += itemHandler.OnChangingAttachments;
            SEXiled.Events.Handlers.Item.ReceivingPreference += itemHandler.OnReceivingPreference;

            SEXiled.Events.Handlers.Scp914.UpgradingItem += scp914Handler.OnUpgradingItem;

            SEXiled.Events.Handlers.Scp096.AddingTarget += scp096Handler.OnAddingTarget;
        }

        /// <summary>
        /// Unregisters the plugin events.
        /// </summary>
        private void UnregisterEvents()
        {
            SEXiled.Events.Handlers.Server.WaitingForPlayers -= serverHandler.OnWaitingForPlayers;
            SEXiled.Events.Handlers.Server.RoundStarted -= serverHandler.OnRoundStarted;

            SEXiled.Events.Handlers.Player.Destroying -= playerHandler.OnDestroying;
            SEXiled.Events.Handlers.Player.Dying -= playerHandler.OnDying;
            SEXiled.Events.Handlers.Player.Died -= playerHandler.OnDied;
            SEXiled.Events.Handlers.Player.ChangingRole -= playerHandler.OnChangingRole;
            SEXiled.Events.Handlers.Player.ChangingItem -= playerHandler.OnChangingItem;
            SEXiled.Events.Handlers.Player.PickingUpItem += playerHandler.OnPickingUpItem;
            SEXiled.Events.Handlers.Player.Verified -= playerHandler.OnVerified;
            SEXiled.Events.Handlers.Player.FailingEscapePocketDimension -= playerHandler.OnFailingEscapePocketDimension;
            SEXiled.Events.Handlers.Player.EscapingPocketDimension -= playerHandler.OnEscapingPocketDimension;
            SEXiled.Events.Handlers.Player.UnlockingGenerator -= playerHandler.OnUnlockingGenerator;
            SEXiled.Events.Handlers.Player.PreAuthenticating -= playerHandler.OnPreAuthenticating;

            SEXiled.Events.Handlers.Warhead.Stopping -= warheadHandler.OnStopping;
            SEXiled.Events.Handlers.Warhead.Starting -= warheadHandler.OnStarting;

            SEXiled.Events.Handlers.Scp106.Teleporting -= playerHandler.OnTeleporting;
            SEXiled.Events.Handlers.Scp106.Containing -= playerHandler.OnContaining;
            SEXiled.Events.Handlers.Scp106.CreatingPortal -= playerHandler.OnCreatingPortal;

            SEXiled.Events.Handlers.Scp914.Activating -= playerHandler.OnActivating;
            SEXiled.Events.Handlers.Scp914.ChangingKnobSetting -= playerHandler.OnChangingKnobSetting;

            SEXiled.Events.Handlers.Map.ExplodingGrenade -= mapHandler.OnExplodingGrenade;
            SEXiled.Events.Handlers.Map.GeneratorActivated -= mapHandler.OnGeneratorActivated;

            SEXiled.Events.Handlers.Item.ChangingDurability -= itemHandler.OnChangingDurability;
            SEXiled.Events.Handlers.Item.ChangingAttachments -= itemHandler.OnChangingAttachments;
            SEXiled.Events.Handlers.Item.ReceivingPreference -= itemHandler.OnReceivingPreference;

            SEXiled.Events.Handlers.Scp914.UpgradingItem -= scp914Handler.OnUpgradingItem;

            SEXiled.Events.Handlers.Scp096.AddingTarget -= scp096Handler.OnAddingTarget;

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
