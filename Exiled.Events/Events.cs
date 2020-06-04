// -----------------------------------------------------------------------
// <copyright file="Events.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events
{
    using System;
    using System.Reflection;
    using Exiled.API.Features;
    using Exiled.API.Interfaces;
    using Exiled.Events.Handlers;
    using Exiled.Loader;
    using HarmonyLib;

    /// <summary>
    /// Patch and unpatch events into the game.
    /// </summary>
    public class Events : Plugin
    {
        /// <summary>
        /// The below variable is used to increment the name of the harmony instance, otherwise harmony will not work upon a plugin reload.
        /// </summary>
        private int patchesCounter;

        private Command command;
        private Handlers.Round round;

        /// <summary>
        /// The custom <see cref="EventHandler"/> delegate.
        /// </summary>
        /// <typeparam name="TEventArgs">The <see cref="EventHandler{TEventArgs}"/> type.</typeparam>
        /// <param name="ev">The <see cref="EventHandler{TEventArgs}"/> instance.</param>
        public delegate void CustomEventHandler<TEventArgs>(TEventArgs ev);

        /// <summary>
        /// The custom <see cref="EventHandler"/> delegate, with empty parameters.
        /// </summary>
        public delegate void CustomEventHandler();

        /// <summary>
        /// Gets the <see cref="HarmonyLib.Harmony"/> instance.
        /// </summary>
        public Harmony Harmony { get; private set; }

        /// <inheritdoc/>
        public override string Name => "EXILED.Events";

        /// <inheritdoc/>
        public override Version RequiredExiledVersion => Assembly.GetExecutingAssembly().GetName().Version;

        /// <inheritdoc/>
        public override IConfig Config => new Config();

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            Log.Debug("Loading configs...", PluginManager.ShouldDebugBeShown);

            Config.Reload();

            Log.Debug("Adding event handlers...", PluginManager.ShouldDebugBeShown);

            command = new Command();
            round = new Handlers.Round();

            Handlers.Server.WaitingForPlayers += round.OnWaitingForPlayers;
            Handlers.Server.RoundStarted += round.OnRoundStarted;
            Handlers.Server.SendingRemoteAdminCommand += command.OnSendingRemoteAdminCommand;

            Handlers.Player.Left += round.OnPlayerLeft;
            Handlers.Player.Joined += round.OnPlayerJoined;
            Handlers.Player.ChangingRole += round.OnChangingRole;

            Patch();

            if (!Exiled.Events.Config.IsNameTrackingEnabled)
                API.Features.Server.Name = $"{API.Features.Server.Name.Replace("<size=1>SM119.0.0</size>", string.Empty)} <color=#00000000><size=1>SM119.{RequiredExiledVersion.Major}.{RequiredExiledVersion.Minor}.{RequiredExiledVersion.Build} (EXILED)</size></color>";
            Log.Info($"EXILED version - {RequiredExiledVersion}");
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            Log.Info("Disabled.");
            Log.Debug("Removing Event Handlers...", PluginManager.ShouldDebugBeShown);

            Handlers.Server.WaitingForPlayers -= round.OnWaitingForPlayers;
            Handlers.Server.RoundStarted -= round.OnRoundStarted;
            Handlers.Server.SendingRemoteAdminCommand -= command.OnSendingRemoteAdminCommand;

            Handlers.Player.Left -= round.OnPlayerLeft;
            Handlers.Player.Joined -= round.OnPlayerJoined;
            Handlers.Player.ChangingRole -= round.OnChangingRole;

            command = null;
            round = null;

            Unpatch();
        }

        /// <summary>
        /// The below is called when the Exiled loader reloads all plugins. The reloading process calls OnDisable, then OnReload, unloads the plugin and reloads the new version, then OnEnable.
        /// </summary>
        public override void OnReloaded() => Log.Info("Reloading events.");

        /// <summary>
        /// Patches all events.
        /// </summary>
        public void Patch()
        {
            try
            {
                Harmony = new Harmony($"exiled.patches.{++patchesCounter}");
                Harmony.PatchAll();

                #if DEBUG
				bool disabledStatus = Harmony.DEBUG;

				Harmony.DEBUG = true;
				Harmony.DEBUG = disabledStatus;
                #endif
            }
            catch (Exception exception)
            {
                Log.Error($"Patching failed! {exception}");
            }

            Log.Debug("Events patched successfully!", PluginManager.ShouldDebugBeShown);
        }

        /// <summary>
        /// Unpatches all events.
        /// </summary>
        public void Unpatch()
        {
            Log.Debug("Unpatching events...", PluginManager.ShouldDebugBeShown);

            Harmony.UnpatchAll();

            Log.Debug("All events have been unpatched complete. Goodbye!", PluginManager.ShouldDebugBeShown);
        }
    }
}