using System;
using System.Collections.Generic;
using System.Linq;
using GameCore;
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
        IEnumerable<GameObject> source = PlayerManager.players.Where(item => item.GetComponent<CharacterClassManager>().CurClass == RoleType.Spectator && !item.GetComponent<ServerRoles>().OverwatchEnabled);
        if (__instance.priorityMTFRespawn)
          source = source.OrderBy(item => item.GetComponent<CharacterClassManager>().DeathTime);
        List<GameObject> list = source.Take(__instance.nextWaveIsCI ? __instance.maxCIRespawnAmount : __instance.maxMTFRespawnAmount).ToList();
        if (ConfigFile.ServerConfig.GetBool("use_crypto_rng"))
          list.ShuffleListSecure();
        else
          list.ShuffleList();
        __instance.playersToNTF.Clear();
        if (__instance.nextWaveIsCI && AlphaWarheadController.Host.detonated)
          __instance.nextWaveIsCI = false;
        
        int maxRespawn = 15;
        List<GameObject> toRespawn = list;
        bool isChaos = __instance.nextWaveIsCI;
        Events.InvokeTeamRespawn(ref isChaos, ref maxRespawn, ref toRespawn);

        toRespawn = toRespawn.Take(maxRespawn).ToList();
        
        foreach (GameObject ply in toRespawn)
        {
          if (!(ply == null))
          {
            ++num;
            if (__instance.nextWaveIsCI)
            {
              __instance.GetComponent<CharacterClassManager>().SetPlayersClass(RoleType.ChaosInsurgency, ply);
              ServerLogs.AddLog(ServerLogs.Modules.ClassChange, ply.GetComponent<NicknameSync>().MyNick + " (" + ply.GetComponent<CharacterClassManager>().UserId + ") respawned as Chaos Insurgency agent.", ServerLogs.ServerLogType.GameEvent);
            }
            else
              __instance.playersToNTF.Add(ply);
          }
        }
        if (num > 0)
        {
          ServerLogs.AddLog(ServerLogs.Modules.ClassChange, (__instance.nextWaveIsCI ? "Chaos Insurgency" : "MTF") + " respawned!", ServerLogs.ServerLogType.GameEvent);
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