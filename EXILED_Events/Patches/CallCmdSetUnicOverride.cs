using Harmony;

namespace EXILED.Patches
{
    [HarmonyPatch(typeof(Inventory), nameof(Inventory.CallCmdSetUnic))]
    public class CallCmdSetUnicOverride
    {
        public static bool Prefix(Inventory __instance, int i)
        {
            int oldItemIndex = __instance.GetItemIndex();

            if (__instance.itemUniq == i || (oldItemIndex == -1 && i == -1))
                return true;

            Inventory.SyncItemInfo oldItem = oldItemIndex == -1 ? new Inventory.SyncItemInfo() { id = ItemType.None } : __instance.GetItemInHand();

            int j = 0;

            for (; j < __instance.items.Count; j++)
                if (__instance.items[j].uniq == i)
                    break;

            Inventory.SyncItemInfo newItem = i == - 1 ? new Inventory.SyncItemInfo() { id = ItemType.None } : __instance.items[j];

            Events.InvokeItemChanged(__instance.gameObject, ref oldItem, newItem);

            if (oldItemIndex != -1) __instance.items[oldItemIndex] = oldItem;

            return true;
        }
    }
}
