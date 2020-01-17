using System;
using Harmony;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Scp096PlayerScript), "IncreaseRage")]
	public class Scp096ProcessLookingOverride
	{
		public static bool Prefix(Scp096PlayerScript __instance, float amount)
		{
			try
			{
				Plugin.Info($"Scp-096 Enrage event check.");
				if (EventPlugin.Scp096PatchDisable)
					return true;

				Plugin.Info($"Scp-096 Enrage event check2");
				__instance._rageProgress += amount;
				if (__instance._rageProgress <
				    (double) __instance.rageCurve.Evaluate(Mathf.Min(PlayerManager.players.Count, 20)))
					return false;
				Plugin.Info($"Scp-096 Enrage event firing..");
				bool allow = true;
				Events.InvokeScp096Enrage(__instance, ref allow);
				if (allow == false)
					return false;
				__instance.Networkenraged = Scp096PlayerScript.RageState.Panic;
				__instance._rageProgress = 15f;
				__instance.Invoke("StartRage", 5f);

				return false;
			}
			catch (Exception e)
			{
				Plugin.Error($"SCP-096 Enrage event: {e}");
				return true;
			}
		}
	}

		[HarmonyPatch(typeof(Scp096PlayerScript), "DeductRage")]
		public class Scp096EndRage
		{
			public static bool Prefix(Scp096PlayerScript __instance)
			{
				try
				{
					Plugin.Info($"Scp-096 Calm event check.");
					if (EventPlugin.Scp096PatchDisable)
						return true;

					Plugin.Info($"Scp-096 calm event firing..");
					bool allow = true;
					Events.InvokeScp096Calm(__instance, ref allow);
					return allow;
				}
				catch (Exception e)
				{
					Plugin.Error($"SCP-096 Calm event: {e}");
					return true;
				}
			}
		} 
}