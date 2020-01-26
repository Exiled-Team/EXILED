using System;
using EXILED.Shared;
using Grenades;
using Harmony;

namespace EXILED.Events.Patches
{
	[HarmonyPatch(typeof(GrenadeManager), nameof(GrenadeManager.CallCmdThrowGrenade))]
	public class GrenadeThrownEvent
	{
		public static bool Prefix(ref GrenadeManager __instance, ref int id, ref bool slowThrow, ref double time)
		{
			if (EventPlugin.GrenadeThrownPatchDisable)
				return true;

			try
			{
				bool allow = true;
				Events.Events.InvokeGrenadeThrown(ref __instance, ref id, ref slowThrow, ref time, ref allow);
				return allow;
			}
			catch (Exception e)
			{
				Plugin.Error($"Grenade thrown patch error: {e}");
				return true;
			}
		}
	}
}