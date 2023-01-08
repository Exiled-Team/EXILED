// -----------------------------------------------------------------------
// <copyright file="StaminaUsage.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313

    using Exiled.API.Features;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles.FirstPersonControl;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="FpcStateProcessor.UpdateMovementState"/>.
    /// Implements <see cref="Player.IsUsingStamina"/>.
    /// </summary>
    [HarmonyPatch(typeof(FpcStateProcessor), nameof(FpcStateProcessor.UpdateMovementState))]
    internal class StaminaUsage
    {
        private static bool Prefix(FpcStateProcessor __instance, PlayerMovementState state, ref PlayerMovementState __result)
        {
            if (!Player.TryGet(__instance._hub, out Player player))
                return true;

            bool isCrouching = state == PlayerMovementState.Crouching;
            float height = __instance._mod.CharacterControllerSettings.Height;
            float num1 = height * __instance._mod.CrouchHeightRatio;

            if (__instance.UpdateCrouching(isCrouching, num1, height) || __instance._firstUpdate)
            {
                __instance._firstUpdate = false;

                float num2 = Mathf.Lerp(0.0f, (float)((height - (double)num1) / 2.0), __instance.CrouchPercent);
                float num3 = Mathf.Lerp(height, num1, __instance.CrouchPercent);

                float radius = __instance._mod.CharController.radius;

                __instance._mod.CharController.height = num3;
                __instance._mod.CharController.center = Vector3.down * num2;
                __instance._camPivot.localPosition = Vector3.up * (float)((num3 / 2.0) - num2 - radius + 0.08799999952316284);
            }

            if (!NetworkServer.active || __instance._useRate == 0.0)
            {
                __result = state;
                return false;
            }

            if (state == PlayerMovementState.Sprinting)
            {
                if (__instance._stat.CurValue > 0.0 && !__instance.SprintingDisabled)
                {
                    __instance._stat.CurValue = !player.IsUsingStamina ? 1 : Mathf.Clamp01(__instance._stat.CurValue - (Time.deltaTime * __instance.ServerUseRate));
                    __instance._regenStopwatch.Restart();
                    __result = PlayerMovementState.Sprinting;
                    return false;
                }

                state = PlayerMovementState.Walking;
            }

            if (__instance._stat.CurValue >= 1.0)
            {
                __result = state;
                return false;
            }

            __instance._stat.CurValue = !player.IsUsingStamina ? 1 : Mathf.Clamp01(__instance._stat.CurValue + (__instance.ServerRegenRate * Time.deltaTime));
            __result = state;
            return false;
        }
    }
}