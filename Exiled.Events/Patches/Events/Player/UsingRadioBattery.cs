// -----------------------------------------------------------------------
// <copyright file="UsingRadioBattery.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using Exiled.Events.Handlers;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="Radio.UseBattery()"/>.
    /// Adds the <see cref="Player.UsingRadioBattery"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Radio), nameof(Radio.UseBattery))]
    internal static class UsingRadioBattery
    {
        internal static bool Prefix(Radio __instance)
        {
            var ply = API.Features.Player.Get(__instance.gameObject);
            if (ply != null && __instance.CheckRadio() && __instance.inv.items[__instance.myRadio].id == ItemType.Radio)
            {
                var ev = new EventArgs.UsingRadioBatteryEventArgs(ply, __instance, __instance.isTransmitting);
                Player.OnRadioBatteryUsing(ev);
                if (!ev.IsAllowed)
                    return false;
            }
            return true;
        }
    }
}
