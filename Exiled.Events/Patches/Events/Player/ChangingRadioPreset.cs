// -----------------------------------------------------------------------
// <copyright file="ChangingRadioPreset.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using Exiled.API.Features;
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    /// <summary>
    /// Patches <see cref="Radio.NetworkcurRangeId"/>.
    /// Adds the <see cref="Handlers.Player.ChangingRadioPreset"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Radio), nameof(Radio.NetworkcurRangeId), MethodType.Setter)]
    internal static class ChangingRadioPreset
    {
        private static bool Prefix(Radio __instance, ref byte value)
        {
            var ev = new ChangingRadioPresetEventArgs(Player.Get(__instance.gameObject), __instance.NetworkcurRangeId, value);

            Handlers.Player.OnChangingRadioPreset(ev);

            if (!ev.IsAllowed)
                return false;

            value = ev.NewValue;

            return true;
        }
    }
}
