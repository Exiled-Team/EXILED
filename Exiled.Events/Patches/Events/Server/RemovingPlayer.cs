// -----------------------------------------------------------------------
// <copyright file="RemovingPlayer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Server
{
    using Exiled.Events.EventArgs;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerManager.RemovePlayer"/>
    /// Adds the <see cref="Handlers.Server.RemovingPlayer"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerManager), nameof(PlayerManager.RemovePlayer))]
    internal static class RemovingPlayer
    {
        private static bool Prefix(GameObject gameObject, int maxPlayers)
        {
            RemovingPlayerEventArgs ev = new RemovingPlayerEventArgs(gameObject, maxPlayers);
            Handlers.Server.OnRemovingPlayer(ev);

            return ev.IsAllowed;
        }
    }
}
