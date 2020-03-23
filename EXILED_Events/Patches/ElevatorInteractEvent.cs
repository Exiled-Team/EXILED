using Harmony;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUseElevator), typeof(GameObject))]
	public class ElevatorInteractEvent
	{
		public static bool Prefix(PlayerInteract __instance, GameObject elevator)
		{
			if (EventPlugin.ElevatorInteractionEventDisable)
				return true;

			try
			{
				bool allow = true;
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
						Events.InvokeElevatorInteract(__instance.gameObject, elevator2, ref allow);

						if (!allow)
							return false;

						elevator.GetComponent<Lift>().UseLift();
						__instance.OnInteract();
					}
				}

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"ElevatorUseEvent error: {exception}");
				return true;
			}
		}
	}
}