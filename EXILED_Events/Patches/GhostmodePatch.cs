using System;
using System.Collections.Generic;
using Harmony;
using Mirror;
using UnityEngine;
using Utf8Json.Resolvers.Internal;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(PlayerPositionManager), "TransmitData")]
	public class GhostmodePatch
	{
		public static bool Prefix(PlayerPositionManager __instance)
		{
			if (EventPlugin.GhostmodePatchDisable)
				return true;
			
			try
			{
				List<GameObject> players = PlayerManager.players;
				__instance.usedData = players.Count;
				if (__instance.receivedData == null || __instance.receivedData.Length < __instance.usedData)
					__instance.receivedData = new PlayerPositionData[__instance.usedData * 2];
				for (int index = 0; index < __instance.usedData; ++index)
					__instance.receivedData[index] = new PlayerPositionData(players[index]);
				if (__instance.transmitBuffer == null || __instance.transmitBuffer.Length < __instance.usedData)
					__instance.transmitBuffer = new PlayerPositionData[__instance.usedData * 2];
				foreach (GameObject gameObject in players)
				{
					CharacterClassManager component1 = gameObject.GetComponent<CharacterClassManager>();
					Array.Copy(__instance.receivedData, __instance.transmitBuffer, __instance.usedData);

					if (component1.CurClass == RoleType.Scp096 || component1.CurClass == RoleType.Scp173)
					{
						for (int i = 0; i < __instance.usedData; i++)
						{
							ReferenceHub hub = Plugin.GetPlayer(__instance.transmitBuffer[i].playerID.ToString());
							if (hub.characterClassManager.CurClass != RoleType.Tutorial)
								continue;
							Scp049PlayerScript script = hub.GetComponent<Scp049PlayerScript>();
							Vector3 fwd = script.plyCam.transform.forward;
							Vector3 pos = script.gameObject.transform.position;
							Vector3 position = component1.gameObject.transform.position;
							float angle = Vector3.Angle(fwd,
								(pos - position).normalized);
							Vector3 dir = (pos - position).normalized;
							Quaternion rot = Quaternion.LookRotation(dir);
							if (angle <= 80f)
							{
								__instance.transmitBuffer[i] = new PlayerPositionData(__instance.transmitBuffer[i].position, Quaternion.Inverse(rot).y, __instance.transmitBuffer[i].playerID, __instance.transmitBuffer[i].uses268);
							}
						}
					}

					if (component1.CurClass.Is939())
					{
						for (int index = 0; index < __instance.usedData; ++index)
						{
							if (__instance.transmitBuffer[index].position.y < 800.0)
							{
								CharacterClassManager component2 = players[index].GetComponent<CharacterClassManager>();
								if (component2.Classes.SafeGet(component2.CurClass).team != Team.SCP &&
								    component2.Classes.SafeGet(component2.CurClass).team != Team.RIP && !players[index]
									    .GetComponent<Scp939_VisionController>()
									    .CanSee(component1.GetComponent<Scp939PlayerScript>()))
									__instance.transmitBuffer[index] = new PlayerPositionData(Vector3.up * 6000f, 0.0f,
										__instance.transmitBuffer[index].playerID);
							}
						}
					}
					// else if (component1.CurClass == RoleType.Scp096)
					// {
					// 	Scp096PlayerScript script = component1.GetComponent<Scp096PlayerScript>();
					// 	if (script.Networkenraged == Scp096PlayerScript.RageState.Enraged || script.Networkenraged == Scp096PlayerScript.RageState.Panic)
					// 		for (int i = 0; i < __instance.usedData; i++)
					// 			if (!Plugin.Scp096Targets.Contains(__instance.transmitBuffer[i].playerID))
					// 				__instance.transmitBuffer[i] = new PlayerPositionData(Vector3.up * 6000f, 0f, __instance.transmitBuffer[i].playerID);
					// }
					else if (component1.CurClass != RoleType.Scp079 && component1.CurClass != RoleType.Spectator)
					{
						for (int index = 0; index < __instance.usedData; ++index)
						{
							if (__instance.transmitBuffer[index].uses268 || EventPlugin.GhostedIds.Contains(__instance.transmitBuffer[index].playerID))
								__instance.transmitBuffer[index] = new PlayerPositionData(Vector3.up * 6000f, 0.0f,
									__instance.transmitBuffer[index].playerID);
						}
					}

					NetworkConnection networkConnection = component1.netIdentity.isLocalPlayer
						? NetworkServer.localConnection
						: component1.netIdentity.connectionToClient;
					if (__instance.usedData <= 20)
					{
						networkConnection.Send(
							new PlayerPositionManager.PositionMessage(__instance.transmitBuffer,
								(byte) __instance.usedData, 0), 1);
					}
					else
					{
						byte part;
						for (part = (byte) 0; (int) part < __instance.usedData / 20; ++part)
							networkConnection.Send(
								new PlayerPositionManager.PositionMessage(__instance.transmitBuffer, 20, part),
								1);
						byte count = (byte) (__instance.usedData % (part * 20));
						if (count > 0)
							networkConnection.Send(
								new PlayerPositionManager.PositionMessage(__instance.transmitBuffer, count, part), 1);
					}
				}

				return false;
			}
			catch (Exception e)
			{
				Plugin.Error($"TransmitData Error: {e}");
				return true;
			}
		}
	}
}