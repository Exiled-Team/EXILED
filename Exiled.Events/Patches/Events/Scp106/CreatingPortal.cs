// -----------------------------------------------------------------------
// <copyright file="CreatingPortal.cs" company="Exiled Team">
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
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="Scp106PlayerScript.CallCmdMakePortal"/>.
    /// Adds the <see cref="Scp106.CreatingPortal"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.CallCmdMakePortal))]
    public class CreatingPortal
    {
        /// <summary>
        /// Prefix of <see cref="Scp106PlayerScript.CallCmdMakePortal"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="Scp106PlayerScript"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(Scp106PlayerScript __instance)
        {
            if (!__instance._interactRateLimit.CanExecute(true) || !__instance.GetComponent<FallDamage>().isGrounded)
                return false;

            bool rayCastHit = Physics.Raycast(new Ray(__instance.transform.position, -__instance.transform.up), out RaycastHit raycastHit, 10f, __instance.teleportPlacementMask);

            var ev = new CreatingPortalEventArgs(API.Features.Player.Get(__instance.gameObject), raycastHit.point - Vector3.up);

            Scp106.OnCreatingPortal(ev);

            Debug.DrawRay(__instance.transform.position, -__instance.transform.up, Color.red, 10f);

            if (ev.IsAllowed && __instance.iAm106 && !__instance.goingViaThePortal && rayCastHit)
                __instance.SetPortalPosition(ev.Position);

            return false;
        }
    }
}
