// -----------------------------------------------------------------------
// <copyright file="DamagingWindow.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Map
{
    using HarmonyLib;
    using Mirror;

    /// <summary>
    /// Patches <see cref="BreakableWindow.ServerDamageWindow(float)"/>.
    /// Adds the <see cref="Handlers.Map.DamagingWindow"/> event.
    /// </summary>
    [HarmonyPatch(typeof(BreakableWindow), nameof(BreakableWindow.ServerDamageWindow))]
    internal static class DamagingWindow
    {
        internal static bool Prefix(BreakableWindow __instance, ref float damage)
        {
            if (NetworkServer.active)
            {
                var ev = new EventArgs.DamagingWindowEventArgs(__instance, damage);
                Handlers.Map.OnDamagingWindow(ev);
                __instance.health -= ev.Damage;
                if (__instance.health <= 0f)
                {
                    __instance.StartCoroutine(__instance.BreakWindow());
                }
            }
            return false;
        }
    }
}
