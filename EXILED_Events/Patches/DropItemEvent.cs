using System;
using System.Net;
using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Inventory), "CallCmdDropItem")]
	public class DropItemEvent
	{
		public static bool Prefix(Inventory __instance, int itemInventoryIndex)
		{
			try
			{
				if (!__instance._iawRateLimit.CanExecute(true) || itemInventoryIndex < 0 ||
				    itemInventoryIndex >= __instance.items.Count)
					return false;
				Inventory.SyncItemInfo syncItemInfo = __instance.items[itemInventoryIndex];
				if (__instance.items[itemInventoryIndex].id != syncItemInfo.id)
					return false;

				bool allow = true;
				Events.InvokeDropItem(__instance.gameObject, ref syncItemInfo, ref allow);
				if (!allow)
					return false;
				
				__instance.SetPickup(syncItemInfo.id, syncItemInfo.durability, __instance.transform.position,
					__instance.camera.transform.rotation, syncItemInfo.modSight, syncItemInfo.modBarrel,
					syncItemInfo.modOther);
				__instance.items.RemoveAt(itemInventoryIndex);
				return false;
			}
			catch (Exception e)
			{
				Plugin.Error($"Drop Item error: {e}");
				return true;
			}
		}
	}
}