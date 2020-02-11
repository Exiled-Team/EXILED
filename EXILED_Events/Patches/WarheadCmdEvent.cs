using System;
using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerInteract), "CallCmdUsePanel")]
	public class WarheadLockPatch
	{
		public static bool Prefix(PlayerInteract __instance, string n)
		{
			if (EventPlugin.WarheadLockPatchDisable)
				return true;

			try
			{
				bool allow = true;
				Events.InvokeWarheadEvent(__instance, ref n, ref allow);
				return !EventPlugin.WarheadLocked && allow;
			}
			catch (Exception e)
			{
				Log.Error($"Warhead event error: {e}");
				return true;
			}
		}
	}
}