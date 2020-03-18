using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Inventory), nameof(Inventory.CallCmdSetUnic))]
	public class CallCmdSetUnicOverride
	{
		public static bool Prefix(Inventory __instance, int i)
		{
			try
			{
				if (__instance.itemUniq == i)
					return true;

				int oldItemIndex = __instance.GetItemIndex();

				if (oldItemIndex == -1 && i == -1)
					return true;

				Inventory.SyncItemInfo oldItem = oldItemIndex == -1 ? new Inventory.SyncItemInfo() { id = ItemType.None } : __instance.GetItemInHand();
				Inventory.SyncItemInfo newItem = new Inventory.SyncItemInfo() { id = ItemType.None };

				foreach (Inventory.SyncItemInfo item in __instance.items)
					if (item.uniq == i)
						newItem = item;

				Events.InvokeItemChanged(__instance.gameObject, ref oldItem, newItem);

				oldItemIndex = __instance.GetItemIndex();

				if (oldItemIndex != -1) __instance.items[oldItemIndex] = oldItem;

				return true;
			}
			catch (Exception exception)
			{
				Log.Error($"ItemChanged error {exception}");
				return true;
			}
		}
	}
}
