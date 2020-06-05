// -----------------------------------------------------------------------
// <copyright file="SetRandomRoles.cs" company="Exiled Team">
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
    using MEC;
    using Mirror;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.SetRandomRoles"/>.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetRandomRoles))]
    public class SetRandomRoles
    {
        /// <summary>
        /// Prefix of <see cref="CharacterClassManager.SetRandomRoles"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="CharacterClassManager"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(CharacterClassManager __instance)
        {
            try
            {
                if (__instance.isLocalPlayer && __instance.isServer)
                    __instance.RunSmartClassPicker();

                if (NetworkServer.active)
                    Timing.RunCoroutine(__instance.MakeSureToSetHPAndStamina(), Segment.FixedUpdate);

                return false;
            }
            catch (Exception exception)
            {
                Log.Error($"SetRandomRoles error: {exception}");
                return true;
            }
        }
    }
}