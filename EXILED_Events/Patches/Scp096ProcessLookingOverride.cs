using System;
using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Scp096PlayerScript), "IncreaseRage")]
	public class Scp096ProcessLookingOverride
	{
			public static bool Prefix(Scp096PlayerScript __instance, float amount)
			{
				if (EventPlugin.Scp096PatchDisable)
					return true;
				__instance._rageProgress += amount;
				if (__instance._rageProgress < 0.150000005960464)
					return false;

				try
				{
					Events.InvokeScp096Enrage(__instance);
					return true;
				}
				catch (Exception e)
				{
					Plugin.Error($"SCP096 Enrage event error: {e}");
					return true;
				}
			}
		}

		[HarmonyPatch(typeof(Scp096PlayerScript), "DeductRage")]
		public class Scp096EndRage
		{
			public static bool Prefix(Scp096PlayerScript __instance)
			{
				if (EventPlugin.Scp096PatchDisable)
					return true;

				bool allow = true;
				Events.InvokeScp096Calm(__instance, ref allow);
				return allow;
			}
		} 
}