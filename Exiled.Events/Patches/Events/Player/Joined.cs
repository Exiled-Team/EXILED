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
    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.AddPlayer))]
    internal static class Joined
    {
        private static void Postfix(GameObject player)
        {
            try
            {
                if (!API.Features.Player.Dictionary.TryGetValue(player, out API.Features.Player newPlayer))
                {
                    newPlayer = new API.Features.Player(ReferenceHub.GetHub(player));

                    API.Features.Player.Dictionary.Add(player, newPlayer);
                }

                API.Features.Log.Debug($"Player {newPlayer?.Nickname} ({newPlayer?.UserId}) connected with the IP: {newPlayer?.IPAddress}");

                if (PlayerManager.players.Count >= CustomNetworkManager.slots)
                    API.Features.Log.Debug($"Server is full!");

                Timing.CallDelayed(0.25f, () =>
                {
                    if (newPlayer != null && newPlayer.IsMuted)
                        newPlayer.ReferenceHub.characterClassManager.SetDirtyBit(1UL);
                });

                var ev = new JoinedEventArgs(API.Features.Player.Get(player));

                Player.OnJoined(ev);
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.Joined: {e}\n{e.StackTrace}");
            }
        }
    }
}
