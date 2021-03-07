// -----------------------------------------------------------------------
// <copyright file="DamagingWindow.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
#pragma warning disable SA1313
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="BreakableWindow.ServerDamageWindow(float)"/>.
    /// Adds the <see cref="Map.DamagingWindow"/> event.
    /// </summary>
    [HarmonyPatch(typeof(BreakableWindow), nameof(BreakableWindow.ServerDamageWindow))]
    internal static class DamagingWindow
    {
        private static void Prefix(BreakableWindow __instance, ref float damage)
        {
            var ev = new DamagingWindowEventArgs(__instance, damage);

            Map.OnDamagingWindow(ev);

            damage = ev.Damage;
        }
    }
}
