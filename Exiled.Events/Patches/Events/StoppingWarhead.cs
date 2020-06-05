// -----------------------------------------------------------------------
// <copyright file="StoppingWarhead.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using System;
    using Exiled.Events;
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using Mirror;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="AlphaWarheadController.CancelDetonation(GameObject)"/>.
    /// Adds the <see cref="Map.StoppingWarhead"/> event.
    /// </summary>
    [HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.CancelDetonation), new Type[] { typeof(GameObject) })]
    public class StoppingWarhead
    {
        /// <summary>
        /// Prefix of <see cref="AlphaWarheadController.CancelDetonation(GameObject)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="AlphaWarheadController"/> instance.</param>
        /// <param name="disabler">The player who's disabling the warhead.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(AlphaWarheadController __instance, GameObject disabler)
        {
            ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Detonation cancelled.", ServerLogs.ServerLogType.GameEvent);

            if (!__instance.inProgress || __instance.timeToDetonation <= 10.0)
                return false;

            if (__instance.timeToDetonation <= 15.0 && disabler != null)
                __instance.GetComponent<PlayerStats>().TargetAchieve(disabler.GetComponent<NetworkIdentity>().connectionToClient, "thatwasclose");

            var ev = new StoppingWarheadEventArgs(API.Features.Player.Get(disabler) ?? API.Features.Server.Host);

            Map.OnStoppingWarhead(ev);

            return ev.IsAllowed && !Exiled.API.Features.Warhead.IsWarheadLocked;
        }
    }
}