using Harmony;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.SpawnRagdoll))]
	public class SpawnRagdollEvent
	{
		public static bool Prefix(RagdollManager __instance, ref Vector3 pos, ref Quaternion rot, ref int classId, ref PlayerStats.HitInfo ragdollInfo, ref bool allowRecall, ref string ownerID, ref string ownerNick, ref int playerId)
		{
			try
			{
				bool allow = true;

				Events.InvokeSpawnRagdoll(__instance.gameObject, ref pos, ref rot, ref classId, ref ragdollInfo, ref allowRecall, ref ownerID, ref ownerNick, ref playerId, ref allow);

				return allow;
			}
			catch (Exception exception)
			{
				Log.Error($"SpawnRagdollEvent error: {exception}");
				return true;
			}
		}
	}
}
