using System;
using Harmony;
using Mirror;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Generator079), "Interact")]
	public class Generator079Tablet
	{
		public static bool Prefix(Generator079 __instance, GameObject person, string command)
		{
			if (EventPlugin.Generator079EventPatchDisable)
				return true;
			
			try
			{
				if (command.StartsWith("EPS_TABLET"))
				{
					if (__instance.isTabletConnected || !__instance.isDoorOpen || __instance.localTime <= 0.0 ||
					    Generator079.mainGenerator.forcedOvercharge)
						return false;
					Inventory component = person.GetComponent<Inventory>();
					foreach (Inventory.SyncItemInfo syncItemInfo in component.items)
					{
						if (syncItemInfo.id == ItemType.WeaponManagerTablet)
						{
							bool allow = true;
							Events.InvokeGeneratorInsert(person, __instance, ref allow);
							if (!allow)
								return false;
							component.items.Remove(syncItemInfo);
							__instance.NetworkisTabletConnected = true;
							break;
						}
					}
				}
				else if (command.StartsWith("EPS_CANCEL"))
				{
					if (!__instance.isTabletConnected)
						return false;
					bool allow = true;
					Events.InvokeGeneratorEject(person, __instance, ref allow);
					if (allow)
						__instance.EjectTablet();
				}
				else if (command.StartsWith("EPS_DOOR"))
				{
					Inventory component = person.GetComponent<Inventory>();
					if (component == null || __instance.doorAnimationCooldown > 0.0 || __instance.deniedCooldown > 0.0)
						return false;
					if (!__instance.isDoorUnlocked)
					{
						bool allow = person.GetComponent<ServerRoles>().BypassMode;
						if (component.curItem > ItemType.KeycardJanitor)
						{
							foreach (string permission in component.GetItemByID(component.curItem).permissions)
							{
								if (permission == "ARMORY_LVL_2")
									allow = true;
							}
						}
						
						Events.InvokeGeneratorUnlock(person, __instance, ref allow);

						if (allow)
						{
							__instance.NetworkisDoorUnlocked = true;
							__instance.doorAnimationCooldown = 0.5f;
						}
						else
							__instance.RpcDenied();
					}
					else
					{
						bool allow = true;
						Events.InvokeGeneratorOpen(person, __instance, ref allow);
						if (!allow)
						{
							__instance.RpcDenied();
							return false;
						}

						__instance.doorAnimationCooldown = 1.5f;
						__instance.NetworkisDoorOpen = !__instance.isDoorOpen;
						__instance.RpcDoSound(__instance.isDoorOpen);
					}
				}

				return false;
			}
			catch (Exception e)
			{
				Plugin.Error($"Generator079 error: {e}");
				return true;
			}
		}
	}

	[HarmonyPatch(typeof(Generator079), "CheckFinish")]
	public class Generator079Finish
	{
		public static bool Prefix(Generator079 __instance)
		{
			if (__instance.prevFinish || __instance.localTime <= 0.0)
				Events.InvokeGeneratorFinish(__instance);
			return true;
		}
	}
}