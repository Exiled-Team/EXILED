// -----------------------------------------------------------------------
// <copyright file="StoppingMedicalItem.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;
    using System.Diagnostics;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="ConsumableAndWearableItems.CallCmdCancelMedicalItem"/>.
    /// Adds the <see cref="Player.StoppingMedicalItem"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ConsumableAndWearableItems), nameof(ConsumableAndWearableItems.CallCmdCancelMedicalItem))]
    internal static class StoppingMedicalItem
    {
        private static bool Prefix(ConsumableAndWearableItems __instance)
        {
            try
            {
                if (!__instance._interactRateLimit.CanExecute(true))
                    return false;

                for (int i = 0; i < __instance.usableItems.Length; ++i)
                {
                    if (__instance.usableItems[i].inventoryID == __instance._hub.inventory.curItem && __instance.usableItems[i].cancelableTime > 0f)
                    {
                        var ev = new StoppingMedicalItemEventArgs(API.Features.Player.Get(__instance.gameObject), __instance._hub.inventory.curItem, __instance.usableItems[i].animationDuration);

                        Player.OnStoppingMedicalItem(ev);

                        __instance._cancel = ev.IsAllowed;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.StoppingMedicalItem:\n{e.ToStringDemystified()}");

                return true;
            }
        }
    }
}
