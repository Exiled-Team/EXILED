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
          ++__instance._frame;
          if (__instance._frame != __instance._syncFrequency)
             return false;
          __instance._frame = 0;
          List<GameObject> players = PlayerManager.players;
          __instance._usedData = players.Count;
          if (__instance._receivedData == null || __instance._receivedData.Length < __instance._usedData)
            __instance._receivedData = new PlayerPositionData[__instance._usedData * 2];
          for (int index = 0; index < __instance._usedData; ++index)
            __instance._receivedData[index] = new PlayerPositionData(ReferenceHub.GetHub(players[index]));
          if (__instance._transmitBuffer == null || __instance._transmitBuffer.Length < __instance._usedData)
            __instance._transmitBuffer = new PlayerPositionData[__instance._usedData * 2];
          foreach (GameObject player in players)
          {
            ReferenceHub hub1 = ReferenceHub.GetHub(player);
            Array.Copy(__instance._receivedData, __instance._transmitBuffer, __instance._usedData);
            if (hub1.characterClassManager.CurClass.Is939())
            {
              for (int index = 0; index < __instance._usedData; ++index)
              {
                if (__instance._transmitBuffer[index].position.y < 800.0)
                {
                  ReferenceHub hub2 = ReferenceHub.GetHub(players[index]);
                  if (EventPlugin.GhostedIds.Contains(__instance._transmitBuffer[index].playerID) || hub2.characterClassManager.CurRole.team != Team.SCP && hub2.characterClassManager.CurRole.team != Team.RIP && !players[index].GetComponent<Scp939_VisionController>().CanSee(hub1.characterClassManager.Scp939))
                    __instance._transmitBuffer[index] = new PlayerPositionData(Vector3.up * 6000f, 0.0f, __instance._transmitBuffer[index].playerID);
                }
              }
            }
            else if (hub1.characterClassManager.CurClass != RoleType.Scp079 && hub1.characterClassManager.CurClass != RoleType.Spectator)
            {
              for (int index = 0; index < __instance._usedData; ++index)
              {
                if (Math.Abs(__instance._transmitBuffer[index].position.y - hub1.playerMovementSync.RealModelPosition.y) > 35)
                {
                  __instance._transmitBuffer[index] = new PlayerPositionData(Vector3.up * 6000f, 0.0f, __instance._transmitBuffer[index].playerID);
                }
                else
                {
                  ReferenceHub hub2;
                  if (ReferenceHub.TryGetHub(__instance._transmitBuffer[index].playerID, out hub2))
                  {
                    if (EventPlugin.GhostedIds.Contains(__instance._transmitBuffer[index].playerID) ||
                        hub1.scpsController.CurrentScp is Scp096 currentScp && currentScp.Enraged &&
                        (!currentScp.HasTarget(hub2) && hub2.characterClassManager.CurRole.team != Team.SCP))
                      __instance._transmitBuffer[index] = new PlayerPositionData(Vector3.up * 6000f, 0.0f,
                        __instance._transmitBuffer[index].playerID);

                    if (hub2.playerEffectsController.GetEffect<Scp268>().Enabled)
                    {
                      bool flag = false;
                      if (hub1.scpsController.CurrentScp is Scp096 curScp && curScp != null)
                        flag = curScp.HasTarget(hub2);
                      if (hub1.characterClassManager.CurClass != RoleType.Scp079 &&
                          hub1.characterClassManager.CurClass != RoleType.Spectator && !flag)
                        __instance._transmitBuffer[index] = new PlayerPositionData(Vector3.up * 6000f, 0.0f,
                          __instance._transmitBuffer[index].playerID);
                    }
                  }
                }
              }
            }
         
            if (EventPlugin.TargetGhost.ContainsKey(player.GetPlayer()))
            {
              for (int i = 0; i < __instance._usedData; i++)
              {
                if (EventPlugin.TargetGhost[player.GetPlayer()]
                  .Contains(__instance._transmitBuffer[i].playerID))
                  __instance._transmitBuffer[i] = new PlayerPositionData(Vector3.up * 6000f, 0.0f, __instance._transmitBuffer[i].playerID);
              }
            }
         
            NetworkConnection networkConnection = hub1.characterClassManager.netIdentity.isLocalPlayer ? NetworkServer.localConnection : hub1.characterClassManager.netIdentity.connectionToClient;
            if (__instance._usedData <= 20)
            {
              networkConnection.Send(new PlayerPositionManager.PositionMessage(__instance._transmitBuffer, (byte) __instance._usedData, 0), 1);
            }
            else
            {
              byte part;
              for (part = (byte) 0; (int) part < __instance._usedData / 20; ++part)
                networkConnection.Send(new PlayerPositionManager.PositionMessage(__instance._transmitBuffer, 20, part), 1);
              byte count = (byte) (__instance._usedData % (part * 20));
              if (count > 0)
                networkConnection.Send(new PlayerPositionManager.PositionMessage(__instance._transmitBuffer, count, part), 1);
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