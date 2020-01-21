using System.Net;
using Harmony;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Inventory), "CallCmdDropItem")]
	public class DropItemEvent
	{
		public static bool Prefix(Inventory __instance, int itemInventoryIndex)
		{
			if (!__instance._iawRateLimit.CanExecute(true) || itemInventoryIndex < 0 || itemInventoryIndex >= __instance.items.Count)
				return false;
			Inventory.SyncItemInfo syncItemInfo = __instance.items[itemInventoryIndex];
			if (__instance.items[itemInventoryIndex].id != syncItemInfo.id)
				return false;
			
			Events.InvokeDropItem(__instance.gameObject, ref syncItemInfo);
			__instance.SetPickup(syncItemInfo.id, syncItemInfo.durability, __instance.transform.position, __instance.camera.transform.rotation, syncItemInfo.modSight, syncItemInfo.modBarrel, syncItemInfo.modOther);
			__instance.items.RemoveAt(itemInventoryIndex);
			return false;
		}
	}
}