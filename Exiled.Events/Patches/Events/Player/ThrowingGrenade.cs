// -----------------------------------------------------------------------
// <copyright file="ThrowingGrenade.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using Grenades;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="GrenadeManager.CallCmdThrowGrenade(int, bool, double)"/>.
    /// Adds the <see cref="Player.ThrowingGrenade"/> event.
    /// </summary>
    [HarmonyPatch(typeof(GrenadeManager), nameof(GrenadeManager.CallCmdThrowGrenade))]
    public class ThrowingGrenade
    {
        /// <summary>
        /// Prefix of <see cref="GrenadeManager.CallCmdThrowGrenade(int, bool, double)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="GrenadeManager"/> instance.</param>
        /// <param name="id"><inheritdoc cref="ThrowingGrenadeEventArgs.Id"/></param>
        /// <param name="slowThrow"><inheritdoc cref="ThrowingGrenadeEventArgs.IsSlow"/></param>
        /// <param name="time"><inheritdoc cref="ThrowingGrenadeEventArgs.FuseTime"/></param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(ref GrenadeManager __instance, ref int id, ref bool slowThrow, ref double time)
        {
            var ev = new ThrowingGrenadeEventArgs(API.Features.Player.Get(__instance.gameObject), __instance, id, slowThrow, time);

            Player.OnThrowingGrenade(ev);

            return ev.IsAllowed;
        }
    }
}
