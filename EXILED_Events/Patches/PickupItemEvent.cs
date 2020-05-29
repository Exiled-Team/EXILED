using Harmony;
using System;
using System.Runtime.CompilerServices;
using Mirror;
using Searching;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(SearchCoordinator), nameof(SearchCoordinator.ContinuePickupServer))]
	public class PickupItemEvent
	{
		public static bool Prefix(SearchCoordinator __instance)
		{
			if (EventPlugin.PickupItemEventPatchDisable)
				return true;

			try
			{
				if (__instance.Completor.ValidateUpdate())
				{
					if (NetworkTime.time < __instance.SessionPipe.Session.FinishTime)
						return false;

					bool allow = true;
					
					Events.InvokePickupItem(__instance.Hub, __instance.Completor.TargetPickup, ref allow);

					if (allow)
					{
						__instance.Completor.Complete();
						return false;
					}
				}
				
				__instance.SessionPipe.Invalidate();
				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"PickupItemEvent error: {exception}");
				return true;
			}
		}
	}
}