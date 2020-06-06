// -----------------------------------------------------------------------
// <copyright file="StoppingMedicalItem.cs" company="Exiled Team">
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
    /// Patches <see cref="ConsumableAndWearableItems.CallCmdCancelMedicalItem"/>.
    /// Adds the <see cref="Player.StoppingMedicalItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ConsumableAndWearableItems), nameof(ConsumableAndWearableItems.CallCmdCancelMedicalItem))]
    public class StoppingMedicalItem
    {
        /// <summary>
        /// Prefix of <see cref="ConsumableAndWearableItems.CallCmdCancelMedicalItem"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="ConsumableAndWearableItems"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(ConsumableAndWearableItems __instance)
        {
            if (!__instance._interactRateLimit.CanExecute(true))
                return false;

            for (int i = 0; i < __instance.usableItems.Length; ++i)
            {
                if (__instance.usableItems[i].inventoryID == __instance.hub.inventory.curItem && __instance.usableItems[i].cancelableTime > 0f)
                {
                    var ev = new StoppingMedicalItemEventArgs(API.Features.Player.Get(__instance.gameObject), __instance.hub.inventory.curItem, __instance.usableItems[i].animationDuration);

                    Player.OnStoppingMedicalItem(ev);

                    __instance.cancel = ev.IsAllowed;
                }
            }

            return false;
        }
    }
}
