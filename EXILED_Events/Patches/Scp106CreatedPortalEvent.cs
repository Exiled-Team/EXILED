using Harmony;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.CallCmdMakePortal))]
	public class Scp106CreatedPortalEvent
	{
		public static bool Prefix(Scp106PlayerScript __instance)
		{
			if (EventPlugin.Scp106CreatedPortalEventDisable)
				return true;

			try
			{
				if (!__instance._interactRateLimit.CanExecute(true) || !__instance.GetComponent<FallDamage>().isGrounded)
					return false;

				bool rayCastHit = Physics.Raycast(new Ray(__instance.transform.position, -__instance.transform.up), out RaycastHit raycastHit, 10f, __instance.teleportPlacementMask);

				bool allow = true;
				Vector3 portalPosition = raycastHit.point - Vector3.up;

				Events.InvokeScp106CreatedPortal(__instance.gameObject, ref allow, ref portalPosition);

				Debug.DrawRay(__instance.transform.position, -__instance.transform.up, Color.red, 10f);

				if (allow && __instance.iAm106 && !__instance.goingViaThePortal && rayCastHit)
					__instance.SetPortalPosition(portalPosition);

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"Scp106CreatedPortalEvent error: {exception}");
				return true;
			}

		}
	}
}
