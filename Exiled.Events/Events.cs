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
    using System.Linq;
    using System.Reflection;

    using API.Enums;
    using API.Features;
    using CentralAuth;
    using Exiled.API.Interfaces;
    using Exiled.Events.Features;
    using HarmonyLib;
    using InventorySystem.Items.Pickups;
    using InventorySystem.Items.Usables;
    using PlayerRoles.Ragdolls;
    using PlayerRoles.RoleAssign;
    using PluginAPI.Events;
    using Respawning;
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
        public static Events Instance => instance;

        /// <inheritdoc/>
        public override PluginPriority Priority { get; } = PluginPriority.First;

        /// <summary>
        /// Gets the <see cref="Features.Patcher"/> used to employ all patches.
        /// </summary>
        public Patcher Patcher { get; private set; }

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            instance = this;
            base.OnEnabled();

            Stopwatch watch = Stopwatch.StartNew();

            Patch();

            watch.Stop();

            Log.Info($"{(Config.UseDynamicPatching ? "Non-event" : "All")} patches completed in {watch.Elapsed}");
            PlayerAuthenticationManager.OnInstanceModeChanged -= RoleAssigner.CheckLateJoin;

            SceneManager.sceneUnloaded += Handlers.Internal.SceneUnloaded.OnSceneUnloaded;
            MapGeneration.SeedSynchronizer.OnMapGenerated += Handlers.Internal.MapGenerated.OnMapGenerated;
            UsableItemsController.ServerOnUsingCompleted += Handlers.Internal.Round.OnServerOnUsingCompleted;
            Handlers.Server.WaitingForPlayers += Handlers.Internal.Round.OnWaitingForPlayers;
            Handlers.Server.RestartingRound += Handlers.Internal.Round.OnRestartingRound;
            Handlers.Server.RoundStarted += Handlers.Internal.Round.OnRoundStarted;
            Handlers.Player.ChangingRole += Handlers.Internal.Round.OnChangingRole;
            Handlers.Scp049.ActivatingSense += Handlers.Internal.Round.OnActivatingSense;
            Handlers.Player.Verified += Handlers.Internal.Round.OnVerified;
            Handlers.Map.ChangedIntoGrenade += Handlers.Internal.ExplodingGrenade.OnChangedIntoGrenade;

            CharacterClassManager.OnRoundStarted += Handlers.Server.OnRoundStarted;

            InventorySystem.InventoryExtensions.OnItemAdded += Handlers.Player.OnItemAdded;
            InventorySystem.InventoryExtensions.OnItemRemoved += Handlers.Player.OnItemRemoved;

            RespawnManager.ServerOnRespawned += Handlers.Server.OnRespawnedTeam;

            RagdollManager.OnRagdollSpawned += Handlers.Internal.RagdollList.OnSpawnedRagdoll;
            RagdollManager.OnRagdollRemoved += Handlers.Internal.RagdollList.OnRemovedRagdoll;
            ItemPickupBase.OnPickupAdded += Handlers.Internal.PickupEvent.OnSpawnedPickup;
            ItemPickupBase.OnPickupDestroyed += Handlers.Internal.PickupEvent.OnRemovedPickup;

            ServerConsole.ReloadServerName();

            EventManager.RegisterEvents<Handlers.Player>(this);

            foreach (Type type in typeof(IEnumClass).Assembly.GetTypes().Where(x => x.GetInterface(nameof(IEnumClass)) == typeof(IEnumClass)))
            {
                FieldInfo[] fieldInfos = type.GetFields();

                if (fieldInfos.All(x => !x.IsInitOnly))
                    continue;

                foreach (FieldInfo field in fieldInfos.Where(x => x.IsStatic))
                {
                    field.GetValue(null);
                }
            }
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();

            Unpatch();

            SceneManager.sceneUnloaded -= Handlers.Internal.SceneUnloaded.OnSceneUnloaded;
            MapGeneration.SeedSynchronizer.OnMapGenerated -= Handlers.Internal.MapGenerated.OnMapGenerated;
            UsableItemsController.ServerOnUsingCompleted -= Handlers.Internal.Round.OnServerOnUsingCompleted;

            Handlers.Server.WaitingForPlayers -= Handlers.Internal.Round.OnWaitingForPlayers;
            Handlers.Server.RestartingRound -= Handlers.Internal.Round.OnRestartingRound;
            Handlers.Server.RoundStarted -= Handlers.Internal.Round.OnRoundStarted;
            Handlers.Player.ChangingRole -= Handlers.Internal.Round.OnChangingRole;
            Handlers.Scp049.ActivatingSense -= Handlers.Internal.Round.OnActivatingSense;
            Handlers.Player.Verified -= Handlers.Internal.Round.OnVerified;
            Handlers.Map.ChangedIntoGrenade -= Handlers.Internal.ExplodingGrenade.OnChangedIntoGrenade;

            CharacterClassManager.OnRoundStarted -= Handlers.Server.OnRoundStarted;

            InventorySystem.InventoryExtensions.OnItemAdded -= Handlers.Player.OnItemAdded;
            InventorySystem.InventoryExtensions.OnItemRemoved -= Handlers.Player.OnItemRemoved;

            RespawnManager.ServerOnRespawned -= Handlers.Server.OnRespawnedTeam;

            RagdollManager.OnRagdollSpawned -= Handlers.Internal.RagdollList.OnSpawnedRagdoll;
            RagdollManager.OnRagdollRemoved -= Handlers.Internal.RagdollList.OnRemovedRagdoll;
            ItemPickupBase.OnPickupAdded -= Handlers.Internal.PickupEvent.OnSpawnedPickup;
            ItemPickupBase.OnPickupDestroyed -= Handlers.Internal.PickupEvent.OnRemovedPickup;

            EventManager.UnregisterEvents<Handlers.Player>(this);
        }

        /// <summary>
        /// Patches all events.
        /// </summary>
        public void Patch()
        {
            try
            {
                Patcher = new Patcher();
#if DEBUG
                bool lastDebugStatus = Harmony.DEBUG;
                Harmony.DEBUG = true;
#endif
                Patcher.PatchAll(!Config.UseDynamicPatching, out int failedPatch);

                if (failedPatch == 0)
                    Log.Debug("Events patched successfully!");
                else
                    Log.Error($"Patching failed! There are {failedPatch} broken patches.");
#if DEBUG
                Harmony.DEBUG = lastDebugStatus;
#endif
            }
            catch (Exception exception)
            {
                Log.Error($"Patching failed!\n{exception}");
            }
        }

        /// <summary>
        /// Unpatches all events.
        /// </summary>
        public void Unpatch()
        {
            Log.Debug("Unpatching events...");
            Patcher.UnpatchAll();
            Patcher = null;
            Log.Debug("All events have been unpatched complete. Goodbye!");
        }
    }
}