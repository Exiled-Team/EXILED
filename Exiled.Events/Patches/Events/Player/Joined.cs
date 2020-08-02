// -----------------------------------------------------------------------
// <copyright file="Joined.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using MEC;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerManager.AddPlayer(GameObject)"/>.
    /// Adds the <see cref="Player.Joined"/> event.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), "set_" + nameof(CharacterClassManager.NetworkIsVerified))]
    internal static class Joined
    {
        private static void Prefix(CharacterClassManager __instance)
        {
            try
            {
                if (string.IsNullOrEmpty(__instance?.UserId))
                    return;

                if (!API.Features.Player.Dictionary.TryGetValue(__instance.gameObject, out API.Features.Player player))
                {
                    player = new API.Features.Player(ReferenceHub.GetHub(__instance.gameObject));

                    API.Features.Player.Dictionary.Add(__instance.gameObject, player);
                }

                API.Features.Log.Debug($"Player {player?.Nickname} ({player?.UserId}) connected with the IP: {player?.IPAddress}");

                if (PlayerManager.players.Count >= CustomNetworkManager.slots)
                    API.Features.Log.Debug($"Server is full!");

                Timing.CallDelayed(0.25f, () =>
                {
                    if (player != null && player.IsMuted)
                        player.ReferenceHub.characterClassManager.SetDirtyBit(1UL);
                });

                var ev = new JoinedEventArgs(API.Features.Player.Get(__instance.gameObject));

                Player.OnJoined(ev);
            }
            catch (Exception exception)
            {
                API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.Joined: {exception}\n{exception.StackTrace}");
            }
        }
    }
}
