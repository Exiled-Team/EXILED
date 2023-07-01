// -----------------------------------------------------------------------
// <copyright file="Joined.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1600

    using System;

    using API.Features;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Loader.Features;

    using HarmonyLib;

    /// <summary>
    ///     Patches <see cref="ReferenceHub.Start" />.
    ///     Adds the <see cref="Handlers.Player.Joined" /> event.
    /// </summary>
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.Start))]
    internal static class Joined
    {
        internal static void CallEvent(ReferenceHub hub, out Player player)
        {
            try
            {
#if DEBUG
                Log.Debug("Creating new player object");
#endif
                player = new Player(hub);
#if DEBUG
                Log.Debug($"Object exists {player is not null}");
                Log.Debug($"Creating player object for {hub.nicknameSync.Network_displayName}");
#endif
                if (ReferenceHub.HostHub == null)
                {
                    Server.Host = player;
                }
                else
                {
                    Player.UnverifiedPlayers.Add(hub.gameObject, player);

                    Handlers.Player.OnJoined(new JoinedEventArgs(player));
                }
            }
            catch (Exception exception)
            {
                Log.Error($"{nameof(CallEvent)}: {exception}\n{exception.StackTrace}");
                player = null;
            }
        }

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter
        private static void Postfix(ReferenceHub __instance)
#pragma warning restore SA1313 // Parameter names should begin with lower-case letter
        {
            if (ReferenceHub.AllHubs.Count - 1 >= CustomNetworkManager.slots)
            {
                MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.SERVER_FULL);
            }

            CallEvent(__instance, out _);
        }
    }
}
