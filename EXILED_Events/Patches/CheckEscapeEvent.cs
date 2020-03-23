using Harmony;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.CallCmdRegisterEscape))]
	public class CheckEscapeEvent
	{
		public static bool Prefix(CharacterClassManager __instance)
		{
			if (EventPlugin.CheckEscapeEventPatchDisable)
				return true;

			try
			{
				if (!__instance._interactRateLimit.CanExecute(false) ||
					 Vector3.Distance(__instance.transform.position,
						__instance.GetComponent<Escape>().worldPosition) >= (double)(Escape.radius * 2))
					return false;

				bool allow = true;

				Events.InvokeCheckEscape(__instance.gameObject, ref allow);

				return allow;
			}
			catch (Exception exception)
			{
				Log.Error($"CheckEscapeEvent error: {exception}");
				return true;
			}
		}
	}
}