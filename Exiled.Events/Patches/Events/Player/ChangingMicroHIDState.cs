// -----------------------------------------------------------------------
// <copyright file="ChangingMicroHIDState.cs" company="Exiled Team">
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
    /// Patches <see cref="MicroHID.NetworkCurrentHidState"/>.
    /// Adds the <see cref="Handlers.Player.OnChangingMicroHIDState"/> event.
    /// </summary>
    [HarmonyPatch(typeof(MicroHID), nameof(MicroHID.NetworkCurrentHidState), MethodType.Setter)]
    internal static class ChangingMicroHIDState
    {
        private static bool Prefix(MicroHID __instance, ref MicroHID.MicroHidState value)
        {
            // NetworkCurrentHid state is set each frame, so this  is to prevent calling the method each frame.
            if (__instance.CurrentHidState == value)
                return true;

            var ev = new ChangingMicroHIDStateEventArgs(Player.Get(__instance.gameObject), __instance.CurrentHidState, value);

            Handlers.Player.OnChangingMicroHIDState(ev);

            if (!ev.IsAllowed)
                return false;

            value = ev.NewState;

            return true;
        }
    }
}
