// -----------------------------------------------------------------------
// <copyright file="InteractingDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using System.Linq;
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerInteract.CallCmdOpenDoor(GameObject)"/>.
    /// Adds the <see cref="Map.InteractingDoor"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdOpenDoor), typeof(GameObject))]
    public class InteractingDoor
    {
        /// <summary>
        /// Prefix of <see cref="PlayerInteract.CallCmdOpenDoor(GameObject)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="PlayerInteract"/> instance.</param>
        /// <param name="doorId">The door id.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(PlayerInteract __instance, GameObject doorId)
        {
            Door door = null;

            var ev = new InteractingDoorEventArgs(API.Features.Player.Get(__instance.gameObject), door);

            if (!__instance._playerInteractRateLimit.CanExecute() ||
                (__instance._hc.CufferId > 0 && !__instance.CanDisarmedInteract) ||
                doorId == null ||
                !doorId.TryGetComponent(out door) ||
                (__instance._ccm.CurClass == RoleType.None || __instance._ccm.CurClass == RoleType.Spectator) ||
                (door.buttons.Count == 0 ?
                    (__instance.ChckDis(doorId.transform.position) ? 1 : 0) :
                    (door.buttons.Any(item => __instance.ChckDis(item.transform.position)) ? 1 : 0)) == 0)
                return false;

            ev.Door = door;

            __instance.OnInteract();

            if (__instance._sr.BypassMode)
            {
                ev.IsAllowed = true;
            }
            else if (ev.Door.PermissionLevels.HasPermission(Door.AccessRequirements.Checkpoints) && __instance._ccm.CurRole.team == Team.SCP)
            {
                ev.IsAllowed = true;
            }
            else
            {
                try
                {
                    if (ev.Door.PermissionLevels == 0)
                    {
                        ev.IsAllowed = !ev.Door.locked;
                    }
                    else if (!ev.Door.RequireAllPermissions)
                    {
                        var itemPerms = __instance._inv.GetItemByID(__instance._inv.curItem).permissions;

                        ev.IsAllowed = itemPerms.Any(p =>
                        ev.Door.backwardsCompatPermissions.TryGetValue(p, out var flag) &&
                        ev.Door.PermissionLevels.HasPermission(flag)) || false;
                    }
                    else
                    {
                        ev.IsAllowed = false;
                    }
                }
                catch
                {
                    ev.IsAllowed = false;
                }
            }

            Map.OnInteractingDoor(ev);

            if (ev.IsAllowed)
                ev.Door.ChangeState(__instance._sr.BypassMode);
            else
                __instance.RpcDenied(doorId);

            return false;
        }
    }
}