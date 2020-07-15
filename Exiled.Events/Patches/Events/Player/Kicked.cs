// -----------------------------------------------------------------------
// <copyright file="Kicked.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="ServerConsole.Disconnect(GameObject, string)"/>.
    /// Adds the <see cref="Player.Kicked"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.Disconnect), new[] { typeof(GameObject), typeof(string) })]
    internal class Kicked
    {
        private static bool Prefix(GameObject player, ref string message)
        {
            try
            {
                if (player == null)
                    return false;

                var ev = new KickedEventArgs(API.Features.Player.Get(player), message);

                Player.OnKicked(ev);

                message = ev.Reason;

                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.Kicked: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
