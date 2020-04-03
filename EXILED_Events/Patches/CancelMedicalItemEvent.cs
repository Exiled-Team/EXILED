using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(ConsumableAndWearableItems), nameof(ConsumableAndWearableItems.CallCmdCancelMedicalItem))]
	public class CancelMedicalItemEvent
	{
		public static bool Prefix(ConsumableAndWearableItems __instance)
		{
			if (EventPlugin.CancelMedicalEventPatchDisable)
				return true;

			try
			{
				if (!__instance._interactRateLimit.CanExecute(true))
					return false;

				for (int i = 0; i < __instance.usableItems.Length; ++i)
				{
					if (__instance.usableItems[i].inventoryID == __instance.inv.curItem && __instance.usableItems[i].cancelableTime > 0f)
					{
						bool allow = true;

						Events.InvokeCancelMedicalItem(__instance.gameObject, __instance.inv.curItem, ref __instance.usableItems[i].animationDuration, ref allow);

						__instance.cancel = allow;
					}
				}

				return false;

			}
			catch (Exception exception)
			{
				Log.Error($"CancelMedicalItemEvent error: {exception}");
				return true;
			}
		}
	}
}
