using System;
using EXILED.Shared.Helpers;
using Harmony;

namespace EXILED.Events.Patches
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
				Events.Events.InvokeWarheadEvent(__instance, ref n, ref allow);
				return allow;
			}
			catch (Exception e)
			{
				LogHelper.Error($"Warhead event error: {e}");
				return true;
			}
		}
	}
}