using System;
using System.Collections.Generic;
using System.Linq;
using Harmony;
using UnityEngine;

namespace EXILED.Patches
{
  [HarmonyPatch(typeof(MTFRespawn), "RespawnDeadPlayers")]
  public class RespawnEvent
  {
    public static bool Prefix(MTFRespawn __instance)
    {
      if (EventPlugin.RespawnPatchDisable)
        return true;
      
      try
      {
        int num = 0;
        __instance.playersToNTF.Clear();
        List<GameObject> players = PlayerManager.players
          .Where(c => c.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator).ToList();
        Plugin.Debug($"Respawn: Got players: {players.Count}");
        foreach (GameObject player in players.ToArray())
          if (player.GetComponent<ServerRoles>().OverwatchEnabled)
          {
            Plugin.Debug($"Removing {ReferenceHub.GetHub(player)} -- Overwatch true");
            players.Remove(player);
          }

        players.ShuffleList();
        int r = EventPlugin.Gen.Next(100);
        int maxRespawn = 15;
        List<GameObject> toRespawn = players.Take(maxRespawn).ToList();
        bool isChaos = __instance.nextWaveIsCI;
        Plugin.Debug($"Respawn: pre-vent list: {toRespawn.Count}");
        Events.InvokeTeamRespawn(ref isChaos, ref maxRespawn, ref toRespawn);

        foreach (GameObject ply in toRespawn)
        {
          if (!(ply == null))
          {
            ++num;
            if (isChaos)
            {
              __instance.GetComponent<CharacterClassManager>()
                .SetPlayersClass(RoleType.ChaosInsurgency, ply);
              ServerLogs.AddLog(ServerLogs.Modules.ClassChange,
                ply.GetComponent<NicknameSync>().MyNick + " (" + ply.GetComponent<CharacterClassManager>().UserId +
                ") respawned as Chaos Insurgency agent.", ServerLogs.ServerLogType.GameEvent);
            }
            else
              __instance.playersToNTF.Add(ply);
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
      catch (Exception e)
      {
        Plugin.Error($"Respawn Event error: {e}");
        return true;
      }
    }
  }
}