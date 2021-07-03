// -----------------------------------------------------------------------
// <copyright file="ReceivingStatusEffect.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;

    using CustomPlayerEffects;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using MEC;

    using Mirror;

    using Log = Exiled.API.Features.Log;

    /// <summary>
    /// Patches the <see cref="PlayerEffect.ServerChangeIntensity"/> method.
    /// Adds the <see cref="Handlers.Player.ReceivingEffect"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerEffect), nameof(PlayerEffect.ServerChangeIntensity))]
    internal static class ReceivingStatusEffect
    {
        private static bool Prefix(PlayerEffect __instance, byte newState)
        {
            try
            {
                if (__instance.Intensity == newState)
                    return false;

                var ev = new ReceivingEffectEventArgs(API.Features.Player.Get(__instance.Hub.gameObject), __instance, newState, __instance.Intensity);
                Exiled.Events.Handlers.Player.OnReceivingEffect(ev);

                if (!ev.IsAllowed)
                    return false;

                byte intensity = ev.CurrentState;
                __instance.Intensity = ev.State;
                if (NetworkServer.active)
                    __instance.Hub.playerEffectsController.Resync();
                __instance.ServerOnIntensityChange(intensity, ev.State);
                if (ev.Duration > 0.0f)
                    Timing.CallDelayed(0.5f, () => __instance.Duration = ev.Duration);
                return false;
            }
            catch (Exception e)
            {
                Log.Error($"RecievingEffect: {e}");
                return true;
            }
        }
    }
}
