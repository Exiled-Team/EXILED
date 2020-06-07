// -----------------------------------------------------------------------
// <copyright file="InteractingLocker.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="PlayerInteract.CallCmdUseLocker(int, int)"/>.
    /// Adds the <see cref="Player.InteractingLocker"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUseLocker))]
    public class InteractingLocker
    {
        /// <summary>
        /// Prefix of <see cref="PlayerInteract.CallCmdUseLocker(int, int)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="PlayerInteract"/> instance.</param>
        /// <param name="lockerId"><inheritdoc cref="InteractingLockerEventArgs.Id"/></param>
        /// <param name="chamberNumber">The chamber number.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(PlayerInteract __instance, int lockerId, int chamberNumber)
        {
            if (!__instance._playerInteractRateLimit.CanExecute(true) || (__instance._hc.CufferId > 0 && !__instance.CanDisarmedInteract))
                return false;

            LockerManager singleton = LockerManager.singleton;

            if (lockerId < 0 || lockerId >= singleton.lockers.Length)
                return false;

            if (!__instance.ChckDis(singleton.lockers[lockerId].gameObject.position) || !singleton.lockers[lockerId].supportsStandarizedAnimation)
                return false;

            if (chamberNumber < 0 || chamberNumber >= singleton.lockers[lockerId].chambers.Length)
                return false;

            if (singleton.lockers[lockerId].chambers[chamberNumber].doorAnimator == null)
                return false;

            if (!singleton.lockers[lockerId].chambers[chamberNumber].CooldownAtZero())
                return false;

            singleton.lockers[lockerId].chambers[chamberNumber].SetCooldown();

            string accessToken = singleton.lockers[lockerId].chambers[chamberNumber].accessToken;
            Item itemByID = __instance._inv.GetItemByID(__instance._inv.curItem);

            var ev = new InteractingLockerEventArgs(
                API.Features.Player.Get(__instance.gameObject),
                singleton.lockers[lockerId],
                lockerId,
                string.IsNullOrEmpty(accessToken) || (itemByID != null && itemByID.permissions.Contains(accessToken)) || __instance._sr.BypassMode);

            Player.OnInteractingLocker(ev);

            if (ev.IsAllowed)
            {
                bool flag = (singleton.openLockers[lockerId] & 1 << chamberNumber) != 1 << chamberNumber;
                singleton.ModifyOpen(lockerId, chamberNumber, flag);
                singleton.RpcDoSound(lockerId, chamberNumber, flag);
                bool state = true;
                for (int i = 0; i < singleton.lockers[lockerId].chambers.Length; i++)
                {
                    if ((singleton.openLockers[lockerId] & 1 << i) == 1 << i)
                    {
                        state = false;
                        break;
                    }
                }

                singleton.lockers[lockerId].LockPickups(state);
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
    }
}
