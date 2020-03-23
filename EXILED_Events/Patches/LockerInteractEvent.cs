using Harmony;
using System;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerInteract), nameof(PlayerInteract.CallCmdUseLocker))]
	public class LockerInteractEvent
	{
		public static bool Prefix(PlayerInteract __instance, int lockerId, int chamberNumber)
		{
			if (EventPlugin.LockerInteractEventPatchDisable)
				return true;

			try
			{
				if (!__instance._playerInteractRateLimit.CanExecute(true) || (__instance._hc.CufferId > 0 && !__instance.CanDisarmedInteract))
					return false;

				LockerManager singleton = LockerManager.singleton;

				if (lockerId < 0 || lockerId >= singleton.lockers.Length)
					return false;

				if (!__instance.ChckDis(singleton.lockers[lockerId].gameObject.position) || !singleton.lockers[lockerId].supportsStandarizedAnimation)
					return false;

				if (chamberNumber < 0 || chamberNumber >= singleton.lockers[lockerId].chambers.Length)
					return false;

				if (singleton.lockers[lockerId].chambers[chamberNumber].doorAnimator == null)
					return false;

				if (!singleton.lockers[lockerId].chambers[chamberNumber].CooldownAtZero())
					return false;

				singleton.lockers[lockerId].chambers[chamberNumber].SetCooldown();

				string accessToken = singleton.lockers[lockerId].chambers[chamberNumber].accessToken;
				Item itemByID = __instance._inv.GetItemByID(__instance._inv.curItem);
				bool allow = string.IsNullOrEmpty(accessToken) || (itemByID != null && itemByID.permissions.Contains(accessToken)) || __instance._sr.BypassMode;

				Events.InvokeLockerInteract(__instance.gameObject, singleton.lockers[lockerId], lockerId, ref allow);

				if (allow)
				{
					bool flag = (singleton.openLockers[lockerId] & 1 << chamberNumber) != 1 << chamberNumber;
					singleton.ModifyOpen(lockerId, chamberNumber, flag);
					singleton.RpcDoSound(lockerId, chamberNumber, flag);
					bool state = true;
					for (int i = 0; i < singleton.lockers[lockerId].chambers.Length; i++)
					{
						if ((singleton.openLockers[lockerId] & 1 << i) == 1 << i)
						{
							state = false;
							break;
						}
					}
					singleton.lockers[lockerId].LockPickups(state);
					if (!string.IsNullOrEmpty(accessToken))
					{
						singleton.RpcChangeMaterial(lockerId, chamberNumber, false);
					}
				}
				else
				{
					singleton.RpcChangeMaterial(lockerId, chamberNumber, true);
				}

				__instance.OnInteract();

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"LockerInteractEvent error: {exception}");
				return true;
			}
		}
	}
}
