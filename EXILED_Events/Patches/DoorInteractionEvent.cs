using System;
using System.Linq;
using Harmony;
using UnityEngine;

namespace EXILED.Patches
{
	public class DoorInteractionEvent
	{
		[HarmonyPatch(typeof(PlayerInteract), "CallCmdOpenDoor")]
		public static bool Prefix(PlayerInteract __instance, GameObject doorId)
    {
      bool allow = true;
			if (!__instance._playerInteractRateLimit.CanExecute() || __instance._hc.CufferId > 0 && !__instance.CanDisarmedInteract || (doorId == null || __instance._ccm.CurClass == RoleType.None || __instance._ccm.CurClass == RoleType.Spectator))
        return false;
      Door component1 = doorId.GetComponent<Door>();
      if (component1 == null || (component1.buttons.Count == 0 ? (__instance.ChckDis(doorId.transform.position) ? 1 : 0) : (component1.buttons.Any(item => __instance.ChckDis(item.transform.position)) ? 1 : 0)) == 0)
        return false;
      Scp096PlayerScript component2 = __instance.GetComponent<Scp096PlayerScript>();
      if (component1.destroyedPrefab != null && (!component1.isOpen || component1.curCooldown > 0.0) && (component2.iAm096 && component2.enraged == Scp096PlayerScript.RageState.Enraged))
      {
        if (!__instance._096DestroyLockedDoors && component1.locked && !__instance._sr.BypassMode)
          return false;
        component1.DestroyDoor(true);
      }
      else
      {
        __instance.OnInteract();
        if (__instance._sr.BypassMode)
        {
          Events.InvokeDoorInteract(__instance.gameObject, component1, ref allow);
          if (allow == false)
            return false;
          component1.ChangeState(true);
        }
        else if (string.Equals(component1.permissionLevel, "CHCKPOINT_ACC", StringComparison.OrdinalIgnoreCase) && __instance.GetComponent<CharacterClassManager>().Classes.SafeGet(__instance.GetComponent<CharacterClassManager>().CurClass).team == Team.SCP)
        {
          Events.InvokeDoorInteract(__instance.gameObject, component1, ref allow);
          if (allow == false)
            return false;
          component1.ChangeState();
        }
        else
        {
          try
          {
            if (string.IsNullOrEmpty(component1.permissionLevel))
            {
              if (component1.locked)
                return false;
              
              Events.InvokeDoorInteract(__instance.gameObject, component1, ref allow);
              if (allow == false)
                return false;
              component1.ChangeState();
            }
            else if (__instance._inv.GetItemByID(__instance._inv.curItem).permissions.Contains(component1.permissionLevel))
            {
              if (!component1.locked)
              {
                Events.InvokeDoorInteract(__instance.gameObject, component1, ref allow);
                if (allow == false)
                  return false;
                component1.ChangeState();
              }
              else
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
      }

      return false;
    }
	}
}