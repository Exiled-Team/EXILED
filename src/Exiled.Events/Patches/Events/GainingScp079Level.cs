// -----------------------------------------------------------------------
// <copyright file="GainingScp079Level.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using Mirror;

    /// <summary>
    /// Patches <see cref="Scp079PlayerScript.TargetLevelChanged(NetworkConnection, int)"/>.
    /// Adds the <see cref="Player.GainingScp079Level"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.TargetLevelChanged))]
    public class GainingScp079Level
    {
        /// <summary>
        /// Prefix of <see cref="Scp079PlayerScript.TargetLevelChanged(NetworkConnection, int)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="Scp079PlayerScript"/> instance.</param>
        /// <param name="conn">The player's connection.</param>
        /// <param name="newLvl"><inheritdoc cref="GainingScp079LevelEventArgs.NewLevel"/></param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(Scp079PlayerScript __instance, NetworkConnection conn, ref int newLvl)
        {
            var ev = new GainingScp079LevelEventArgs(API.Features.Player.Get(__instance.gameObject), __instance.curLvl, newLvl);

            Player.OnGainingScp079Level(ev);

            newLvl = ev.NewLevel;

            return ev.IsAllowed;
        }
    }
}