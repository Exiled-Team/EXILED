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
    using Exiled.Loader.Features;

    using HarmonyLib;

    using MEC;

    /// <summary>
    /// Patches <see cref="ServerRoles.CallCmdServerSignatureComplete(string, string, string, bool)"/>.
    /// Adds the <see cref="Player.Joined"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ServerRoles), nameof(ServerRoles.CallCmdServerSignatureComplete))]
    internal static class Joined
    {
        private static void Postfix(ServerRoles __instance)
        {
            try
            {
                // It means the client has failed the verification
                if (!__instance.PublicKeyAccepted)
                    return;

                // Allow only one call to this event
                if (API.Features.Player.Dictionary.ContainsKey(__instance.gameObject))
                    return;

                var player = new API.Features.Player(ReferenceHub.GetHub(__instance.gameObject));
                API.Features.Player.Dictionary.Add(__instance.gameObject, player);

                API.Features.Log.SendRaw($"Player {player?.Nickname} ({player?.UserId}) ({player?.Id}) connected with the IP: {player?.IPAddress}", ConsoleColor.Green);

                if (PlayerManager.players.Count >= CustomNetworkManager.slots)
                    MultiAdminFeatures.CallEvent(MultiAdminFeatures.EventType.SERVER_FULL);

                Timing.CallDelayed(0.25f, () =>
                {
                    if (player?.IsMuted == true)
                        player.ReferenceHub.characterClassManager.SetDirtyBit(2UL);
                });

                var ev = new JoinedEventArgs(API.Features.Player.Get(__instance.gameObject));

                Player.OnJoined(ev);
            }
            catch (Exception exception)
            {
                API.Features.Log.Error($"{typeof(Joined).FullName}:\n{exception}");
            }
        }
    }
}
