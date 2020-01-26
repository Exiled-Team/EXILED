using System;
using Harmony;
using UnityEngine;

namespace EXILED.Patches
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
				if (info.GetDamageType() == DamageTypes.Pocket)
				{
					bool allow = true;
					Events.InvokePocketDimDamage(__instance.gameObject, ref allow);
					if (!allow)
						info.Amount = 0f;
				}

				Events.InvokePlayerHurt(__instance, ref info, go);
			}
			catch (Exception e)
			{
				Plugin.Error($"Player hurt event error: {e}");
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
				{
					if (info.GetDamageType() == DamageTypes.Pocket)
					{
						bool allow = true;
						Events.InvokePocketDimDeath(__instance.gameObject, ref allow);
						if (!allow)
							info.Amount = 0;
					}
					Events.InvokePlayerDeath(__instance, ref info, go);
				}
			}
			catch (Exception e)
			{
				Plugin.Error($"Player death event error: {e}");
			}
		}
	}
}