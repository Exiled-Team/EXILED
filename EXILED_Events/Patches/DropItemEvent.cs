using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Inventory), nameof(Inventory.CallCmdDropItem))]
	public class DropItemEvent
	{
		public static bool Prefix(Inventory __instance, int itemInventoryIndex)
		{
			if (EventPlugin.DropItemEventPatchDisable)
				return true;

			try
			{
				if (!__instance._iawRateLimit.CanExecute(true) || itemInventoryIndex < 0 || itemInventoryIndex >= __instance.items.Count)
					return false;

				Inventory.SyncItemInfo syncItemInfo = __instance.items[itemInventoryIndex];

				if (__instance.items[itemInventoryIndex].id != syncItemInfo.id)
					return false;

				bool allow = true;

				Events.InvokeDropItem(__instance.gameObject, ref syncItemInfo, ref allow);

				if (!allow)
					return false;

				Pickup dropped = __instance.SetPickup(syncItemInfo.id, syncItemInfo.durability, __instance.transform.position,
					__instance.camera.transform.rotation, syncItemInfo.modSight, syncItemInfo.modBarrel,
					syncItemInfo.modOther);

				__instance.items.RemoveAt(itemInventoryIndex);

				Events.InvokeItemDropped(__instance.gameObject, dropped, syncItemInfo);

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"DropItemEvent error: {exception}");
				return true;
			}
		}
	}
}