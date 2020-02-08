using System;
using Harmony;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.CallCmdRegisterEscape))]
	public class CheckEscapeEvent
	{
		public static bool Prefix(CharacterClassManager __instance)
		{
			try
			{
				if (EventPlugin.CheckEscapeEventPatchDisable)
					return true;

				if (!__instance._interactRateLimit.CanExecute(true) ||
				    (double) Vector3.Distance(__instance.transform.position,
					    __instance.GetComponent<Escape>().worldPosition) >= (double) (Escape.radius * 2))
					return false;

				bool allow = true;
				Events.InvokeCheckEscape(__instance.gameObject, ref allow);
				return allow;
			}
			catch (Exception e)
			{
				Log.Error($"CheckEscape Error: {e}");
				return true;
			}
		}
	}
}