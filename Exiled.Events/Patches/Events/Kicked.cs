// -----------------------------------------------------------------------
// <copyright file="Kicked.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="ServerConsole.Disconnect(GameObject, string)"/>.
    /// Adds the <see cref="Player.Kicked"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.Disconnect), new[] { typeof(GameObject), typeof(string) })]
    public class Kicked
    {
        /// <summary>
        /// Prefix of <see cref="ServerConsole.Disconnect(GameObject, string)"/>.
        /// </summary>
        /// <param name="player">The player's game object to disconnect.</param>
        /// <param name="message"><inheritdoc cref="KickedEventArgs.Reason"/></param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(GameObject player, string message)
        {
            if (player == null)
                return false;

            var ev = new KickedEventArgs(API.Features.Player.Get(player), message);

            Player.OnKicked(ev);

            message = ev.Reason;

            return !ev.IsAllowed;
        }
    }
}
