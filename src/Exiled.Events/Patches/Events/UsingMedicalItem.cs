// -----------------------------------------------------------------------
// <copyright file="UsingMedicalItem.cs" company="Exiled Team">
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
    using MEC;

    /// <summary>
    /// Patches <see cref="ConsumableAndWearableItems.CallCmdUseMedicalItem"/>.
    /// Adds the <see cref="Player.UsingMedicalItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ConsumableAndWearableItems), nameof(ConsumableAndWearableItems.CallCmdUseMedicalItem))]
    public class UsingMedicalItem
    {
        /// <summary>
        /// Prefix of <see cref="ConsumableAndWearableItems.CallCmdUseMedicalItem"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="ConsumableAndWearableItems"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(ConsumableAndWearableItems __instance)
        {
            if (!__instance._interactRateLimit.CanExecute())
                return false;

            __instance.cancel = false;

            if (__instance.cooldown > 0.0)
                return false;

            for (int i = 0; i < __instance.usableItems.Length; ++i)
            {
                if (__instance.usableItems[i].inventoryID == __instance.hub.inventory.curItem && __instance.usableCooldowns[i] <= 0.0)
                {
                    var ev = new UsingMedicalItemEventArgs(API.Features.Player.Get(__instance.gameObject), __instance.hub.inventory.curItem, __instance.usableItems[i].animationDuration);

                    __instance.usableItems[i].animationDuration = ev.Cooldown;

                    if (ev.IsAllowed)
                        Timing.RunCoroutine(__instance.UseMedicalItem(i), Segment.FixedUpdate);
                }
            }

            return false;
        }
    }
}