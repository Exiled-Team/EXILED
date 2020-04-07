using Harmony;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(WeaponManager), nameof(WeaponManager.RpcPlaceDecal))]
	public class PlaceDecalEvent
	{
		public static bool Prefix(WeaponManager __instance, bool isBlood, int type, Vector3 pos, Quaternion rot)
		{
			try
			{
				bool allow = true;
				float multiplier = 1;

				if (isBlood)
				{
					Events.InvokePlaceBlood(
						__instance.gameObject, 
						ref pos, 
						ref __instance.hub.characterClassManager.Classes.SafeGet(__instance.hub.characterClassManager.CurClass).bloodType, 
						ref multiplier, 
						ref allow
					);

					if (EventPlugin.RemoveBloodPlacement)
						return false;
				}
				else
					Events.InvokePlaceDecal(__instance.gameObject, ref pos, ref rot, ref type, ref allow);

				return allow;
			}
			catch (Exception exception)
			{
				Log.Error($"PlaceDecalEvent error: {exception}");
				return true;
			}
		}
	}
}
