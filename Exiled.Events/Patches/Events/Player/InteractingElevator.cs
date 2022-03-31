// -----------------------------------------------------------------------
// <copyright file="InteractingElevator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using Exiled.Events.EventArgs;

#pragma warning disable SA1313
    /// <summary>
    /// Patches <see cref="PlayerInteract.UserCode_CmdUseElevator(UnityEngine.GameObject)"/>.
    /// Adds the <see cref="Handlers.Player.InteractingElevator"/> event.
    /// </summary>
    [HarmonyLib.HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.UserCode_CmdUseElevator), typeof(UnityEngine.GameObject))]
    internal static class InteractingElevator
    {
        private static bool Prefix(PlayerInteract __instance, UnityEngine.GameObject elevator)
        {
            try
            {
                if (!__instance.CanInteract || elevator is null)
                    return false;

                Lift component = elevator.GetComponent<Lift>();
                if (component is null)
                    return false;

                foreach (Lift.Elevator elevator1 in component.elevators)
                {
                    if (!__instance.ChckDis(elevator1.door.transform.position))
                        continue;

                    InteractingElevatorEventArgs interactingEventArgs = new(API.Features.Player.Get(__instance._hub), elevator1, component);
                    Handlers.Player.OnInteractingElevator(interactingEventArgs);

                    if (interactingEventArgs.IsAllowed)
                    {
                        elevator.GetComponent<Lift>().UseLift();
                        __instance.OnInteract();
                    }
                }

                return false;
            }
            catch (System.Exception e)
            {
                API.Features.Log.Error($"Exiled.Events.Patches.Events.Player.InteractingElevator:\n{e}");

                return true;
            }
        }
    }
}
