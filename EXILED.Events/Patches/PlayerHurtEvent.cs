using System;
using EXILED.Shared.Helpers;
using Harmony;
using UnityEngine;

namespace EXILED.Events.Patches
{
	[HarmonyPatch(typeof(PlayerStats), "HurtPlayer")]
	public class PlayerHurtEvent
	{
		public static void Prefix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
		{
			if (EventPlugin.PlayerHurtPatchDisable)
				return;
			try
			{
				Events.Events.InvokePlayerHurt(__instance, ref info, go);
			}
			catch (Exception e)
			{
				LogHelper.Error($"Player hurt event error: {e}");
			}
		}

		public static void Postfix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
		{
			if (EventPlugin.PlayerHurtPatchDisable)
				return;

			try
			{
				if (go.GetComponent<PlayerStats>().health < 1 ||
				    go.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator)
					Events.Events.InvokePlayerDeath(__instance, ref info, go);
			}
			catch (Exception e)
			{
				LogHelper.Error($"Player death event error: {e}");
			}
		}
	}
}