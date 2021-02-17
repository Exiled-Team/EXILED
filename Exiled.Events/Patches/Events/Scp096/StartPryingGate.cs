// -----------------------------------------------------------------------
// <copyright file="StartPryingGate.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp096
{
#pragma warning disable SA1313
    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Interactables.Interobjects;

    using PlayableScps;

    /// <summary>
    /// Patches the <see cref="Scp096.PryGate"/> method.
    /// Adds the <see cref="Handlers.Scp096.StartPryingGate"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.PryGate))]
    internal static class StartPryingGate
    {
        private static bool Prefix(PlayableScps.Scp096 __instance, PryableDoor gate)
        {
            if (__instance.Charging && __instance.Enraged && (!gate.TargetState /* && gate.doorType == Door.DoorTypes.HeavyGate */))
            {
                var ev = new StartPryingGateEventArgs(API.Features.Player.Get(__instance.Hub.gameObject), gate);
                Exiled.Events.Handlers.Scp096.OnStartPryingGate(ev);
                return ev.IsAllowed;
            }

            return false;
        }
    }
}
