using System.Linq;
using EXILED;
using Grenades;
using Harmony;
using UnityEngine;

namespace JokersPlugin.Patches
{
	[HarmonyPatch(typeof(Scp096PlayerScript), "IncreaseRage")]
	public class Scp096ProcessLookingOverride
	{
			public static bool Prefix(Scp096PlayerScript __instance, float amount)
			{
				if (EXILED.plugin.Scp096PatchDisable)
					return true;
				
				__instance._rageProgress += amount;
				if (__instance._rageProgress < 0.150000005960464)
					return false;
				Events.InvokeScp096Enrage(__instance);
				return true;
			}
		}

		[HarmonyPatch(typeof(Scp096PlayerScript), "DeductRage")]
		public class Scp096EndRage
		{
			public static bool Prefix(Scp096PlayerScript __instance)
			{
				if (EXILED.plugin.Scp096PatchDisable)
					return true;

				bool allow = true;
				Events.InvokeScp096Calm(__instance, ref allow);
				return allow;
			}
		} 
}