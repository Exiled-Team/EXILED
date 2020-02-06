using System;
using Harmony;
using MEC;

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
				if (!__instance._interactRateLimit.CanExecute())
					return false;
				__instance.cancel = false;
				if (__instance.cooldown > 0.0)
					return false;
				for (int mid = 0; mid < __instance.usableItems.Length; ++mid)
				{
					if (__instance.usableItems[mid].inventoryID == __instance.inv.curItem && __instance.usableCooldowns[mid] <= 0.0)
					{
						bool allow = true;
						Events.InvokeUseMedicalItem(__instance.gameObject, __instance.GetComponent<Inventory>().curItem, ref allow);
						if (allow)
							Timing.RunCoroutine(__instance.UseMedicalItem(mid), Segment.FixedUpdate);
					}
				}

				return false;
			}
			catch (Exception e)
			{
				Plugin.Error($"UseMedicalItem event error: {e}");
				return true;
			}
		}
	}
}