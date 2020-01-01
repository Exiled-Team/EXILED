using EXILED;
using Harmony;
using UnityEngine;

namespace JokersPlugin.Patches
{
	[HarmonyPatch(typeof(PlayerStats), "HurtPlayer")]
	public class PlayerHurtEvent
	{
		public static void Prefix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
		{
			if (EXILED.plugin.PlayerHurtPatchDisable)
				return;
			Events.InvokePlayerHurt(__instance, ref info, go);
		}

		public static void Postfix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
		{
			if (EXILED.plugin.PlayerHurtPatchDisable)
				return;
			
			if (go.GetComponent<PlayerStats>().health < 1 || go.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator)
				Events.InvokePlayerDeath(__instance, ref info, go);
		}
	}
}