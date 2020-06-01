using System;
using System.Linq;
using Harmony;
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
				if (!__instance._playerInteractRateLimit.CanExecute() || __instance._hc.CufferId > 0 && !__instance.CanDisarmedInteract || (doorId == null || __instance._ccm.CurClass == RoleType.None || (__instance._ccm.CurClass == RoleType.Spectator || !doorId.TryGetComponent<Door>(out door))) || (door.buttons.Count == 0 ? (__instance.ChckDis(doorId.transform.position) ? 1 : 0) : (door.buttons.Any(item => __instance.ChckDis(item.transform.position)) ? 1 : 0)) == 0)
					return false;

				__instance.OnInteract();
				if (__instance._sr.BypassMode)
					allow = true;
				else if (door.PermissionLevels.HasPermission(Door.AccessRequirements.Checkpoints) && __instance._ccm.CurRole.team == Team.SCP)
					allow = true;
				else
				{
					try
					{
						if (door.PermissionLevels == 0)
						{
							allow = !door.locked;
						}
						else if (!door.RequireAllPermissions)
						{
							var itemPerms = __instance._inv.GetItemByID(__instance._inv.curItem).permissions;
							// If the item’s expansion does not allow,
							// then set it to false, cuz the item does not have permission ¯\_(ツ)_/¯
							allow = itemPerms.Any(p =>
							door.backwardsCompatPermissions.TryGetValue(p, out var flag) &&
							door.PermissionLevels.HasPermission(flag)) || false;
						}
						else
							allow = false;
					}
					catch
					{
						allow = false;
					}

					Events.InvokeDoorInteract(__instance.gameObject, door, ref allow);

					if (allow)
						// Simply pass the force status as the presence of bypassMode on the player
						door.ChangeState(__instance._sr.BypassMode);
					else
						__instance.RpcDenied(doorId);
				}

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"DoorInteractionEvent error: {exception}");

				if (allow) 
					door.ChangeState(__instance._sr.BypassMode);
				else
					__instance.RpcDenied(doorId);

				return false;
			}
		}
	}
}