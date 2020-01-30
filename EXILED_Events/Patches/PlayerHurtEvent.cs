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
					
					if (info.Amount >= __instance.health)
						Events.InvokePocketDimDeath(__instance.gameObject, ref allow);
					if (!allow)
						info.Amount = 0f;
				}

				Events.InvokePlayerHurt(__instance, ref info, go);
				
				if (info.Amount >= __instance.health)
					Events.InvokePlayerDeath(__instance, ref info, go);
			}
			catch (Exception e)
			{
				Plugin.Error($"Player hurt/death event error: {e}");
			}
		}
	}
}