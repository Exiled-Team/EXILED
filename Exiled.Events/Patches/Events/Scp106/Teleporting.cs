// -----------------------------------------------------------------------
// <copyright file="Teleporting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp106
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using MEC;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp106PlayerScript.CallCmdUsePortal"/>.
    /// Adds the <see cref="Scp106.Teleporting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.CallCmdUsePortal))]
    public class Teleporting
    {
        /// <summary>
        /// Prefix of <see cref="Scp106PlayerScript.CallCmdUsePortal"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="Scp106PlayerScript"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(Scp106PlayerScript __instance)
        {
            if (!__instance._interactRateLimit.CanExecute(false) || !__instance.GetComponent<FallDamage>().isGrounded)
                return false;

            var ev = new TeleportingEventArgs(API.Features.Player.Get(__instance.gameObject), __instance.portalPosition);

            Scp106.OnTeleporting(ev);

            __instance.portalPosition = ev.PortalPosition;

            if (!ev.IsAllowed)
                return false;

            if (__instance.iAm106 && __instance.portalPosition != Vector3.zero && !__instance.goingViaThePortal)
                Timing.RunCoroutine(__instance._DoTeleportAnimation(), Segment.Update);

            return true;
        }
    }
}
