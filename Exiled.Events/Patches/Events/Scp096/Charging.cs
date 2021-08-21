// -----------------------------------------------------------------------
// <copyright file="Charging.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using PlayableScps;

    /// <summary>
    /// Patches <see cref="Scp096.Charge"/>.
    /// Adds the <see cref="Handlers.Scp096.Charging"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.Charge))]
    internal static class Charging
    {
        private static bool Prefix(Scp096 __instance)
        {
            ChargingEventArgs ev = new ChargingEventArgs(__instance, API.Features.Player.Get(__instance.Hub));

            Handlers.Scp096.OnCharging(ev);

            return ev.IsAllowed;
        }
    }
}
