// -----------------------------------------------------------------------
// <copyright file="CustomItems.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems
{
    using System;

    using Exiled.API.Features;

    using HarmonyLib;

    /// <summary>
    /// Handles all CustomItem API.
    /// </summary>
    public class CustomItems : Plugin<Config>
    {
        private static readonly CustomItems Singleton = new CustomItems();

        private RoundHandler roundHandler;
        private Harmony harmony;

        private CustomItems()
        {
        }

        /// <summary>
        /// Gets the static reference to this <see cref="CustomItems"/> class.
        /// </summary>
        public static CustomItems Instance => Singleton;

        /// <inheritdoc />
        public override void OnEnabled()
        {
            roundHandler = new RoundHandler();

            Events.Handlers.Server.RoundStarted += roundHandler.OnRoundStarted;
            harmony = new Harmony($"com.{nameof(CustomItems)}.galaxy119-{DateTime.Now.Ticks}");
            harmony.PatchAll();

            base.OnEnabled();
        }

        /// <inheritdoc />
        public override void OnDisabled()
        {
            Events.Handlers.Server.RoundStarted -= roundHandler.OnRoundStarted;
            harmony.UnpatchAll();

            harmony = null;
            roundHandler = null;

            base.OnDisabled();
        }
    }
}
