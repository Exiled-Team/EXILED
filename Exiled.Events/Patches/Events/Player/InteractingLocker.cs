// -----------------------------------------------------------------------
// <copyright file="InteractingLocker.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="PlayerInteract.CallCmdUseLocker(byte, byte)"/>.
    /// Adds the <see cref="Player.InteractingLocker"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUseLocker))]
    internal static class InteractingLocker
    {
        private static bool Prefix(PlayerInteract __instance, byte lockerId, byte chamberNumber)
        {
            try
            {
                if (!__instance._playerInteractRateLimit.CanExecute(true) ||
                    (__instance._hc.CufferId > 0 && !PlayerInteract.CanDisarmedInteract))
                    return false;

                LockerManager singleton = LockerManager.singleton;

                if (lockerId >= singleton.lockers.Length)
                    return false;

                if (!__instance.ChckDis(singleton.lockers[lockerId].gameObject.position) ||
                    !singleton.lockers[lockerId].supportsStandarizedAnimation)
                    return false;

                if (chamberNumber >= singleton.lockers[lockerId].chambers.Length)
                    return false;

                if (singleton.lockers[lockerId].chambers[chamberNumber].doorAnimator == null)
                    return false;

                if (!singleton.lockers[lockerId].chambers[chamberNumber].CooldownAtZero())
                    return false;

                singleton.lockers[lockerId].chambers[chamberNumber].SetCooldown();

                string accessToken = singleton.lockers[lockerId].chambers[chamberNumber].accessToken;
                var itemById = __instance._inv.GetItemByID(__instance._inv.curItem);

                var ev = new InteractingLockerEventArgs(
                    API.Features.Player.Get(__instance.gameObject),
                    singleton.lockers[lockerId],
                    singleton.lockers[lockerId].chambers[chamberNumber],
                    lockerId,
                    chamberNumber,
                    string.IsNullOrEmpty(accessToken) || (itemById != null && itemById.permissions.Contains(accessToken)) || __instance._sr.BypassMode);

                Player.OnInteractingLocker(ev);

                if (ev.IsAllowed)
                {
                    bool flag = (singleton.openLockers[lockerId] & 1 << chamberNumber) != 1 << chamberNumber;
                    singleton.ModifyOpen(lockerId, chamberNumber, flag);
                    singleton.RpcDoSound(lockerId, chamberNumber, flag);
                    bool anyOpen = true;
                    for (int i = 0; i < singleton.lockers[lockerId].chambers.Length; i++)
                    {
                        if ((singleton.openLockers[lockerId] & 1 << i) == 1 << i)
                        {
                            anyOpen = false;
                            break;
                        }
                    }

                    singleton.lockers[lockerId].LockPickups(!flag, chamberNumber, anyOpen);
                    if (!string.IsNullOrEmpty(accessToken))
                    {
                        singleton.RpcChangeMaterial(lockerId, chamberNumber, false);
                    }
                }
                else
                {
                    singleton.RpcChangeMaterial(lockerId, chamberNumber, true);
                }

                __instance.OnInteract();

                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.InteractingLocker:\n{e}");

                return true;
            }
        }
    }
}
