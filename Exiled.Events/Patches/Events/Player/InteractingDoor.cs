// -----------------------------------------------------------------------
// <copyright file="InteractingDoor.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
#pragma warning disable SA1005
#pragma warning disable SA1515
#pragma warning disable SA1513
#pragma warning disable SA1512
    using System;

    using API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;

    using PlayerRoles;

    using PluginAPI.Events;

    /// <summary>
    ///     Patches <see cref="DoorVariant.ServerInteract(ReferenceHub, byte)" />.
    ///     Adds the <see cref="Handlers.Player.InteractingDoor" /> event.
    /// </summary>
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.ServerInteract), typeof(ReferenceHub), typeof(byte))]
    internal static class InteractingDoor
    {
        private static bool Prefix(DoorVariant __instance, ReferenceHub ply, byte colliderId)
        {
            try
            {
                InteractingDoorEventArgs ev = new(Player.Get(ply), __instance, true, true);

                bool bypassDenied = false;

                if (__instance.ActiveLocks > 0 && !ply.serverRoles.BypassMode)
                {
                    DoorLockMode mode = DoorLockUtils.GetMode((DoorLockReason)__instance.ActiveLocks);
                    if ((!mode.HasFlagFast(DoorLockMode.CanClose) || !mode.HasFlagFast(DoorLockMode.CanOpen)) &&
                        (!mode.HasFlagFast(DoorLockMode.ScpOverride) || !ply.IsSCP(true)) &&
                        (mode == DoorLockMode.FullLock || (__instance.TargetState && !mode.HasFlagFast(DoorLockMode.CanClose)) ||
                        (!__instance.TargetState && !mode.HasFlagFast(DoorLockMode.CanOpen))))
                    {
                        bypassDenied = true;
                    }
                }

                if (!bypassDenied && (ev.IsAllowed = __instance.AllowInteracting(ply, colliderId)))
                {
                    if (ply.GetRoleId() == RoleTypeId.Scp079 || __instance.RequiredPermissions.CheckPermissions(ply.inventory.CurInstance, ply))
                    {
                        ev.AccessGranted = true;
                    }
                    else
                    {
                        ev.AccessGranted = false;
                    }
                }

                Handlers.Player.OnInteractingDoor(ev);

                if (EventManager.ExecuteEvent(PluginAPI.Enums.ServerEventType.PlayerInteractDoor, new object[] { ply, __instance, ev.IsAllowed && ev.AccessGranted }))
                {
                    if (!ev.IsAllowed)
                    {
                        return false;
                    }

                    if (ev.AccessGranted)
                    {
                        __instance.NetworkTargetState = !__instance.TargetState;
                        __instance._triggerPlayer = ply;
                    }
                    else if (bypassDenied)
                    {
                        __instance.LockBypassDenied(ply, colliderId);
                    }
                    else
                    {
                        __instance.PermissionsDenied(ply, colliderId);
                        DoorEvents.TriggerAction(__instance, DoorAction.AccessDenied, ply);
                    }
                }
                return false;
            }
            catch (Exception exception)
            {
                Log.Error($"{typeof(InteractingDoor).FullName}.{nameof(Prefix)}:\n{exception}");
                return true;
            }
        }
    }
}