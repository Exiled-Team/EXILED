using System;
using System.Collections.Generic;
using System.Linq;
using EXILED.Extensions;
using GameCore;
using Harmony;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(MTFRespawn), nameof(MTFRespawn.RespawnDeadPlayers))]
	public class TeamRespawnEvent
	{
		public static bool Prefix(MTFRespawn __instance)
		{
			if (EventPlugin.RespawnPatchDisable)
				return true;

			try
			{
				int num1 = 0;
				IEnumerable<GameObject> source = PlayerManager.players.Where(item => item.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator && !item.GetComponent<ServerRoles>().OverwatchEnabled);
				if (__instance.priorityMTFRespawn)
					source = source.OrderBy(item => item.GetComponent<CharacterClassManager>().DeathTime);
				int num2 = __instance.nextWaveIsCI ? __instance.maxCIRespawnAmount : __instance.maxMTFRespawnAmount;
				if (ConfigFile.ServerConfig.GetBool("respawn_tickets_enable", true))
				{
					if (__instance.NextWaveRespawnTickets == 0)
					{
						if (__instance.nextWaveIsCI)
						{
							__instance._ciDisabled = true;
							return false;
						}
						RoundSummary.singleton.ForceEnd();
						return false;
					}
					num2 = Mathf.Min(num2, __instance.NextWaveRespawnTickets);
				}
				List<GameObject> list = source.Take(num2).ToList();
				__instance.NextWaveRespawnTickets -= num2 - list.Count;

				if (Plugin.Config.GetBool("exiled_random_respawns"))
				{
					if (ConfigFile.ServerConfig.GetBool("use_crypto_rng"))
						list.ShuffleListSecure();
					else
						list.ShuffleList();
				}

				bool isChaos = __instance.nextWaveIsCI;
				int maxRespawn = isChaos ? __instance.maxCIRespawnAmount : __instance.maxMTFRespawnAmount;

				List<ReferenceHub> playersToRespawn = EventPlugin.DeadPlayers.Take(maxRespawn).ToList();
				Log.Debug($"Respawn: pre-vent list: {playersToRespawn.Count}");
				Events.InvokeTeamRespawn(ref isChaos, ref maxRespawn, ref playersToRespawn);

				__instance.playersToNTF.Clear();
				if (__instance.nextWaveIsCI && AlphaWarheadController.Host.detonated)
					__instance.nextWaveIsCI = false;
				foreach (ReferenceHub ply in playersToRespawn)
				{
					if (num1 >= maxRespawn)
						break;
					
					if (!(ply == null))
					{
						++num1;
						if (__instance.nextWaveIsCI)
						{
							__instance.GetComponent<CharacterClassManager>().SetPlayersClass(RoleType.ChaosInsurgency, ply.gameObject);
							ServerLogs.AddLog(ServerLogs.Modules.ClassChange, ply.GetNickname() + " (" + ply.GetUserId() + ") respawned as Chaos Insurgency agent.", ServerLogs.ServerLogType.GameEvent);
						}
						else
							__instance.playersToNTF.Add(ply.gameObject);
					}
				}
				if (num1 > 0)
				{
					ServerLogs.AddLog(ServerLogs.Modules.ClassChange, (__instance.nextWaveIsCI ? "Chaos Insurgency" : "MTF") + " respawned!", ServerLogs.ServerLogType.GameEvent);
					if (__instance.nextWaveIsCI)
						__instance.Invoke("CmdDelayCIAnnounc", 1f);
				}
				__instance.SummonNTF();
				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"RespawnEvent error: {exception}");
				return true;
			}
		}
	}
}