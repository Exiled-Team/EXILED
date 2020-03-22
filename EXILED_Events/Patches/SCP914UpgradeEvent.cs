using Harmony;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(Scp914.Scp914Machine), nameof(Scp914.Scp914Machine.ProcessItems))]
	public class SCP914UpgradeEvent
	{
		public static bool Prefix(Scp914.Scp914Machine __instance)
		{
			if (EventPlugin.Scp914UpgradeEventPatchDisable)
				return true;

			try
			{
				if (!NetworkServer.active)
					return true;
				Collider[] colliderArray = Physics.OverlapBox(__instance.intake.position, __instance.inputSize / 2f);
				__instance.players.Clear();
				__instance.items.Clear();
				foreach (Collider collider in colliderArray)
				{
					CharacterClassManager component1 = collider.GetComponent<CharacterClassManager>();
					if (component1 != null)
					{
						__instance.players.Add(component1);
					}
					else
					{
						Pickup component2 = collider.GetComponent<Pickup>();
						if (component2 != null)
							__instance.items.Add(component2);
					}
				}

				List<Pickup> pickups = __instance.items;
				bool allowUpgrade = true;

				Events.InvokeScp914Upgrade(__instance, __instance.players, ref pickups, __instance.knobState, ref allowUpgrade);

				__instance.MoveObjects(pickups, __instance.players);

				if (allowUpgrade)
					__instance.UpgradeObjects(pickups, __instance.players);

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"SCP914UpgradeEvent error: {exception}");
				return true;
			}
		}
	}
}
