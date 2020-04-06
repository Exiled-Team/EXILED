using Harmony;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.RpcPlaceBlood))]
	public class PlaceBloodEvent
	{
		public static bool Prefix(CharacterClassManager __instance, Vector3 pos, int type, float f)
		{
			try
			{
				bool allow = true;

				Events.InvokePlaceBlood(__instance.gameObject, ref pos, ref type, ref f, ref allow);

				return allow && !EventPlugin.RemoveBloodPlacement;
			}
			catch (Exception exception)
			{
				Log.Error($"PlaceBloodEvent error: {exception}");
				return true;
			}
		}
	}
}
