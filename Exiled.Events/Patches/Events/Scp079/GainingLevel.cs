// -----------------------------------------------------------------------
// <copyright file="GainingLevel.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp079
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using Mirror;

    /// <summary>
    /// Patches <see cref="Scp079PlayerScript.TargetLevelChanged(NetworkConnection, int)"/>.
    /// Adds the <see cref="Scp079.GainingLevel"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp079PlayerScript), nameof(Scp079PlayerScript.TargetLevelChanged))]
    internal static class GainingLevel
    {
        private static bool Prefix(Scp079PlayerScript __instance, ref int newLvl)
        {
            var ev = new GainingLevelEventArgs(API.Features.Player.Get(__instance.gameObject), __instance.curLvl - 1, newLvl);

            Scp079.OnGainingLevel(ev);

            newLvl = ev.NewLevel;

            if (ev.IsAllowed)
                __instance.Lvl = (byte)newLvl;

            return ev.IsAllowed;
        }
    }
}
