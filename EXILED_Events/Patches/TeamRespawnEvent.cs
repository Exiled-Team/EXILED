using EXILED.Extensions;
using Harmony;
using System;
using System.Collections.Generic;
using System.Linq;

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
				int num = 0;
				__instance.playersToNTF.Clear();

				Log.Debug($"Respawn: Got players: {EventPlugin.DeadPlayers.Count}");

				foreach (ReferenceHub player in EventPlugin.DeadPlayers.ToArray())
				{
					if (player.GetOverwatch() || player.GetRole() != RoleType.Spectator)
					{
						Log.Debug($"Removing {player.GetNickname()} -- Overwatch true or not spectator");
						EventPlugin.DeadPlayers.Remove(player);
					}
				}

				if (Plugin.Config.GetBool("exiled_random_respawns"))
					EventPlugin.DeadPlayers.ShuffleList();

				bool isChaos = __instance.nextWaveIsCI;
				int maxRespawn = isChaos ? __instance.maxCIRespawnAmount : __instance.maxMTFRespawnAmount;

				List<ReferenceHub> playersToRespawn = EventPlugin.DeadPlayers.Take(maxRespawn).ToList();
				Log.Debug($"Respawn: pre-vent list: {playersToRespawn.Count}");
				Events.InvokeTeamRespawn(ref isChaos, ref maxRespawn, ref playersToRespawn);

				if (maxRespawn <= 0 || playersToRespawn == null || playersToRespawn.Count == 0)
					return false;

				foreach (ReferenceHub player in playersToRespawn)
				{
					if (num >= maxRespawn)
						break;

					if (player != null)
					{
						++num;
						if (isChaos)
						{
							__instance.GetComponent<CharacterClassManager>().SetPlayersClass(RoleType.ChaosInsurgency, player.gameObject);
							ServerLogs.AddLog(ServerLogs.Modules.ClassChange, player.GetNickname() + " (" + player.GetUserId() +
							  ") respawned as Chaos Insurgency agent.", ServerLogs.ServerLogType.GameEvent);
						}
						else
							__instance.playersToNTF.Add(player.gameObject);

						EventPlugin.DeadPlayers.Remove(player);
					}
				}

				if (num > 0)
				{
					ServerLogs.AddLog(ServerLogs.Modules.ClassChange,
					  (__instance.nextWaveIsCI ? "Chaos Insurgency" : "MTF") + " respawned!", ServerLogs.ServerLogType.GameEvent);
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