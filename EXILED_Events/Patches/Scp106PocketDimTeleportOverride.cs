using GameCore;
using Harmony;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EXILED.Patches
{
    [HarmonyPatch(typeof(PocketDimensionTeleport), "OnTriggerEnter")]
    public class Scp106PocketDimTeleportOverride
    {
        public static void Prefix(PocketDimensionTeleport __instance, Collider other)
        {
			if (!NetworkServer.active)
				return;
			NetworkIdentity component = other.GetComponent<NetworkIdentity>();
			if (component != null)
			{
				if (__instance.type == PocketDimensionTeleport.PDTeleportType.Killer || UnityEngine.Object.FindObjectOfType<BlastDoor>().isClosed)
				{
					bool AllowDeath = true;

					Events.InvokePocketDimDeath(__instance.gameObject, ref AllowDeath);

					if (!AllowDeath)
						return;

					component.GetComponent<PlayerStats>().HurtPlayer(new PlayerStats.HitInfo(999990f, "WORLD", DamageTypes.Pocket, 0), other.gameObject);
				}
				else if (__instance.type == PocketDimensionTeleport.PDTeleportType.Exit)
				{
					__instance.tpPositions.Clear();
					List<string> stringList = ConfigFile.ServerConfig.GetStringList((GameObject.Find("Host").GetComponent<DecontaminationLCZ>().GetCurAnnouncement() > 5) ? "pd_random_exit_rids_after_decontamination" : "pd_random_exit_rids");
					if (stringList.Count > 0)
					{
						foreach (GameObject gameObject in GameObject.FindGameObjectsWithTag("RoomID"))
							if (gameObject.GetComponent<Rid>() != null && stringList.Contains(gameObject.GetComponent<Rid>().id))
								__instance.tpPositions.Add(gameObject.transform.position);
						if (stringList.Contains("PORTAL"))
							foreach (Scp106PlayerScript scp106PlayerScript in UnityEngine.Object.FindObjectsOfType<Scp106PlayerScript>())
								if (scp106PlayerScript.portalPosition != Vector3.zero)
									__instance.tpPositions.Add(scp106PlayerScript.portalPosition);
					}
					if (__instance.tpPositions == null || __instance.tpPositions.Count == 0)
						foreach (GameObject gameObject2 in GameObject.FindGameObjectsWithTag("PD_EXIT"))
							__instance.tpPositions.Add(gameObject2.transform.position);

					bool AllowEscape = true;

					Events.InvokePocketDimEscaped(__instance.gameObject, ref AllowEscape);

					if (!AllowEscape)
						return;

					Vector3 pos = __instance.tpPositions[UnityEngine.Random.Range(0, __instance.tpPositions.Count)];
					pos.y += 2f;
					PlyMovementSync component2 = other.GetComponent<PlyMovementSync>();
					component2.SetSafeTime(2f);
					component2.OverridePosition(pos, 0f, false);
					__instance.RemoveCorrosionEffect(other.gameObject);
					PlayerManager.localPlayer.GetComponent<PlayerStats>().TargetAchieve(component.connectionToClient, "larryisyourfriend");
				}
				if (__instance.RefreshExit)
					ImageGenerator.pocketDimensionGenerator.GenerateRandom();
			}
		}
    }
}
