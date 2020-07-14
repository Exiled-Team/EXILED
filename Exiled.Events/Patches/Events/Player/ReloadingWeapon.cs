// -----------------------------------------------------------------------
// <copyright file="ReloadingWeapon.cs" company="Exiled Team">
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
    /// Patches <see cref="WeaponManager.CallCmdReload(bool)"/>.
    /// Adds the <see cref="Player.ReloadingWeapon"/> event.
    /// </summary>
    [HarmonyPatch(typeof(WeaponManager), nameof(WeaponManager.CallCmdReload))]
    internal class ReloadingWeapon
    {
        private static bool Prefix(WeaponManager __instance, bool animationOnly)
        {
            try
            {
                if (!__instance._iawRateLimit.CanExecute(false))
                    return false;

                int itemIndex = __instance._hub.inventory.GetItemIndex();

                if (itemIndex < 0 || itemIndex >= __instance._hub.inventory.items.Count ||
                    (__instance.curWeapon < 0 || __instance._hub.inventory.curItem !=
                        __instance.weapons[__instance.curWeapon].inventoryID) ||
                    __instance._hub.inventory.items[itemIndex].durability >=
                    (double)__instance.weapons[__instance.curWeapon].maxAmmo)
                    return false;

                var ev = new ReloadingWeaponEventArgs(API.Features.Player.Get(__instance.gameObject), animationOnly);

                Player.OnReloadingWeapon(ev);

                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.ReloadingWeapon: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
