// -----------------------------------------------------------------------
// <copyright file="ChangingScp914KnobSetting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using System;
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using Scp914;

    /// <summary>
    /// Patches <see cref="PlayerInteract.CallCmdChange914Knob"/>.
    /// Adds the <see cref="Player.ChangingScp914KnobSetting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdChange914Knob))]
    public class ChangingScp914KnobSetting
    {
        /// <summary>
        /// Prefix of <see cref="PlayerInteract.CallCmdChange914Knob"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="PlayerInteract"/> instance.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(PlayerInteract __instance)
        {
            if (!__instance._playerInteractRateLimit.CanExecute(true) ||
                (__instance._hc.CufferId > 0 && !__instance.CanDisarmedInteract) ||
                Scp914Machine.singleton.working || !__instance.ChckDis(Scp914Machine.singleton.knob.position) ||
                Math.Abs(Scp914Machine.singleton.curKnobCooldown) > 0.001f)
                return false;

            Scp914Knob knobSetting = Scp914Machine.singleton.knobState;

            if (knobSetting + 1 > Scp914Machine.knobStateMax)
                knobSetting = Scp914Machine.knobStateMin;
            else
                knobSetting += 1;

            var ev = new ChangingScp914KnobSettingEventArgs(API.Features.Player.Get(__instance.gameObject), knobSetting);

            Player.OnChangingScp914KnobSetting(ev);

            if (ev.IsAllowed)
            {
                Scp914Machine.singleton.NetworkknobState = ev.KnobSetting;
                Scp914Machine.singleton.curKnobCooldown = Scp914Machine.singleton.knobCooldown;
                __instance.OnInteract();
            }

            return false;
        }
    }
}