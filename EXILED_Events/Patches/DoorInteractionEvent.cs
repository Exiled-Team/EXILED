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
					door.ChangeState(true);
				else if (door.PermissionLevels.HasPermission(Door.AccessRequirements.Checkpoints) && __instance._ccm.CurRole.team == Team.SCP)
				{
					door.ChangeState();
				}
				else
				{
					try
					{
						if (door.PermissionLevels == 0)
						{
							if (door.locked)
								return false;
							Events.InvokeDoorInteract(__instance.gameObject, door, ref allow);
							
							if (allow)
								door.ChangeState();
							else
								__instance.RpcDenied(doorId);
						}
						else if (!door.RequireAllPermissions)
						{
							foreach (string permission in __instance._inv.GetItemByID(__instance._inv.curItem).permissions)
							{
								Door.AccessRequirements flag;
								if (door.backwardsCompatPermissions.TryGetValue(permission, out flag) && door.PermissionLevels.HasPermission(flag))
								{
									if (door.locked)
										return false;
									
									Events.InvokeDoorInteract(__instance.gameObject, door, ref allow);
							
									if (allow)
										door.ChangeState();
									else
										__instance.RpcDenied(doorId);
									door.ChangeState();
									return false;
								}
							}
							__instance.RpcDenied(doorId);
						}
						else
							__instance.RpcDenied(doorId);
					}
					catch
					{
						__instance.RpcDenied(doorId);
					}
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