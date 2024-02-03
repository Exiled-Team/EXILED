// -----------------------------------------------------------------------
// <copyright file="Verified.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;

    using API.Features;
    using CentralAuth;
    using Exiled.API.Extensions;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    /// <summary>
    /// Patches <see cref="PlayerAuthenticationManager.FinalizeAuthentication" />.
    /// Adds the <see cref="Handlers.Player.Verified" /> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerAuthenticationManager), nameof(PlayerAuthenticationManager.FinalizeAuthentication))]
    internal static class Verified
    {
        private static void Postfix(PlayerAuthenticationManager __instance)
        {
            if (!Player.UnverifiedPlayers.TryGetValue(__instance._hub.gameObject, out Player player))
                Joined.CallEvent(__instance._hub, out player);

            Player.Dictionary.Add(__instance._hub.gameObject, player);

            player.IsVerified = true;
            player.RawUserId = player.UserId.GetRawUserId();

            Log.SendRaw($"Player {player.Nickname} ({player.UserId}) ({player.Id}) connected with the IP: {player.IPAddress}", ConsoleColor.Green);

            Handlers.Player.OnVerified(new VerifiedEventArgs(player));
        }
    }
}