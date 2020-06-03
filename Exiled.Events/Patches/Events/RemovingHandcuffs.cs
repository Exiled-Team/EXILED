// -----------------------------------------------------------------------
// <copyright file="RemovingHandcuffs.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Handcuffs.ClearTarget"/>.
    /// Adds the <see cref="Player.RemovingHandcuffs"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Handcuffs), nameof(Handcuffs.ClearTarget))]
    public class RemovingHandcuffs
    {
        /// <summary>
        /// Prefix of <see cref="Handcuffs.ClearTarget"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="Handcuffs"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(Handcuffs __instance)
        {
            foreach (GameObject player in PlayerManager.players)
            {
                var targetPlayer = API.Features.Player.Get(player);

                if (targetPlayer.CufferId == __instance.MyReferenceHub.queryProcessor.PlayerId)
                {
                    var ev = new RemovingHandcuffsEventArgs(API.Features.Player.Get(__instance.gameObject), targetPlayer);

                    Player.OnRemovingHandcuffs(ev);

                    if (ev.IsAllowed)
                        targetPlayer.CufferId = -1;

                    break;
                }
            }

            return false;
        }
    }
}