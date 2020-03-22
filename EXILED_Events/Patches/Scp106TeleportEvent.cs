using Harmony;
using MEC;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Scp106PlayerScript), nameof(Scp106PlayerScript.CallCmdUsePortal))]
	public class Scp106TeleportEvent
	{
		public static bool Prefix(Scp106PlayerScript __instance)
		{
			if (EventPlugin.Scp106TeleportEventPatchDisable)
				return true;

			try
			{
				if (!__instance._interactRateLimit.CanExecute(false) || !__instance.GetComponent<FallDamage>().isGrounded)
					return false;

				bool allow = true;

				Events.InvokeScp106Teleport(__instance.gameObject, __instance.portalPosition, ref allow);

				if (!allow)
					return false;

				if (__instance.iAm106 && __instance.portalPosition != Vector3.zero && !__instance.goingViaThePortal)
					Timing.RunCoroutine(__instance._DoTeleportAnimation(), Segment.Update);

				return true;
			}
			catch (Exception exception)
			{
				Log.Error($"Scp106TeleportEvent error: {exception}");
				return true;
			}
		}
	}
}
