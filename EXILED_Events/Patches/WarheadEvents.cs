using Harmony;
using Mirror;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.Detonate))]
	public class WarheadDetonationEvent
	{
		public static void Prefix(AlphaWarheadController __instance)
		{
			try
			{
				Events.InvokeWarheadDetonation();
			}
			catch (Exception exception)
			{
				Log.Error($"WarheadDetonationEvent error: {exception}");
			}
		}
	}

	[HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.CancelDetonation), new Type[] { typeof(GameObject) })]
	public class WarheadCancelledEvent
	{
		public static bool Prefix(AlphaWarheadController __instance, GameObject disabler)
		{
			try
			{
				ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Detonation cancelled.", ServerLogs.ServerLogType.GameEvent);

				if (!__instance.inProgress || __instance.timeToDetonation <= 10.0)
					return false;

				if (__instance.timeToDetonation <= 15.0 && disabler != null)
					__instance.GetComponent<PlayerStats>().TargetAchieve(disabler.GetComponent<NetworkIdentity>().connectionToClient, "thatwasclose");

				bool allow = true;

				Events.InvokeWarheadCancel(disabler, ref allow);

				return allow && !EventPlugin.WarheadLocked;
			}
			catch (Exception exception)
			{
				Log.Error($"WarheadCancelledEvent error: {exception}");
				return true;
			}
		}
	}

	[HarmonyPatch(typeof(AlphaWarheadController), nameof(AlphaWarheadController.StartDetonation))]
	public class WarheadStartEvent
	{
		public static bool Prefix(AlphaWarheadController __instance)
		{
			if (EventPlugin.WarheadStartEventPatchDisable)
				return true;

			try
			{
				if (Recontainer079.isLocked)
					return false;

				__instance.doorsOpen = false;

				ServerLogs.AddLog(ServerLogs.Modules.Warhead, "Countdown started.", ServerLogs.ServerLogType.GameEvent);

				if ((AlphaWarheadController._resumeScenario != -1 || __instance.scenarios_start[AlphaWarheadController._startScenario].SumTime() != (double)__instance.timeToDetonation) && (AlphaWarheadController._resumeScenario == -1 || __instance.scenarios_resume[AlphaWarheadController._resumeScenario].SumTime() != (double)__instance.timeToDetonation))
					return false;

				bool allow = true;

				Events.InvokeWarheadStart(ref allow);

				if (allow)
					__instance.NetworkinProgress = true;

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"WarheadStartEvent error: {exception}");
				return true;
			}
		}
	}
}