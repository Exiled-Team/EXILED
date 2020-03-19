using Harmony;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.SetPlayersClass))]
	public class StartItemsEvent
	{
		public static bool Prefix(CharacterClassManager __instance, RoleType classid, GameObject ply, bool lite = false, bool escape = false)
		{
			if (EventPlugin.StartItemsEventPatchDisable)
				return true;

			try
			{
				if (!NetworkServer.active)
				{
					return false;
				}
				if (!ply.GetComponent<CharacterClassManager>().IsVerified)
				{
					return false;
				}
				List<ItemType> startItems = lite ? new List<ItemType>(0) : __instance.Classes.SafeGet(classid).startItems.ToList();
				Events.InvokeStartItems(ply, classid, ref startItems);
				ply.GetComponent<CharacterClassManager>().SetClassIDAdv(classid, lite, escape);
				ply.GetComponent<PlayerStats>().SetHPAmount(__instance.Classes.SafeGet(classid).maxHP);
				if (lite)
				{
					return false;
				}
				Inventory component = ply.GetComponent<Inventory>();
				List<Inventory.SyncItemInfo> list = new List<Inventory.SyncItemInfo>();
				if (escape && __instance.KeepItemsAfterEscaping)
				{
					foreach (Inventory.SyncItemInfo item in component.items)
					{
						list.Add(item);
					}
				}
				component.items.Clear();
				foreach (ItemType id in startItems)
				{
					component.AddNewItem(id, -4.65664672E+11f, 0, 0, 0);
				}
				if (escape && __instance.KeepItemsAfterEscaping)
				{
					foreach (Inventory.SyncItemInfo syncItemInfo in list)
					{
						if (__instance.PutItemsInInvAfterEscaping)
						{
							Item itemByID = component.GetItemByID(syncItemInfo.id);
							bool flag = false;
							InventoryCategory[] categories = __instance._searching.categories;
							int i = 0;
							while (i < categories.Length)
							{
								InventoryCategory inventoryCategory = categories[i];
								if (inventoryCategory.itemType == itemByID.itemCategory && (itemByID.itemCategory != ItemCategory.None || itemByID.itemCategory != ItemCategory.NoCategory))
								{
									int num = 0;
									foreach (Inventory.SyncItemInfo syncItemInfo2 in component.items)
									{
										if (component.GetItemByID(syncItemInfo2.id).itemCategory == itemByID.itemCategory)
										{
											num++;
										}
									}
									if (num >= inventoryCategory.maxItems)
									{
										flag = true;
										break;
									}
									break;
								}
								else
								{
									i++;
								}
							}
							if (component.items.Count >= 8 || flag)
							{
								component.SetPickup(syncItemInfo.id, syncItemInfo.durability, __instance._pms.RealModelPosition, Quaternion.Euler(__instance._pms.Rotations.x, __instance._pms.Rotations.y, 0f), syncItemInfo.modSight, syncItemInfo.modBarrel, syncItemInfo.modOther);
							}
							else
							{
								component.AddNewItem(syncItemInfo.id, syncItemInfo.durability, syncItemInfo.modSight, syncItemInfo.modBarrel, syncItemInfo.modOther);
							}
						}
						else
						{
							component.SetPickup(syncItemInfo.id, syncItemInfo.durability, __instance._pms.RealModelPosition, Quaternion.Euler(__instance._pms.Rotations.x, __instance._pms.Rotations.y, 0f), syncItemInfo.modSight, syncItemInfo.modBarrel, syncItemInfo.modOther);
						}
					}
				}
				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"StartItemsEvent error: {exception}");
				return true;
			}
		}
	}
}
