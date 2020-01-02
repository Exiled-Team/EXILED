using System;
using EXILED;
using Harmony;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerStats), "HurtPlayer")]
	public class PlayerHurtEvent
	{
		public static void Prefix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
		{
			if (EXILED.plugin.PlayerHurtPatchDisable)
				return;
			try
			{
				Events.InvokePlayerHurt(__instance, ref info, go);
			}
			catch (Exception e)
			{
				Plugin.Error($"Player hurt event error: {e}");
			}
		}

		public static void Postfix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
		{
			if (EXILED.plugin.PlayerHurtPatchDisable)
				return;

			try
			{
				if (go.GetComponent<PlayerStats>().health < 1 ||
				    go.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator)
					Events.InvokePlayerDeath(__instance, ref info, go);
			}
			catch (Exception e)
			{
				Plugin.Error($"Player death event error: {e}");
			}
		}
	}
}