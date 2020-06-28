using Harmony;
using Mirror;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.AllowContain))]
	public class FemurEnterEvent
	{
		public static bool Prefix(CharacterClassManager __instance)
		{
			if (EventPlugin.FemurEnterEventDisable)
				return true;

			try
			{
				if (!NetworkServer.active || !NonFacilityCompatibility.currentSceneSettings.enableStandardGamplayItems)
					return false;

				foreach (ReferenceHub referenceHub in ReferenceHub.GetAllHubs().Values)
				{
					if (!referenceHub.isDedicatedServer && referenceHub.isReady && Vector3.Distance(referenceHub.transform.position, __instance._lureSpj.transform.position) < 1.97000002861023)
					{
						CharacterClassManager component1 = referenceHub.characterClassManager;
						PlayerStats component2 = referenceHub.playerStats;

						if (component1.CurRole.team != Team.SCP && component1.CurClass != RoleType.Spectator && !component1.GodMode)
						{
							bool allow = true;

							Events.InvokeFemurEnterEvent(component2.gameObject, ref allow);

							if (allow)
							{
								component2.HurtPlayer(new PlayerStats.HitInfo(10000f, "WORLD", DamageTypes.Lure, 0), referenceHub.gameObject);
								__instance._lureSpj.SetState(true);
							}
						}
					}
				}

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"FemurEnterEventEvent error: {exception}");
				return true;
			}
		}
	}
}
