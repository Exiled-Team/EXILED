using System;
using System.Collections.Generic;
using CustomPlayerEffects;
using EXILED.Extensions;
using Harmony;
using Mirror;
using PlayableScps;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerPositionManager), nameof(PlayerPositionManager.TransmitData))]
	public class GhostmodePatch
	{
		public static bool Prefix(PlayerPositionManager __instance)
		{
			if (EventPlugin.GhostmodePatchDisable)
				return true;

			try
			{ 
          ++__instance.frame;
          if (__instance.frame != __instance.syncFrequency)
             return false;
          __instance.frame = 0;
          List<GameObject> players = PlayerManager.players;
          __instance.usedData = players.Count;
          if (__instance.receivedData == null || __instance.receivedData.Length < __instance.usedData)
            __instance.receivedData = new PlayerPositionData[__instance.usedData * 2];
          for (int index = 0; index < __instance.usedData; ++index)
            __instance.receivedData[index] = new PlayerPositionData(ReferenceHub.GetHub(players[index]));
          if (__instance.transmitBuffer == null || __instance.transmitBuffer.Length < __instance.usedData)
            __instance.transmitBuffer = new PlayerPositionData[__instance.usedData * 2];
          foreach (GameObject player in players)
          {
            ReferenceHub hub1 = ReferenceHub.GetHub(player);
            Array.Copy(__instance.receivedData, __instance.transmitBuffer, __instance.usedData);
            if (hub1.characterClassManager.CurClass.Is939())
            {
              for (int index = 0; index < __instance.usedData; ++index)
              {
                if (__instance.transmitBuffer[index].position.y < 800.0)
                {
                  ReferenceHub hub2 = ReferenceHub.GetHub(players[index]);
                  if (EventPlugin.GhostedIds.Contains(__instance.transmitBuffer[index].playerID) || hub2.characterClassManager.CurRole.team != Team.SCP && hub2.characterClassManager.CurRole.team != Team.RIP && !players[index].GetComponent<Scp939_VisionController>().CanSee(hub1.characterClassManager.Scp939))
                    __instance.transmitBuffer[index] = new PlayerPositionData(Vector3.up * 6000f, 0.0f, __instance.transmitBuffer[index].playerID);
                }
              }
            }
            else if (hub1.characterClassManager.CurClass != RoleType.Scp079 && hub1.characterClassManager.CurClass != RoleType.Spectator)
            {
              for (int index = 0; index < __instance.usedData; ++index)
              {
                ReferenceHub hub2;
                if (ReferenceHub.TryGetHub(__instance.transmitBuffer[index].playerID, out hub2))
                {
                  if (EventPlugin.GhostedIds.Contains(__instance.transmitBuffer[index].playerID) || hub1.scpsController.CurrentScp is Scp096 currentScp && currentScp.Enraged && (!currentScp.HasTarget(hub2) && hub2.characterClassManager.CurRole.team != Team.SCP))
                    __instance.transmitBuffer[index] = new PlayerPositionData(Vector3.up * 6000f, 0.0f, __instance.transmitBuffer[index].playerID);
               
                  if (hub2.playerEffectsController.GetEffect<Scp268>().Enabled)
                  {
                    bool flag = false;
                    if (hub1.scpsController.CurrentScp is Scp096 curScp && curScp != null)
                      flag = curScp.HasTarget(hub2);
                    if (hub1.characterClassManager.CurClass != RoleType.Scp079 && hub1.characterClassManager.CurClass != RoleType.Spectator && !flag)
                      __instance.transmitBuffer[index] = new PlayerPositionData(Vector3.up * 6000f, 0.0f, __instance.transmitBuffer[index].playerID);
                  }
                }
              }
            }
         
            if (EventPlugin.TargetGhost.ContainsKey(player.GetPlayer()))
            {
              for (int i = 0; i < __instance.usedData; i++)
              {
                if (EventPlugin.TargetGhost[player.GetPlayer()]
                  .Contains(__instance.transmitBuffer[i].playerID))
                  __instance.transmitBuffer[i] = new PlayerPositionData(Vector3.up * 6000f, 0.0f, __instance.transmitBuffer[i].playerID);
              }
            }
         
            NetworkConnection networkConnection = hub1.characterClassManager.netIdentity.isLocalPlayer ? NetworkServer.localConnection : hub1.characterClassManager.netIdentity.connectionToClient;
            if (__instance.usedData <= 20)
            {
              networkConnection.Send(new PlayerPositionManager.PositionMessage(__instance.transmitBuffer, (byte) __instance.usedData, 0), 1);
            }
            else
            {
              byte part;
              for (part = (byte) 0; (int) part < __instance.usedData / 20; ++part)
                networkConnection.Send(new PlayerPositionManager.PositionMessage(__instance.transmitBuffer, 20, part), 1);
              byte count = (byte) (__instance.usedData % (part * 20));
              if (count > 0)
                networkConnection.Send(new PlayerPositionManager.PositionMessage(__instance.transmitBuffer, count, part), 1);
            }
          }

          return false;
			}
			catch (Exception exception)
			{
				Log.Error($"GhostmodePatch error: {exception}");
				return true;
			}
		}
	}
}