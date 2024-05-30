namespace Exiled.Events.Patches.Fixes
{
    using Exiled.API.Features.CustomStats;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles.FirstPersonControl;
    using UnityEngine;

    /// <summary>
    /// Fix for <see cref="CustomStaminaStat"/>
    /// </summary>
    [HarmonyPatch(typeof(FpcStateProcessor), nameof(FpcStateProcessor.UpdateMovementState))]
    internal static class FixStaminaBar
    {
        private static bool Prefix(FpcStateProcessor __instance, PlayerMovementState state, PlayerMovementState __result)
        {
            if (__instance._stat is not CustomStaminaStat stat)
                return true;

            float height = __instance._mod.CharacterControllerSettings.Height;
            float num1 = height * __instance._mod.CrouchHeightRatio;

            if (__instance.UpdateCrouching(state == PlayerMovementState.Crouching, num1, height) || __instance._firstUpdate)
            {
                __instance._firstUpdate = false;
                float num2 = Mathf.Lerp(0.0f, (float)((height - num1) / 2.0), __instance.CrouchPercent);
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
                if (stat.CurValue > 0.0 && !__instance.SprintingDisabled)
                {
                    stat.CurValue = stat.Clamp(stat.CurValue - (Time.deltaTime * __instance.ServerUseRate));
                    __instance._regenStopwatch.Restart();
                    __result = PlayerMovementState.Sprinting;
                    return false;
                }

                state = PlayerMovementState.Walking;
            }

            if (stat.CurValue >= stat.MaxValue)
            {
                __result = state;
                return false;
            }

            stat.CurValue = stat.Clamp(__instance._stat.CurValue + (__instance.ServerRegenRate * Time.deltaTime));
            __result = state;
            return false;
        }
    }
}