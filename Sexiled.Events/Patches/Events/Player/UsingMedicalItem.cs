// -----------------------------------------------------------------------
// <copyright file="UsingMedicalItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.API.Features;
using Sexiled.Events.EventArgs;

namespace Sexiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Sexiled.API.Features;
    using Sexiled.Events.EventArgs;

    using HarmonyLib;

    using MEC;

    /// <summary>
    /// Patches <see cref="ConsumableAndWearableItems.CallCmdUseMedicalItem"/>.
    /// Adds the <see cref="Handlers.Player.UsingMedicalItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ConsumableAndWearableItems), nameof(ConsumableAndWearableItems.CallCmdUseMedicalItem))]
    internal static class UsingMedicalItem
    {
        private static bool Prefix(ConsumableAndWearableItems __instance)
        {
            try
            {
                if (!__instance._medicalItemRateLimit.CanExecute(true))
                    return false;

                __instance._cancel = false;
                if (__instance.cooldown > 0f)
                    return false;

                for (int i = 0; i < __instance.usableItems.Length; ++i)
                {
                    if (__instance.usableItems[i].inventoryID == __instance._hub.inventory.curItem &&
                        __instance.usableCooldowns[i] <= 0.0)
                    {
                        var ev = new UsingMedicalItemEventArgs(API.Features.Player.Get(__instance.gameObject), __instance._hub.inventory.curItem, __instance.usableItems[i].animationDuration);

                        Handlers.Player.OnUsingMedicalItem(ev);

                        __instance.cooldown = ev.Cooldown;

                        if (ev.IsAllowed)
                            Timing.RunCoroutine(__instance.UseMedicalItem(i), Segment.FixedUpdate);
                    }
                }

                return false;
            }
            catch (Exception exception)
            {
                Log.Error($"Sexiled.Events.Patches.Events.Player.UsingMedicalItem: {exception}\n{exception.StackTrace}");

                return true;
            }
        }
    }
}
