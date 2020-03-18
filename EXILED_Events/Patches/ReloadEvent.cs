using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(WeaponManager), nameof(WeaponManager.CallCmdReload))]
	public class ReloadEvent
	{
		public static bool Prefix(WeaponManager __instance, bool animationOnly)
		{
			if (!__instance._iawRateLimit.CanExecute(false))
				return false;
			try
			{
				int itemIndex = __instance.hub.inventory.GetItemIndex();
				if (itemIndex < 0 || itemIndex >= __instance.hub.inventory.items.Count ||
					(__instance.curWeapon < 0 || __instance.hub.inventory.curItem !=
					 __instance.weapons[__instance.curWeapon].inventoryID) ||
					__instance.hub.inventory.items[itemIndex].durability >=
					(double)__instance.weapons[__instance.curWeapon].maxAmmo)
					return false;

				bool allow = true;
				Events.InvokePlayerReload(__instance.gameObject, ref allow);
				return allow;
			}
			catch (Exception e)
			{
				Log.Error($"Reload Error: {e}");
				return true;
			}
		}
	}
}