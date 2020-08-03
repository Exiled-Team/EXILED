// -----------------------------------------------------------------------
// <copyright file="CreatingPortal.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp106
{
#pragma warning disable SA1118
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
    internal static class CreatingPortal
    {
        private static bool Prefix(Scp106PlayerScript __instance)
        {
            if (!__instance._interactRateLimit.CanExecute(true) || !__instance.hub.falldamage.isGrounded)
                return false;
            Transform transform = __instance.transform;
            Debug.DrawRay(transform.position, -transform.up, Color.red, 10f);
            RaycastHit hitInfo;
            if (!__instance.iAm106 || __instance.goingViaThePortal || !Physics.Raycast(new Ray(__instance.transform.position, -__instance.transform.up), out hitInfo, 10f, __instance.teleportPlacementMask))
                return false;

            var creatingPortalEventArgs = new CreatingPortalEventArgs(API.Features.Player.Get(__instance.gameObject), hitInfo.point);
            Exiled.Events.Handlers.Scp106.OnCreatingPortal(creatingPortalEventArgs);

            if (creatingPortalEventArgs.IsAllowed)
                __instance.SetPortalPosition(hitInfo.point - Vector3.up);

            return false;
        }
    }
}
