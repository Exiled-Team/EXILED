using GameCore;
using Harmony;
using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PocketDimensionTeleport), nameof(PocketDimensionTeleport.OnTriggerEnter))]
	public class PocketDimEscapedEvent
	{
		public static bool Prefix(PocketDimensionTeleport __instance, Collider other)
		{
			if (EventPlugin.PocketDimensionEscapedEventPatchDisabled)
				return true;

			try
			{
				PlayerStats component1 = other.GetComponent<PlayerStats>();
				if (component1 == null)
					return false;
				if (__instance.type == PocketDimensionTeleport.PDTeleportType.Killer ||
					Object.FindObjectOfType<BlastDoor>().isClosed)
					component1.HurtPlayer(new PlayerStats.HitInfo(999990f, "WORLD", DamageTypes.Pocket, 0), other.gameObject);
				else if (__instance.type == PocketDimensionTeleport.PDTeleportType.Exit)
				{
					__instance.tpPositions.Clear();
					List<string> stringList = ConfigFile.ServerConfig.GetStringList(
					  GameObject.Find("Host").GetComponent<DecontaminationLCZ>().GetCurAnnouncement() > 5
						? "pd_random_exit_rids_after_decontamination"
						: "pd_random_exit_rids");
					if (stringList.Count > 0)
					{
						foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("RoomID"))
						{
							if (gameObject.GetComponent<Rid>() != null && stringList.Contains(gameObject.GetComponent<Rid>().id))
								__instance.tpPositions.Add(gameObject.transform.position);
						}

						if (stringList.Contains("PORTAL"))
						{
							foreach (Scp106PlayerScript scp106PlayerScript in Object.FindObjectsOfType<Scp106PlayerScript>())
							{
								if (scp106PlayerScript.portalPosition != Vector3.zero)
									__instance.tpPositions.Add(scp106PlayerScript.portalPosition);
							}
						}
					}

					if (__instance.tpPositions == null || __instance.tpPositions.Count == 0)
					{
						foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("PD_EXIT"))
							__instance.tpPositions.Add(gameObject.transform.position);
					}

					Vector3 tpPosition = __instance.tpPositions[Random.Range(0, __instance.tpPositions.Count)];
					tpPosition.y += 2f;
					PlyMovementSync component2 = other.GetComponent<PlyMovementSync>();
					if (component2 == null)
						return false;
					component2.SetSafeTime(2f);

					bool allowEscape = true;
					Events.InvokePocketDimEscaped(component2.gameObject, ref allowEscape);

					if (allowEscape)
					{
						component2.OverridePosition(tpPosition, 0.0f, false);
						__instance.RemoveCorrosionEffect(other.gameObject);
						PlayerManager.localPlayer.GetComponent<PlayerStats>()
						  .TargetAchieve(component1.connectionToClient, "larryisyourfriend");
					}
				}

				if (!__instance.RefreshExit)
					return false;
				ImageGenerator.pocketDimensionGenerator.GenerateRandom();
				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"PocketDimEscapedEvent error {exception}");
				return true;
			}
		}
	}
}
