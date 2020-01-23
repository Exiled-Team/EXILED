using System;
using Harmony;
using Mirror;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Searching), "CallCmdPickupItem")]
	public class PickupItemEvent
	{
    public static bool Prefix(Searching __instance, GameObject t)
    {
      try
      {
        if (!__instance._playerInteractRateLimit.CanExecute(true) || t == null ||
            (!__instance.hub.characterClassManager.IsHuman() ||
             Vector3.Distance(__instance.GetComponent<PlyMovementSync>().RealModelPosition, t.transform.position) >
             3.5))
          return false;
        Pickup component = t.GetComponent<Pickup>();
        if (component == null || !__instance._pickupInProgressServer ||
            (t != __instance._pickupObjectServer || __instance._pickupProgressServer > 0.25) || component.info.locked)
          return false;
        Item itemById1 = __instance.hub.inventory.GetItemByID(component.info.itemId);
        if (itemById1.noEquipable)
        {
          for (int type = 0; type < __instance.hub.ammoBox.types.Length; ++type)
          {
            if (__instance.hub.ammoBox.types[type].inventoryID == component.info.itemId)
            {
              int ammo = __instance.hub.ammoBox.GetAmmo(type);
              int num = __instance.hub.characterClassManager.Classes
                .SafeGet(__instance.hub.characterClassManager.CurClass).maxAmmo[type];
              int durability;
              for (durability = (int) component.info.durability; ammo < num && durability > 0; ++ammo)
                --durability;
              Pickup.PickupInfo info = component.info;
              info.durability = durability;
              component.Networkinfo = info;
              if (durability <= 0)
                component.Delete();
              __instance.hub.ammoBox.SetOneAmount(type, $"{ammo}");
            }
          }
        }
        else
        {
          ItemCategory itemCategory = itemById1.itemCategory;
          switch (itemCategory)
          {
            case ItemCategory.None:
            case ItemCategory.NoCategory:
              ItemType itemId = component.info.itemId;
              component.Delete();
              if (itemId == ItemType.None)
                break;
              bool allow = true;
              Events.InvokePickupItem(__instance.gameObject, ref component.info, ref allow);
              if (allow == false)
                return false;
              __instance.AddItem(itemId,
                (Object) t.GetComponent<Pickup>() == (Object) null ? -1f : component.info.durability,
                component.info.weaponMods);
              break;
            default:
              int num = 0;
              foreach (Inventory.SyncItemInfo syncItemInfo in __instance.hub.inventory.items)
              {
                Item itemById2 = __instance.hub.inventory.GetItemByID(syncItemInfo.id);
                if ((itemById2 != null ? (itemById2.itemCategory == itemCategory ? 1 : 0) : 0) != 0)
                  ++num;
              }

              foreach (InventoryCategory category in __instance.categories)
              {
                if (category.itemType == itemCategory)
                {
                  if (num >= category.maxItems)
                    return false;
                  if (num == category.maxItems - 1 && !category.hideWarning)
                    __instance.TargetShowWarning(__instance.connectionToClient, category.label, category.maxItems);
                }
              }

              goto case ItemCategory.None;
          }
        }

        return false;
      }
      catch (Exception e)
      {
        Plugin.Error($"Item Pickup error: {e}");
        return true;
      }
    }
  }
}