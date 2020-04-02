using Harmony;
using MEC;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(ConsumableAndWearableItems), nameof(ConsumableAndWearableItems.CallCmdUseMedicalItem))]
	public class UseMedicalItemEvent
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

				for (int i = 0; i < __instance.usableItems.Length; ++i)
				{
					if (__instance.usableItems[i].inventoryID == __instance.inv.curItem && __instance.usableCooldowns[i] <= 0.0)
					{
						bool allow = true;

						Events.InvokeUseMedicalItem(__instance.gameObject, __instance.inv.curItem, ref __instance.usableItems[i].animationDuration, ref allow);

						if (allow)
							Timing.RunCoroutine(__instance.UseMedicalItem(i), Segment.FixedUpdate);
					}
				}

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"UseMedicalItemEvent error: {exception}");
				return true;
			}
		}
	}
}