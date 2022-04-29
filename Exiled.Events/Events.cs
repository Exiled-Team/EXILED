// -----------------------------------------------------------------------
// <copyright file="Events.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Loader;

    using HarmonyLib;

    using UnityEngine.SceneManagement;

    /// <summary>
    /// Patch and unpatch events into the game.
    /// </summary>
    public sealed class Events : Plugin<Config>
    {
        private static Events instance;

        /// <summary>
        /// The below variable is used to increment the name of the harmony instance, otherwise harmony will not work upon a plugin reload.
        /// </summary>
        private int patchesCounter;

        /// <summary>
        /// The custom <see cref="EventHandler"/> delegate.
        /// </summary>
        /// <typeparam name="TEventArgs">The <see cref="EventHandler{TEventArgs}"/> type.</typeparam>
        /// <param name="ev">The <see cref="EventHandler{TEventArgs}"/> instance.</param>
        public delegate void CustomEventHandler<TEventArgs>(TEventArgs ev)
            where TEventArgs : System.EventArgs;

        /// <summary>
        /// The custom <see cref="EventHandler"/> delegate, with empty parameters.
        /// </summary>
        public delegate void CustomEventHandler();

        /// <summary>
        /// Gets the plugin instance.
        /// </summary>
        public static Events Instance => instance;

        /// <summary>
        /// Gets a set of types and methods for which EXILED patches should not be run.
        /// </summary>
        public static HashSet<MethodBase> DisabledPatchesHashSet { get; } = new();

        /// <inheritdoc/>
        public override PluginPriority Priority { get; } = PluginPriority.First;

        /// <summary>
        /// Gets the <see cref="HarmonyLib.Harmony"/> instance.
        /// </summary>
        public Harmony Harmony { get; private set; }

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            instance = this;
            base.OnEnabled();

            Stopwatch watch = Stopwatch.StartNew();
            Patch();

            watch.Stop();
            Log.Info($"Patching completed in {watch.Elapsed}");
            SceneManager.sceneUnloaded += Handlers.Internal.SceneUnloaded.OnSceneUnloaded;
            MapGeneration.SeedSynchronizer.OnMapGenerated += Handlers.Internal.MapGenerated.OnMapGenerated;

            Handlers.Server.WaitingForPlayers += Handlers.Internal.Round.OnWaitingForPlayers;
            Handlers.Server.RestartingRound += Handlers.Internal.Round.OnRestartingRound;
            Handlers.Server.RoundStarted += Handlers.Internal.Round.OnRoundStarted;
            Handlers.Player.ChangingRole += Handlers.Internal.Round.OnChangingRole;
            PlayerMovementSync.OnPlayerSpawned += Handlers.Player.OnSpawned;

            ServerConsole.ReloadServerName();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();

            Unpatch();

            DisabledPatchesHashSet.Clear();

            SceneManager.sceneUnloaded -= Handlers.Internal.SceneUnloaded.OnSceneUnloaded;

            Handlers.Server.WaitingForPlayers -= Handlers.Internal.Round.OnWaitingForPlayers;
            Handlers.Server.RestartingRound -= Handlers.Internal.Round.OnRestartingRound;
            Handlers.Server.RoundStarted -= Handlers.Internal.Round.OnRoundStarted;
            Handlers.Player.ChangingRole -= Handlers.Internal.Round.OnChangingRole;
            PlayerMovementSync.OnPlayerSpawned -= Handlers.Player.OnSpawned;
            Handlers.Map.Generated -= Handlers.Internal.MapGenerated.OnMapGenerated;

            MapGeneration.SeedSynchronizer.OnMapGenerated -= Handlers.Map.OnGenerated;
        }

        /// <summary>
        /// Patches all events.
        /// </summary>
        public void Patch()
        {
            try
            {
                Harmony = new Harmony($"exiled.events.{++patchesCounter}");
#if DEBUG
                bool lastDebugStatus = Harmony.DEBUG;
                Harmony.DEBUG = true;
#endif
                if (PatchByAttributes())
                {
                    Log.Debug("Events patched successfully!", Loader.ShouldDebugBeShown);
                }
                else
                {
                    Log.Error($"Patching failed!");
                }
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
        /// Checks the <see cref="DisabledPatchesHashSet"/> list and un-patches any methods that have been defined there. Once un-patching has been done, they can be patched by plugins, but will not be re-patchable by Exiled until a server reboot.
        /// </summary>
        public void ReloadDisabledPatches()
        {
            foreach (MethodBase method in DisabledPatchesHashSet)
            {
                Harmony.Unpatch(method, HarmonyPatchType.All, Harmony.Id);

                Log.Info($"Unpatched {method.Name}");
            }
        }

        /// <summary>
        /// Unpatches all events.
        /// </summary>
        public void Unpatch()
        {
            Log.Debug("Unpatching events...", Loader.ShouldDebugBeShown);
            Harmony.UnpatchAll();

            Log.Debug("All events have been unpatched complete. Goodbye!", Loader.ShouldDebugBeShown);
        }

        private bool PatchByAttributes()
        {
            try
            {
                Harmony.PatchAll();

                Log.Debug("Events patched by attributes successfully!", Loader.ShouldDebugBeShown);
                return true;
            }
            catch (Exception exception)
            {
                Log.Error($"Patching by attributes failed!\n{exception}");
                return false;
            }
        }
    }
}
