using Harmony;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdSwitchAWButton))]
	public class WarheadKeycardEvent
	{
		public static bool Prefix(PlayerInteract __instance)
		{
			if (EventPlugin.WarheadKeycardAccessEventDisable)
				return true;
			
			if (!__instance._playerInteractRateLimit.CanExecute(true) || (__instance._hc.CufferId > 0 && !__instance.CanDisarmedInteract))
				return false;

			GameObject gameObject = GameObject.Find("OutsitePanelScript");

			if (!__instance.ChckDis(gameObject.transform.position))
				return false;

			bool allow = true;
			string requiredPerms = "CONT_LVL_3";

			Events.InvokeWarheadKeycardAccess(__instance.gameObject, ref allow, ref requiredPerms);

			if (allow && __instance._inv.GetItemByID(__instance._inv.curItem).permissions.Contains(requiredPerms))
			{
				gameObject.GetComponentInParent<AlphaWarheadOutsitePanel>().NetworkkeycardEntered = true;
				__instance.OnInteract();
			}

			return false;
		}
	}
}