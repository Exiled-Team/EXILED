using Harmony;
using System;
using System.Linq;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdOpenDoor), typeof(GameObject))]
	public class DoorInteractionEvent
	{
		public static bool Prefix(PlayerInteract __instance, GameObject doorId)
		{
			if (EventPlugin.DoorInteractionEventPatchDisable)
				return true;

			bool allow = true;
			Door door = doorId.GetComponent<Door>();

			try
			{
				if (!__instance._playerInteractRateLimit.CanExecute() ||
					__instance._hc.CufferId > 0 && !__instance.CanDisarmedInteract ||
					(doorId == null || __instance._ccm.CurClass == RoleType.None ||
					 __instance._ccm.CurClass == RoleType.Spectator))
					return false;

				if (door == null || (door.buttons.Count == 0
						? (__instance.ChckDis(doorId.transform.position) ? 1 : 0)
						: (door.buttons.Any(item => __instance.ChckDis(item.transform.position)) ? 1 : 0)) == 0)
					return false;

				Scp096PlayerScript component2 = __instance.GetComponent<Scp096PlayerScript>();

				if (door.destroyedPrefab != null && (!door.isOpen || door.curCooldown > 0.0) &&
					(component2.iAm096 && component2.enraged == Scp096PlayerScript.RageState.Enraged))
				{
					if (!__instance._096DestroyLockedDoors && door.locked && !__instance._sr.BypassMode)
						return false;

					door.DestroyDoor(true);
				}
				else
				{
					__instance.OnInteract();
					if (__instance._sr.BypassMode)
					{
						allow = true;
					}
					else if (string.Equals(door.permissionLevel, "CHCKPOINT_ACC", StringComparison.OrdinalIgnoreCase) &&
						__instance.GetComponent<CharacterClassManager>().Classes.SafeGet(__instance.GetComponent<CharacterClassManager>().CurClass).team == Team.SCP)
					{
						allow = true;
					}
					else
					{
						Item item = __instance._inv.GetItemByID(__instance._inv.curItem);
						if (string.IsNullOrEmpty(door.permissionLevel))
						{
							allow = !door.locked;
						}
						else if (item != null && item.permissions.Contains(door.permissionLevel))
						{
							allow = !door.locked;
						}
						else
							allow = false;
					}

					Events.InvokeDoorInteract(__instance.gameObject, door, ref allow);

					if (!allow)
					{
						__instance.RpcDenied(doorId);
						return false;
					}

					door.ChangeState();
				}

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"DoorInteractionEvent error: {exception}");

				if (allow) door.ChangeState();
				else __instance.RpcDenied(doorId);

				return false;
			}
		}
	}
}