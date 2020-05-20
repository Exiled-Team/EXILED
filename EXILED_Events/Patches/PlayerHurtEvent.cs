using Harmony;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
	public class PlayerHurtEvent
	{
		public static List<string> DeathStuff = new List<string>();
		public static void Prefix(PlayerStats __instance, ref PlayerStats.HitInfo info, GameObject go)
		{
			if (EventPlugin.PlayerHurtPatchDisable)
				return;

			try
			{
				if (go == null) // As far as I remember, it's possible @iRebbok
					return;

				var goReferenceHub = ReferenceHub.Hubs[go];

				if (info.GetDamageType() == DamageTypes.Grenade)
					Events.InvokePlayerHurt(__instance, ref info, go, info.PlyId);
				else
					Events.InvokePlayerHurt(__instance, ref info, go);

				// GodMode players can't die
				if (goReferenceHub.characterClassManager.GodMode)
					return;

				bool isDied = Math.Floor(goReferenceHub.playerStats.health - info.Amount) <= 0f;

				if (info.GetDamageType() == DamageTypes.Pocket)
				{
					bool allow = true;

					Events.InvokePocketDimDamage(__instance.gameObject, ref allow);

					if (!allow)
						info.Amount = 0f;

					if (isDied)
						Events.InvokePocketDimDeath(__instance.gameObject, ref allow);

					if (!allow)
						info.Amount = 0f;
				}

				// don't need to check if it contains more damage
				if (isDied)
				{
					// I don't think that's possible
					// ReferenceHub can't be without CharterClassManager
					//if (goReferenceHub.characterClassManager != null)
					//{
					if (DeathStuff.Contains(goReferenceHub.characterClassManager.UserId))
						return;

					DeathStuff.Add(goReferenceHub.characterClassManager.UserId);
					//}

					if (info.GetDamageType() == DamageTypes.Grenade)
						Events.InvokePlayerDeath(__instance, ref info, go, info.PlyId);
					else
						Events.InvokePlayerDeath(__instance, ref info, go);
				}
			}
			catch (Exception exception)
			{
				Log.Error($"PocketDimDamageEvent/PocketDimDeathEvent/PlayerHurtEvent error: {exception}");
			}
		}
	}

	[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
	public class PlayerDeathFix
	{
		public static void Postfix(PlayerStats __instance, PlayerStats.HitInfo info, GameObject go)
		{
			CharacterClassManager ccm = go.GetComponent<CharacterClassManager>();

			if (ccm != null)
				if (PlayerHurtEvent.DeathStuff.Contains(ccm.UserId))
					PlayerHurtEvent.DeathStuff.Remove(ccm.UserId);
		}
	}
}
