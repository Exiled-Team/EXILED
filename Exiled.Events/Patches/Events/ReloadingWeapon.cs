// -----------------------------------------------------------------------
// <copyright file="ReloadingWeapon.cs" company="Exiled Team">
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

    /// <summary>
    /// Patches <see cref="WeaponManager.CallCmdReload(bool)"/>.
    /// Adds the <see cref="Player.ReloadingWeapon"/> event.
    /// </summary>
    [HarmonyPatch(typeof(WeaponManager), nameof(WeaponManager.CallCmdReload))]
    public class ReloadingWeapon
    {
        /// <summary>
        /// Prefix of <see cref="WeaponManager.CallCmdReload(bool)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="WeaponManager"/> instance.</param>
        /// <param name="animationOnly"><inheritdoc cref="ReloadingWeaponEventArgs.IsAnimationOnly"/></param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(WeaponManager __instance, bool animationOnly)
        {
            if (!__instance._iawRateLimit.CanExecute(false))
                return false;

            int itemIndex = __instance.hub.inventory.GetItemIndex();

            if (itemIndex < 0 || itemIndex >= __instance.hub.inventory.items.Count ||
                (__instance.curWeapon < 0 || __instance.hub.inventory.curItem !=
                 __instance.weapons[__instance.curWeapon].inventoryID) ||
                __instance.hub.inventory.items[itemIndex].durability >=
                (double)__instance.weapons[__instance.curWeapon].maxAmmo)
                return false;

            var ev = new ReloadingWeaponEventArgs(API.Features.Player.Get(__instance.gameObject), animationOnly);

            Player.OnReloadingWeapon(ev);

            return ev.IsAllowed;
        }
    }
}