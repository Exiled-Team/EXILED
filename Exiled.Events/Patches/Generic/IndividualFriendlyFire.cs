// -----------------------------------------------------------------------
// <copyright file="IndividualFriendlyFire.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
    using System;

    using Exiled.API.Features;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="WeaponManager.GetShootPermission(CharacterClassManager, bool)"/>.
    /// </summary>
    [HarmonyPatch(typeof(WeaponManager), nameof(WeaponManager.GetShootPermission), new Type[] { typeof(CharacterClassManager), typeof(bool) })]
    internal static class IndividualFriendlyFire
    {
        private static bool Prefix(WeaponManager __instance, ref bool forceFriendlyFire, ref bool __result) => !(__result = Player.Get(__instance.gameObject).IsFriendlyFireEnabled || forceFriendlyFire || ServerConsole.FriendlyFire);
    }
}
