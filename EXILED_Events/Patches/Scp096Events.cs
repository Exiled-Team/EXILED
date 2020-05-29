using Harmony;
using System;
using PlayableScps;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Scp096), nameof(Scp096.Enrage))]
	public class Scp096EnrageEvent
	{
		public static bool Prefix(Scp096 __instance)
		{
			if (EventPlugin.Scp096PatchDisable)
				return true;

			try
			{
				bool allow = true;

				Events.InvokeScp096Enrage(__instance, ref allow);

				if (!allow)
					return false;

				if (__instance.Enraged)
				{
					__instance.AddReset();
				}
				else
				{
					__instance.SetMovementSpeed(12f);
					__instance.SetJumpHeight(10f);
					__instance.PlayerState = Scp096PlayerState.Enraged;
					__instance.EnrageTimeLeft = __instance.EnrageTime;
				}

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"Scp096EnrageEvent error: {exception}");
				return true;
			}
		}
	}

	[HarmonyPatch(typeof(Scp096), nameof(Scp096.EndEnrage))]
	public class Scp096CalmEvent
	{
		public static bool Prefix(Scp096 __instance)
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