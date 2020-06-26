// -----------------------------------------------------------------------
// <copyright file="IndividualFriendlyFireFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1313
    using System;
    using Exiled.API.Features;
    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="WeaponManager.GetShootPermission(CharacterClassManager,bool)"/>.
    /// </summary>
    [HarmonyPatch(typeof(WeaponManager), nameof(WeaponManager.GetShootPermission), new Type[] { typeof(CharacterClassManager), typeof(bool) })]
    public class IndividualFriendlyFireFix
    {
        /// <summary>
        /// Fix NW removing individualized FF bools.
        /// </summary>
        /// <param name="__instance">The WeaponManager instance of the player.</param>
        /// <param name="c">The CCM of the target player.</param>
        /// <param name="forceFriendlyFire">If FF should be forced or not.</param>
        public static void Prefix(WeaponManager __instance, CharacterClassManager c, ref bool forceFriendlyFire)
        {
            forceFriendlyFire = Player.Get(__instance.gameObject).IsFriendlyFireEnabled;
        }
    }
}
