// -----------------------------------------------------------------------
// <copyright file="InteractingElevator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events
{
    #pragma warning disable SA1313
    using Exiled.Events.Handlers;
    using Exiled.Events.Handlers.EventArgs;
    using HarmonyLib;
    using UnityEngine;

    /// <summary>
    /// Patches <see cref="PlayerInteract.CallCmdUseElevator(GameObject)"/>.
    /// Adds the <see cref="Map.InteractingElevator"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUseElevator), typeof(GameObject))]
    public class InteractingElevator
    {
        /// <summary>
        /// Prefix of <see cref="PlayerInteract.CallCmdUseElevator(GameObject)"/>.
        /// </summary>
        /// <param name="__instance">The <see cref="PlayerInteract"/> instance.</param>
        /// <param name="elevator">The elevator game object.</param>
        /// <returns>Returns a value indicating whether the original method has to be executed or not.</returns>
        public static bool Prefix(PlayerInteract __instance, GameObject elevator)
        {
            if (!__instance._playerInteractRateLimit.CanExecute(true) ||
                (__instance._hc.CufferId > 0 && !__instance.CanDisarmedInteract) || elevator == null)
                return false;

            Lift component = elevator.GetComponent<Lift>();
            if (component == null)
                return false;

            foreach (Lift.Elevator elevator2 in component.elevators)
            {
                if (__instance.ChckDis(elevator2.door.transform.position))
                {
                    var ev = new InteractingElevatorEventArgs(API.Features.Player.Get(__instance.gameObject), elevator2);

                    Map.OnInteractingElevator(ev);

                    if (!ev.IsAllowed)
                        return false;

                    elevator.GetComponent<Lift>().UseLift();
                    __instance.OnInteract();
                }
            }

            return false;
        }
    }
}