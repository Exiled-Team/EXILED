using Harmony;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Scp096PlayerScript), nameof(Scp096PlayerScript.IncreaseRage))]
	public class Scp096EnrageEvent
	{
		public static bool Prefix(Scp096PlayerScript __instance, float amount)
		{
			if (EventPlugin.Scp096PatchDisable)
				return true;

			try
			{
				__instance._rageProgress += amount;

				if (__instance._rageProgress <
					(double)__instance.rageCurve.Evaluate(Mathf.Min(PlayerManager.players.Count, 20)))
					return false;

				bool allow = true;

				Events.InvokeScp096Enrage(__instance, ref allow);

				if (!allow)
					return false;

				__instance.Networkenraged = Scp096PlayerScript.RageState.Panic;
				__instance._rageProgress = 15f;
				__instance.Invoke("StartRage", 5f);

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"Scp096EnrageEvent error: {exception}");
				return true;
			}
		}
	}

	[HarmonyPatch(typeof(Scp096PlayerScript), nameof(Scp096PlayerScript.DeductRage))]
	public class Scp096CalmEvent
	{
		public static bool Prefix(Scp096PlayerScript __instance)
		{
			if (EventPlugin.Scp096PatchDisable)
				return true;

			try
			{
				bool allow = true;

				Events.InvokeScp096Calm(__instance, ref allow);

				return allow;
			}
			catch (Exception exception)
			{
				Log.Error($"Scp096CalmEvent error: {exception}");
				return true;
			}
		}
	}
}