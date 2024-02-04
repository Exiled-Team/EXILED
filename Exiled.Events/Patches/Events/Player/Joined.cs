// -----------------------------------------------------------------------
// <copyright file="Joined.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1600
#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    using System;

    using API.Features;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Loader.Features;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="ReferenceHub.Start" />.
    /// Adds the <see cref="Handlers.Player.Joined" /> event.
    /// </summary>
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.Start))]
    public static class Joined
    {
        internal static void CallEvent(ReferenceHub hub, out Player player)
        {
            try
            {
#if DEBUG
                Log.Debug("Creating new player object");
#endif
                player = Activator.CreateInstance(Player.DEFAULT_PLAYER_CLASS, args: hub) as Player;
#if DEBUG
                Log.Debug($"Object exists {player is not null}");
                Log.Debug($"Creating player object for {hub.nicknameSync.Network_displayName}");
#endif
                Player.UnverifiedPlayers.Add(hub.gameObject, player);

                if (ReferenceHub.HostHub == null)
                {
                    Server.Host = player;
                }
                else
                {
                    Handlers.Player.OnJoined(new JoinedEventArgs(player));
                }
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(CallEvent)}: {exception}\n{exception.StackTrace}");
                player = null;
            }
        }

        private static void Postfix(ReferenceHub __instance)
        {
            if (ReferenceHub.AllHubs.Count - 1 >= CustomNetworkManager.slots)
            {
                MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.SERVER_FULL);
            }

            CallEvent(__instance, out _);
        }
    }
}
