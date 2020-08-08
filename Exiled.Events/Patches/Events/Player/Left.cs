// -----------------------------------------------------------------------
// <copyright file="Left.cs" company="Exiled Team">
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

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="ReferenceHub.OnDestroy"/>.
    /// Adds the <see cref="Handlers.Player.Left"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ReferenceHub), nameof(ReferenceHub.OnDestroy))]
    internal static class Left
    {
        private static void Prefix(ReferenceHub __instance)
        {
            try
            {
                Player player = Player.Get(__instance.gameObject);

                if (player == null || player.IsHost)
                    return;

                var ev = new LeftEventArgs(player);

                Log.SendRaw($"Player {ev.Player.Nickname} ({ev.Player.UserId}) ({player?.Id}) disconnected", ConsoleColor.Green);

                Handlers.Player.OnLeft(ev);

                Player.IdsCache.Remove(player.Id);
                Player.UserIdsCache.Remove(player.UserId);
                Player.Dictionary.Remove(player.GameObject);
            }
            catch (Exception exception)
            {
                Log.Error($"Exiled.Events.Patches.Events.Player.Left: {exception}\n{exception.StackTrace}");
            }
        }
    }
}
