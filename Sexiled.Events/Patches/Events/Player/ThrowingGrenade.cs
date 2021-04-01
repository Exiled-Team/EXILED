// -----------------------------------------------------------------------
// <copyright file="ThrowingGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using Sexiled.API.Enums;
using Sexiled.API.Features;
using Sexiled.Events.EventArgs;

namespace Sexiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Sexiled.API.Enums;
    using Sexiled.Events.EventArgs;
    using Sexiled.Events.Handlers;

    using Grenades;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="GrenadeManager.CallCmdThrowGrenade(int, bool, double)"/>.
    /// Adds the <see cref="Handlers.Player.ThrowingGrenade"/> event.
    /// </summary>
    [HarmonyPatch(typeof(GrenadeManager), nameof(GrenadeManager.CallCmdThrowGrenade))]
    internal static class ThrowingGrenade
    {
        private static bool Prefix(ref GrenadeManager __instance, ref int id, ref bool slowThrow, ref double time)
        {
            try
            {
                var ev = new ThrowingGrenadeEventArgs(API.Features.Player.Get(__instance.gameObject), __instance, (GrenadeType)id, slowThrow, time);

                Handlers.Player.OnThrowingGrenade(ev);

                id = (int)ev.Type;
                slowThrow = ev.IsSlow;
                time = ev.FuseTime;

                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Log.Error($"Sexiled.Events.Patches.Events.Player.ThrowingGrenade: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
