// -----------------------------------------------------------------------
// <copyright file="InteractingEvents.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp914
{
#pragma warning disable SA1313
    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Scp914;

    using global::Scp914;

    using HarmonyLib;

    using Scp914 = Exiled.Events.Handlers.Scp914;

    /// <summary>
    ///     Patches <see cref="Scp914Controller.ServerInteract" />.
    ///     Adds the <see cref="Handlers.Scp914.Activating" /> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp914Controller), nameof(Scp914Controller.ServerInteract))]
    internal static class InteractingEvents
    {
        private static bool Prefix(Scp914Controller __instance, ReferenceHub ply, byte colliderId)
        {
            if (__instance._remainingCooldown > 0.0)
                return false;
            switch ((Scp914InteractCode) colliderId)
            {
                case Scp914InteractCode.ChangeMode:
                    Scp914KnobSetting scp914KnobSetting;
                    if (__instance._knobSetting + 1 > Scp914KnobSetting.VeryFine)
                        scp914KnobSetting = Scp914KnobSetting.Rough;
                    else
                        scp914KnobSetting = __instance._knobSetting + 1;
                    ChangingKnobSettingEventArgs ev = new(Player.Get(ply), scp914KnobSetting);

                    Scp914.OnChangingKnobSetting(ev);
                    if (!ev.IsAllowed)
                        return false;

                    __instance._remainingCooldown = __instance._knobChangeCooldown;
                    scp914KnobSetting = ev.KnobSetting;
                    __instance.Network_knobSetting = scp914KnobSetting;
                    __instance.RpcPlaySound(0);
                    break;
                case Scp914InteractCode.Activate:
                    ActivatingEventArgs ev2 = new(Player.Get(ply));

                    Scp914.OnActivating(ev2);

                    if (!ev2.IsAllowed)
                        return false;
                    __instance._remainingCooldown = __instance._totalSequenceTime;
                    __instance._isUpgrading = true;
                    __instance._itemsAlreadyUpgraded = false;
                    __instance.RpcPlaySound(1);
                    break;
            }

            return false;
        }
    }
}
