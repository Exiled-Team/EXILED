// -----------------------------------------------------------------------
// <copyright file="EnragingScp096.cs" company="Exiled Team">
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
    using PlayableScps;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp096.Enrage"/>.
    /// Adds the <see cref="Player.EnragingScp096"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.Enrage))]
    public class EnragingScp096
    {
        /// <summary>
        /// Prefix of <see cref="Scp096.Enrage"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="Scp096"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(Scp096 __instance)
        {
            var ev = new EnragingScp096EventArgs(__instance, API.Features.Player.Get(__instance.Hub.gameObject));

            Player.OnEnragingScp096(ev);

            if (!ev.IsAllowed)
                return false;

            if (__instance.Enraged)
            {
                __instance.AddReset();
            }
            else
            {
                __instance.SetMovementSpeed(12f);
                __instance.SetJumpHeight(10f);
                __instance.PlayerState = Scp096PlayerState.Enraged;
                __instance.EnrageTimeLeft = __instance.EnrageTime;
            }

            return false;
        }
    }
}