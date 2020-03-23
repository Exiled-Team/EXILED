using Harmony;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Generator079), nameof(Generator079.Interact))]
	public class GeneratorInsertedEvent
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
						if (!__instance.NetworkisDoorOpen)
							Events.InvokeGeneratorOpen(person, __instance, ref allow);
						else
							Events.InvokeGeneratorClose(person, __instance, ref allow);

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
			catch (Exception exception)
			{
				Log.Error($"GeneratorUnlockEvent/GeneratorOpenedEvent/GeneratorClosedEvent error: {exception}");
				return true;
			}
		}
	}

	[HarmonyPatch(typeof(Generator079), nameof(Generator079.CheckFinish))]
	public class GeneratorFinishedEvent
	{
		public static List<Generator079> FinishedGenerators = new List<Generator079>();
		public static bool Prefix(Generator079 __instance)
		{
			if (EventPlugin.GeneratorFinishedEventPatchDisable)
				return true;

			if (__instance.prevFinish || __instance.localTime > 0.0)
				return false;

			try
			{
				Events.InvokeGeneratorFinish(__instance);

				__instance.prevFinish = true;
				__instance.epsenRenderer.sharedMaterial = __instance.matLetGreen;
				__instance.epsdisRenderer.sharedMaterial = __instance.matLedBlack;
				__instance.asource.PlayOneShot(__instance.unlockSound);

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"GeneratorFinishedEvent error: {exception}");
				return true;
			}
		}
	}
}