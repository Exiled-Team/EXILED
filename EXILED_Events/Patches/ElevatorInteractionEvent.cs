﻿using System;
using Harmony;
using UnityEngine;

namespace EXILED.Patches
{
    [HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUseElevator), typeof(GameObject))]
    public class ElevatorUseEvent
    {
        public static bool Prefix(PlayerInteract _instance, GameObject elevator)
        {
            if (EventPlugin.ElevatorInteractionEventDisable)
                return true;

            try
            {
                bool allow = true;
                if (!_instance._playerInteractRateLimit.CanExecute(true) || 
                    (_instance._hc.CufferId > 0 && !_instance.CanDisarmedInteract) || elevator == null) 
                    return false;
                    
                Lift component = elevator.GetComponent<Lift>();
                if (component == null)
                    return false;

                foreach (Lift.Elevator elevator2 in component.elevators)
                {
                    if (_instance.ChckDis(elevator2.door.transform.position))
                    {
                        Events.InvokeElevatorInteract(_instance.gameObject, elevator2, ref allow);

                        if (!allow)
                            return false;

                        elevator.GetComponent<Lift>().UseLift();
                        _instance.OnInteract();
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                Log.Error($"Elevator Use Error: {e}");
                return true;
            }
        }
    }
}