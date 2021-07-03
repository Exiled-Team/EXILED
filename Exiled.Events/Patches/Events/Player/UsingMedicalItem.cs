// -----------------------------------------------------------------------
// <copyright file="UsingMedicalItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;
    using System.Diagnostics;

    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

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
                        var ev = new UsingMedicalItemEventArgs(Player.Get(__instance.gameObject), __instance._hub.inventory.curItem, __instance.usableItems[i].animationDuration);

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
                Log.Error($"Exiled.Events.Patches.Events.Player.UsingMedicalItem:\n{exception.ToStringDemystified()}");

                return true;
            }
        }
    }
}
