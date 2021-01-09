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

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;
    using Exiled.Loader.Features;

    using HarmonyLib;

    using MEC;

    using PlayerAPI = Exiled.API.Features.Player;
    using PlayerEvents = Exiled.Events.Handlers.Player;

    /// <summary>
    /// Patches <see cref="ReferenceHub.Awake"/>.
    /// Adds the <see cref="PlayerEvents.Joined"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.Awake))]
    internal static class Joined
    {
        private static void Postfix(ReferenceHub __instance)
        {
            try
            {
                // ReferenceHub is a component that is loaded first
                if (__instance.isDedicatedServer || ReferenceHub.HostHub == null || PlayerManager.localPlayer == null)
                    return;

                var player = new PlayerAPI(__instance);
                PlayerAPI.Dictionary.Add(__instance.gameObject, player);

                Log.SendRaw($"Player {player.Nickname} ({player.UserId}) ({player.Id}) connected with the IP: {player.IPAddress}", ConsoleColor.Green);

                if (PlayerManager.players.Count >= CustomNetworkManager.slots)
                    MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.SERVER_FULL);

                Timing.CallDelayed(0.25f, () =>
                {
                    if (player.IsMuted)
                        player.ReferenceHub.characterClassManager.SetDirtyBit(2UL);
                });

                PlayerEvents.OnJoined(new JoinedEventArgs(player));
            }
            catch (Exception exception)
            {
                Log.Error($"{typeof(Joined).FullName}:\n{exception}");
            }
        }
    }
}
