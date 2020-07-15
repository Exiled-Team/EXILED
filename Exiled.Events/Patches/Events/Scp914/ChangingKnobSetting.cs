// -----------------------------------------------------------------------
// <copyright file="ChangingKnobSetting.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp914
{
#pragma warning disable SA1313
    using System;

    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;

    using global::Scp914;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="PlayerInteract.CallCmdChange914Knob"/>.
    /// Adds the <see cref="Scp914.ChangingKnobSetting"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdChange914Knob))]
    internal class ChangingKnobSetting
    {
        private static bool Prefix(PlayerInteract __instance)
        {
            if (!__instance._playerInteractRateLimit.CanExecute(true) ||
                (__instance._hc.CufferId > 0 && !PlayerInteract.CanDisarmedInteract) ||
                Scp914Machine.singleton.working || !__instance.ChckDis(Scp914Machine.singleton.knob.position) ||
                Math.Abs(Scp914Machine.singleton.curKnobCooldown) > 0.001f)
                return false;

            var ev = new ChangingKnobSettingEventArgs(API.Features.Player.Get(__instance.gameObject), Scp914Machine.singleton.knobState + 1);

            Scp914.OnChangingKnobSetting(ev);

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
