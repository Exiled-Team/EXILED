// -----------------------------------------------------------------------
// <copyright file="CustomItems.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.CustomItems
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.CustomItems.API.Features;

    using HarmonyLib;

    using MEC;

    /// <summary>
    /// Handles all CustomItem API.
    /// </summary>
    public class CustomItems : Plugin<Config>
    {
        private static readonly CustomItems Singleton = new CustomItems();

        private RoundHandler roundHandler;
        private ServerHandler serverHandler;
        private PlayerHandler playerHandler;
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
            serverHandler = new ServerHandler();
            playerHandler = new PlayerHandler();

            Events.Handlers.Server.RoundStarted += roundHandler.OnRoundStarted;
            Events.Handlers.Server.SendingRemoteAdminCommand += serverHandler.OnRemoteAdminCommand;

            Events.Handlers.Player.ChangingRole += playerHandler.OnChangingRole;

            harmony = new Harmony($"com.{nameof(CustomItems)}.galaxy119-{DateTime.Now.Ticks}");
            harmony.PatchAll();

            base.OnEnabled();
        }

        /// <inheritdoc />
        public override void OnDisabled()
        {
            Events.Handlers.Server.RoundStarted -= roundHandler.OnRoundStarted;
            Events.Handlers.Server.SendingRemoteAdminCommand -= serverHandler.OnRemoteAdminCommand;

            Events.Handlers.Player.ChangingRole -= playerHandler.OnChangingRole;

            harmony.UnpatchAll();

            harmony = null;
            roundHandler = null;
            serverHandler = null;

            base.OnDisabled();
        }
    }
}
