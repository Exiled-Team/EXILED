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
				if (info.GetDamageType() == DamageTypes.Pocket)
				{
					bool allow = true;
					Events.InvokePocketDimDamage(__instance.gameObject, ref allow);
					if (!allow)
						info.Amount = 0f;

					if (info.Amount >= go.GetComponent<PlayerStats>().health)
						Events.InvokePocketDimDeath(__instance.gameObject, ref allow);
					if (!allow)
						info.Amount = 0f;
				}

				if (info.GetDamageType() == DamageTypes.Grenade)
					Events.InvokePlayerHurt(__instance, ref info, go, info.PlyId);
				else
					Events.InvokePlayerHurt(__instance, ref info, go);

				if (info.Amount >= go.GetComponent<PlayerStats>().health || (go.GetComponent<PlayerStats>().health - info.Amount) <= 1f)
				{
					CharacterClassManager ccm = go.GetComponent<CharacterClassManager>();
					if (ccm != null)
					{
						if (DeathStuff.Contains(ccm.UserId))
							return;
						DeathStuff.Add(ccm.UserId);
					}

					if (info.GetDamageType() == DamageTypes.Grenade)
						Events.InvokePlayerDeath(__instance, ref info, go, info.PlyId);
					else
						Events.InvokePlayerDeath(__instance, ref info, go);
				}
			}
			catch (Exception e)
			{
				Log.Error($"Player hurt/death event error: {e}");
			}
		}
	}

	[HarmonyPatch(typeof(PlayerStats), nameof(PlayerStats.HurtPlayer))]
	public class DeathFix
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
