// -----------------------------------------------------------------------
// <copyright file="AntiCheatTriggerByR2WithCustomScale.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using HarmonyLib;

    using UnityEngine;

#pragma warning disable SA1313 // Parameter names should begin with lower-case letter

    /// <summary>
    /// Patches <see cref="PlayerMovementSync.AnticheatRaycast(UnityEngine.Vector3, bool)"/>.
    /// Fixes triggering due to the R.2 code by using the Y coordinate from the player's scale to multiply the offset.
    /// </summary>
    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.AnticheatRaycast), new[] { typeof(Vector3), typeof(bool) })]
    internal static class AntiCheatTriggerByR2WithCustomScale
    {
        private static void Prefix(PlayerMovementSync __instance, ref Vector3 offset) => offset.y *= __instance.transform.localScale.y;
    }
}
