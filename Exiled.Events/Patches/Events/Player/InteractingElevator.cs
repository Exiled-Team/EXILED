// -----------------------------------------------------------------------
// <copyright file="InteractingElevator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1313
    using System;
    using Exiled.Events.EventArgs;
    using Exiled.Events.Handlers;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerInteract.CallCmdUseElevator(GameObject)"/>.
    /// Adds the <see cref="Player.InteractingElevator"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUseElevator), typeof(GameObject))]
    internal class InteractingElevator
    {
        private static bool Prefix(PlayerInteract __instance, GameObject elevator)
        {
            try
            {
                if (!__instance._playerInteractRateLimit.CanExecute(true) ||
                    (__instance._hc.CufferId > 0 && !PlayerInteract.CanDisarmedInteract) || elevator == null)
                    return false;

                Lift component = elevator.GetComponent<Lift>();
                if (component == null)
                    return false;

                foreach (Lift.Elevator elevator2 in component.elevators)
                {
                    if (__instance.ChckDis(elevator2.door.transform.position))
                    {
                        var ev = new InteractingElevatorEventArgs(API.Features.Player.Get(__instance.gameObject), elevator2);

                        Player.OnInteractingElevator(ev);

                        if (!ev.IsAllowed)
                            return false;

                        elevator.GetComponent<Lift>().UseLift();
                        __instance.OnInteract();
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                Exiled.API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.InteractingElevator: {e}\n{e.StackTrace}");

                return true;
            }
        }
    }
}
