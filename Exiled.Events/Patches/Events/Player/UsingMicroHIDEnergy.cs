// -----------------------------------------------------------------------
// <copyright file="UsingMicroHIDEnergy.cs" company="Exiled Team">
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
    /// Patches <see cref="MicroHID.NetworkEnergy"/>.
    /// Adds the <see cref="Handlers.Player.OnUsingMicroHIDEnergy"/> event.
    /// </summary>
    [HarmonyPatch(typeof(MicroHID), nameof(MicroHID.NetworkEnergy), MethodType.Setter)]
    internal static class UsingMicroHIDEnergy
    {
        private static bool Prefix(MicroHID __instance, ref float value)
        {
            var ev = new UsingMicroHIDEnergyEventArgs(Player.Get(__instance.gameObject), __instance.CurrentHidState, __instance.Energy, value);

            Handlers.Player.OnUsingMicroHIDEnergy(ev);

            if (!ev.IsAllowed)
                return false;

            value = ev.NewValue;

            return true;
        }
    }
}
