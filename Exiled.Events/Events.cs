// -----------------------------------------------------------------------
// <copyright file="Events.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events
{
    using System;

    using Exiled.API.Enums;
    using Exiled.API.Features;
    using Exiled.Loader;

    using HarmonyLib;

    /// <summary>
    /// Patch and unpatch events into the game.
    /// </summary>
    public sealed class Events : Plugin<Config>
    {
        private static readonly Lazy<Events> LazyInstance = new Lazy<Events>(() => new Events());
        private readonly Handlers.Round round = new Handlers.Round();

        /// <summary>
        /// The below variable is used to increment the name of the harmony instance, otherwise harmony will not work upon a plugin reload.
        /// </summary>
        private int patchesCounter;

        private Events()
        {
        }

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
        public static Events Instance => LazyInstance.Value;

        /// <inheritdoc/>
        public override PluginPriority Priority { get; } = PluginPriority.Last;

        /// <summary>
        /// Gets the <see cref="HarmonyLib.Harmony"/> instance.
        /// </summary>
        public Harmony Harmony { get; private set; }

        /// <inheritdoc/>
        public override void OnEnabled()
        {
            base.OnEnabled();

            Patch();

            Handlers.Server.WaitingForPlayers += round.OnWaitingForPlayers;
            Handlers.Server.RoundStarted += round.OnRoundStarted;

            Handlers.Player.ChangingRole += round.OnChangingRole;

            ServerConsole.ReloadServerName();
        }

        /// <inheritdoc/>
        public override void OnDisabled()
        {
            base.OnDisabled();

            Unpatch();

            Handlers.Server.WaitingForPlayers -= round.OnWaitingForPlayers;
            Handlers.Server.RoundStarted -= round.OnRoundStarted;

            Handlers.Player.ChangingRole -= round.OnChangingRole;
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
                var lastDebugStatus = Harmony.DEBUG;
                Harmony.DEBUG = true;
#endif
                Harmony.PatchAll();
#if DEBUG
                Harmony.DEBUG = lastDebugStatus;
#endif
                Log.Debug("Events patched successfully!", Loader.ShouldDebugBeShown);
            }
            catch (Exception exception)
            {
                Log.Error($"Patching failed! {exception}");
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
    }
}
