// -----------------------------------------------------------------------
// <copyright file="InteractingDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;
    using System.Linq;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerInteract.CallCmdOpenDoor(GameObject)"/>.
    /// Adds the <see cref="Handlers.Player.InteractingDoor"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdOpenDoor), typeof(GameObject))]
    internal static class InteractingDoor
    {
        private static bool Prefix(PlayerInteract __instance, GameObject doorId)
        {
            try
            {
                if (!__instance._playerInteractRateLimit.CanExecute() ||
                    (__instance._hc.CufferId > 0 && !PlayerInteract.CanDisarmedInteract) ||
                    (doorId == null || __instance._ccm.CurClass == RoleType.None ||
                     (__instance._ccm.CurClass == RoleType.Spectator || !doorId.TryGetComponent(out Door door))) ||
                    (door.Buttons.Count == 0
                        ? (!__instance.ChckDis(doorId.transform.position) ? 1 : 0)
                        : (door.Buttons.All(item => !__instance.ChckDis(item.button.transform.position)) ? 1 : 0)) != 0)
                    return false;

                var ev = new InteractingDoorEventArgs(API.Features.Player.Get(__instance.gameObject), door);

                __instance.OnInteract();

                if (__instance._sr.BypassMode)
                {
                    ev.IsAllowed = true;
                }
                else if (ev.Door.PermissionLevels.HasPermission(Door.AccessRequirements.Checkpoints) &&
                         __instance._ccm.CurRole.team == Team.SCP)
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
                                Door.backwardsCompatPermissions.TryGetValue(p, out var flag) &&
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

                Handlers.Player.OnInteractingDoor(ev);

                if (ev.IsAllowed && (!ev.Door.locked || __instance._sr.BypassMode))
                    ev.Door.ChangeState(__instance._sr.BypassMode);
                else
                    __instance.RpcDenied(doorId);

                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Exiled.Events.Patches.Events.Player.InteractingDoor: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
