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
        private CoroutineHandle handle;
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

            Events.Handlers.Server.RoundStarted += roundHandler.OnRoundStarted;
            Events.Handlers.Server.SendingRemoteAdminCommand += serverHandler.OnRemoteAdminCommand;
            harmony = new Harmony($"com.{nameof(CustomItems)}.galaxy119-{DateTime.Now.Ticks}");
            harmony.PatchAll();

            handle = Timing.RunCoroutine(ShowCustomItemInNickname());
            base.OnEnabled();
        }

        /// <inheritdoc />
        public override void OnDisabled()
        {
            Timing.KillCoroutines(handle);
            Events.Handlers.Server.RoundStarted -= roundHandler.OnRoundStarted;
            Events.Handlers.Server.SendingRemoteAdminCommand -= serverHandler.OnRemoteAdminCommand;
            harmony.UnpatchAll();

            harmony = null;
            roundHandler = null;
            serverHandler = null;

            base.OnDisabled();
        }

        private IEnumerator<float> ShowCustomItemInNickname()
        {
            for (; ;)
            {
                foreach (Player player in Player.List)
                {
                    if (!CustomItem.TryGet(player.CurrentItem, out CustomItem customItem))
                        continue;

                    foreach (Player target in Player.List)
                    {
                        target.SendFakeSyncVar(
                            player.ReferenceHub.networkIdentity,
                            typeof(NicknameSync),
                            nameof(NicknameSync.Network_myNickSync),
                            target.Role == RoleType.Spectator ? $"{player.Nickname} (CustomItem: {customItem.Name})" : player.Nickname);
                    }
                }

                yield return Timing.WaitForSeconds(0.50f);
            }
        }
    }
}
