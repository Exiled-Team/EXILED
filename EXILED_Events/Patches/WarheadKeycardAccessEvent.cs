using Harmony;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdSwitchAWButton))]
	public class WarheadKeycardAccessEvent
	{
		public static bool Prefix(PlayerInteract __instance)
		{
			if (EventPlugin.WarheadKeycardAccessEventDisable)
				return true;

			try
			{
				if (!__instance._playerInteractRateLimit.CanExecute(true) || (__instance._hc.CufferId > 0 && !__instance.CanDisarmedInteract))
					return false;

				GameObject gameObject = GameObject.Find("OutsitePanelScript");

				if (!__instance.ChckDis(gameObject.transform.position))
					return false;

				bool allow = true;
				string requiredPermission = "CONT_LVL_3";

				Events.InvokeWarheadKeycardAccess(__instance.gameObject, ref allow, ref requiredPermission);

				if (allow && __instance._inv.GetItemByID(__instance._inv.curItem).permissions.Contains(requiredPermission))
				{
					gameObject.GetComponentInParent<AlphaWarheadOutsitePanel>().NetworkkeycardEntered = true;
					__instance.OnInteract();
				}

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"WarheadKeycardAccessEvent error: {exception}");
				return true;
			}
		}
	}
}