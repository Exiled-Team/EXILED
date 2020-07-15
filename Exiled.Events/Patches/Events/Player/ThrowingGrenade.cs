// -----------------------------------------------------------------------
// <copyright file="ThrowingGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using Grenades;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="GrenadeManager.CallCmdThrowGrenade(int, bool, double)"/>.
    /// Adds the <see cref="Player.ThrowingGrenade"/> event.
    /// </summary>
    [HarmonyPatch(typeof(GrenadeManager), nameof(GrenadeManager.CallCmdThrowGrenade))]
    internal class ThrowingGrenade
    {
        private static bool Prefix(ref GrenadeManager __instance, ref int id, ref bool slowThrow, ref double time)
        {
            try
            {
                var ev = new ThrowingGrenadeEventArgs(API.Features.Player.Get(__instance.gameObject), __instance, id, slowThrow, time);

                Player.OnThrowingGrenade(ev);

                id = ev.Id;
                slowThrow = ev.IsSlow;
                time = ev.FuseTime;

                return ev.IsAllowed;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.ThrowingGrenade: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
