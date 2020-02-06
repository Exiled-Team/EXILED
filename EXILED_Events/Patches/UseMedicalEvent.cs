using System;
using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(ConsumableAndWearableItems), "CallCmdUseMedicalItem")]
	public class UseMedicalEvent
	{
		public static bool Prefix(ConsumableAndWearableItems __instance)
		{
			if (EventPlugin.UseMedicalPatchDisable)
				return true;

			try
			{
				bool allow = true;
				Events.InvokeUseMedicalItem(__instance.gameObject, __instance.GetComponent<Inventory>().curItem, ref allow);
				return allow;
			}
			catch (Exception e)
			{
				Plugin.Error($"UseMedicalItem event error: {e}");
				return true;
			}
		}
	}
}