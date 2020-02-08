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
				if (EventPlugin.Scp096PatchDisable)
					return true;
				
				__instance._rageProgress += amount;
				if (__instance._rageProgress <
				    (double) __instance.rageCurve.Evaluate(Mathf.Min(PlayerManager.players.Count, 20)))
					return false;
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
				Log.Error($"SCP-096 Enrage event: {e}");
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
					if (EventPlugin.Scp096PatchDisable)
						return true;
					
					bool allow = true;
					Events.InvokeScp096Calm(__instance, ref allow);
					return allow;
				}
				catch (Exception e)
				{
					Log.Error($"SCP-096 Calm event: {e}");
					return true;
				}
			}
		} 
}