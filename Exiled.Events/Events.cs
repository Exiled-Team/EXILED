// -----------------------------------------------------------------------
// <copyright file="Events.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events
{
    using System;
    using System.Diagnostics;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Interfaces;
    using Exiled.Events.Features;

    using UnityEngine.SceneManagement;

    /// <summary>
    /// Patch and unpatch events into the game.
    /// </summary>
    public sealed class Events : Plugin<Config>
    {
        private static Events instance;

        /// <summary>
        /// Gets the plugin instance.
        /// </summary>
        public static Events Instance
        {
            get => instance;
        }

        /// <inheritdoc/>
        public override PluginPriority Priority { get; } = PluginPriority.First;

        /// <summary>
        /// Gets the <see cref="Patcher"/> used to employ all patches.
        /// </summary>
        internal Patcher Patcher { get; private set; }

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            instance = this;
            base.OnEnabled();

            Patcher = new Patcher();

            Stopwatch watch = Stopwatch.StartNew();
            Patcher.PatchAll(!Config.UseDynamicPatching);
            watch.Stop();
            Log.Info($"{(Config.UseDynamicPatching ? "Non-event" : "All")} patches completed in {watch.Elapsed}");

            SceneManager.sceneUnloaded += Handlers.Internal.SceneUnloaded.OnSceneUnloaded;

            Handlers.Server.WaitingForPlayers += Handlers.Internal.Round.OnWaitingForPlayers;
            Handlers.Server.RestartingRound += Handlers.Internal.Round.OnRestartingRound;
            Handlers.Server.RoundStarted += Handlers.Internal.Round.OnRoundStarted;
            Handlers.Player.ChangingRole += Handlers.Internal.Round.OnChangingRole;
            Handlers.Player.Verified += Handlers.Internal.Round.OnVerified;
            Handlers.Player.Joined += Handlers.Internal.Round.OnJoined;
            Handlers.Player.Left += Handlers.Internal.Round.OnLeft;
            Handlers.Player.Destroying += Handlers.Internal.Round.OnDestroying;

            PlayerMovementSync.OnPlayerSpawned += Handlers.Player.OnSpawned;
            InventorySystem.InventoryExtensions.OnItemAdded += Handlers.Player.OnItemAdded;
            MapGeneration.SeedSynchronizer.OnMapGenerated += Handlers.Internal.MapGenerated.OnMapGenerated;

            ServerConsole.ReloadServerName();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();

            Patcher.DisabledPatchesHashSet.Clear();
            Patcher.UnpatchAll();
            Patcher = null;

            SceneManager.sceneUnloaded -= Handlers.Internal.SceneUnloaded.OnSceneUnloaded;

            Handlers.Server.WaitingForPlayers -= Handlers.Internal.Round.OnWaitingForPlayers;
            Handlers.Server.RestartingRound -= Handlers.Internal.Round.OnRestartingRound;
            Handlers.Server.RoundStarted -= Handlers.Internal.Round.OnRoundStarted;
            Handlers.Player.ChangingRole -= Handlers.Internal.Round.OnChangingRole;
            Handlers.Player.Verified -= Handlers.Internal.Round.OnVerified;
            Handlers.Player.Joined -= Handlers.Internal.Round.OnJoined;
            Handlers.Player.Left -= Handlers.Internal.Round.OnLeft;
            Handlers.Player.Destroying -= Handlers.Internal.Round.OnDestroying;

            PlayerMovementSync.OnPlayerSpawned -= Handlers.Player.OnSpawned;
            InventorySystem.InventoryExtensions.OnItemAdded -= Handlers.Player.OnItemAdded;
            MapGeneration.SeedSynchronizer.OnMapGenerated -= Handlers.Internal.MapGenerated.OnMapGenerated;
        }
    }
}