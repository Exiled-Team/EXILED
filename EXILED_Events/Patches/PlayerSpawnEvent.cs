using Harmony;
using Mirror;
using System;
using UnityEngine;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.ApplyProperties))]
	public class PlayerSpawnEvent
	{
		public static bool Prefix(CharacterClassManager __instance, bool lite = false, bool escape = false)
		{
			if (EventPlugin.PlayerSpawnEventPatchDisable)
				return true;

			try
			{
				Role role = __instance.Classes.SafeGet(__instance.CurClass);
				if (!__instance._wasAnytimeAlive && __instance.CurClass != RoleType.Spectator && __instance.CurClass != RoleType.None)
				{
					__instance._wasAnytimeAlive = true;
				}
				__instance.InitSCPs();
				__instance.AliveTime = 0f;
				switch (role.team)
				{
					case Team.MTF:
						AchievementManager.Achieve("arescue");
						break;
					case Team.CHI:
						AchievementManager.Achieve("chaos");
						break;
					case Team.RSC:
					case Team.CDP:
						__instance.EscapeStartTime = (int)Time.realtimeSinceStartup;
						break;
				}
				__instance.GetComponent<Inventory>();
				try
				{
					__instance.GetComponent<FootstepSync>().SetLoudness(role.team, role.roleId.Is939());
				}
				catch
				{
				}
				if (NetworkServer.active)
				{
					Handcuffs component = __instance.GetComponent<Handcuffs>();
					component.ClearTarget();
					component.NetworkCufferId = -1;
				}
				if (role.team != Team.RIP)
				{
					if (NetworkServer.active && !lite)
					{
						Vector3 constantRespawnPoint = NonFacilityCompatibility.currentSceneSettings.constantRespawnPoint;
						if (constantRespawnPoint != Vector3.zero)
						{
							__instance._pms.OnPlayerClassChange(constantRespawnPoint, 0f);
						}
						else
						{
							GameObject randomPosition = CharacterClassManager._spawnpointManager.GetRandomPosition(__instance.CurClass);
							Vector3 spawnPoint = new Vector3(0f, 0f, 0f);
							float rotY = 0f;
							if (randomPosition != null)
							{
								spawnPoint = randomPosition.transform.position;
								rotY = randomPosition.transform.rotation.eulerAngles.y;
								AmmoBox component1 = __instance.GetComponent<AmmoBox>();
								if (escape && __instance.KeepItemsAfterEscaping)
								{
									Inventory component2 = PlayerManager.localPlayer.GetComponent<Inventory>();
									for (ushort index = 0; index < (ushort) 3; ++index)
									{
										if (component1[(int) index] >= 15U)
											component2.SetPickup(component1.types[(int) index].inventoryID, (float) component1[(int) index], randomPosition.transform.position, randomPosition.transform.rotation, 0, 0, 0);
									}
								}
								component1.ResetAmmo();
							}
							else
							{
								spawnPoint = __instance.DeathPosition;
								rotY = 0f;
							}
							Events.InvokePlayerSpawn(__instance, __instance.CurClass, ref spawnPoint, ref rotY);
							__instance._pms.OnPlayerClassChange(spawnPoint, rotY);
						}
						if (!__instance.SpawnProtected && __instance.EnableSP && __instance.SProtectedTeam.Contains((int)role.team))
						{
							__instance.GodMode = true;
							__instance.SpawnProtected = true;
							__instance.ProtectedTime = Time.time;
						}
					}
					if (!__instance.isLocalPlayer)
					{
						__instance.GetComponent<PlayerStats>().maxHP = role.maxHP;
					}
				}
				__instance.Scp0492.iAm049_2 = __instance.CurClass == RoleType.Scp0492;
				__instance.Scp106.iAm106 = __instance.CurClass == RoleType.Scp106;
				__instance.Scp173.iAm173 = __instance.CurClass == RoleType.Scp173;
				__instance.Scp939.iAm939 = __instance.CurClass.Is939();
				__instance.RefreshPlyModel();

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"PlayerSpawnEvent error: {exception}");
				return true;
			}
		}
	}
}