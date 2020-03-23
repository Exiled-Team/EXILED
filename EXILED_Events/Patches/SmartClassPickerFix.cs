using GameCore;
using Harmony;
using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using Console = GameCore.Console;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

namespace EXILED.Patches
{
	[HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.RunSmartClassPicker))]
	public class SmartClassPickerFix
	{
		public static bool Prefix(CharacterClassManager __instance)
		{
			try
			{
				Dictionary<GameObject, RoleType> roles = new Dictionary<GameObject, RoleType>();
				RoleType forcedClass = NonFacilityCompatibility.currentSceneSettings.forcedClass;
				GameObject[] array = __instance.GetShuffledPlayerList().ToArray();
				RoundSummary.SumInfo_ClassList startClassList = default;
				bool flag = false;
				int num = 0;
				float[] array2 = new float[] { 0f, 0.4f, 0.6f, 0.5f };
				__instance.laterJoinNextIndex = 0;
				int r = EventPlugin.Gen.Next(1, 100);

				for (int i = 0; i < array.Length; i++)
				{
					RoleType roleType = (RoleType)((__instance.ForceClass < RoleType.Scp173)
						? __instance.FindRandomIdUsingDefinedTeam(__instance.ClassTeamQueue[i])
						: ((int)__instance.ForceClass));
					__instance.laterJoinNextIndex++;
					if (__instance.Classes.CheckBounds(forcedClass))
					{
						roleType = forcedClass;
					}

					if (r <= __instance.CiPercentage && roleType == RoleType.FacilityGuard)
						roleType = RoleType.ChaosInsurgency;

					switch (__instance.Classes.SafeGet(roleType).team)
					{
						case Team.SCP:
							startClassList.scps_except_zombies++;
							break;
						case Team.MTF:
							startClassList.mtf_and_guards++;
							break;
						case Team.CHI:
							startClassList.chaos_insurgents++;
							break;
						case Team.RSC:
							startClassList.scientists++;
							break;
						case Team.CDP:
							startClassList.class_ds++;
							break;
					}

					if (__instance.Classes.SafeGet(roleType).team == Team.SCP && !flag)
					{
						if (array2[Mathf.Clamp(num, 0, array2.Length)] > Random.value)
						{
							flag = true;
							__instance.Classes.Get(roleType).banClass = false;
							roleType = RoleType.Scp079;
						}

						num++;
					}

					if (TutorialManager.status)
					{
						__instance.SetPlayersClass(RoleType.Tutorial, __instance.gameObject);
					}
					else
					{
						if (!roles.ContainsKey(array[i]))
						{
							roles.Add(array[i], roleType);
						}
						else
						{
							roles[array[i]] = roleType;
						}

						ServerLogs.AddLog(ServerLogs.Modules.ClassChange,
							string.Concat(array[i].GetComponent<NicknameSync>().MyNick, " (",
								array[i].GetComponent<CharacterClassManager>().UserId, ") spawned as ",
								__instance.Classes.SafeGet(roleType).fullName.Replace("\n", ""), "."),
							ServerLogs.ServerLogType.GameEvent);
					}
				}

				Object.FindObjectOfType<PlayerList>().NetworkRoundStartTime = (int)Time.realtimeSinceStartup;
				startClassList.time = (int)Time.realtimeSinceStartup;
				startClassList.warhead_kills = -1;
				Object.FindObjectOfType<RoundSummary>().SetStartClassList(startClassList);
				if (ConfigFile.ServerConfig.GetBool("smart_class_picker", true))
				{
					string str = "Before Starting";
					try
					{
						str = "Setting Initial Value";
						if (ConfigFile.smBalancedPicker == null)
						{
							ConfigFile.smBalancedPicker = new Dictionary<string, int[]>();
						}

						str = "Valid Players List Error";
						List<GameObject> shuffledPlayerList = __instance.GetShuffledPlayerList();
						str = "Copying Balanced Picker List";
						Dictionary<string, int[]> dictionary =
							new Dictionary<string, int[]>(ConfigFile.smBalancedPicker);
						str = "Clearing Balanced Picker List";
						ConfigFile.smBalancedPicker.Clear();
						str = "Re-building Balanced Picker List";
						foreach (GameObject gameObject in shuffledPlayerList)
						{
							if (!(gameObject == null))
							{
								CharacterClassManager component = gameObject.GetComponent<CharacterClassManager>();
								NetworkConnection networkConnection = null;
								if (component != null)
								{
									networkConnection = (component.connectionToClient ?? component.connectionToServer);
								}

								str = "Getting Player ID";
								if (networkConnection == null && component == null)
								{
									shuffledPlayerList.Remove(gameObject);
									break;
								}

								if (__instance.SrvRoles.DoNotTrack)
								{
									shuffledPlayerList.Remove(gameObject);
								}
								else
								{
									string str4 = (networkConnection != null) ? networkConnection.address : "";
									string str2 = (component != null) ? component.UserId : "";
									string text = str4 + str2;
									str = "Setting up Player \"" + text + "\"";
									if (!dictionary.ContainsKey(text))
									{
										str = "Adding Player \"" + text + "\" to smBalancedPicker";
										int[] arra = new int[__instance.Classes.Length];
										for (int j = 0; j < arra.Length; j++)
										{
											arra[j] = ConfigFile.ServerConfig.GetInt("smart_cp_starting_weight", 6);
										}

										ConfigFile.smBalancedPicker.Add(text, arra);
									}
									else
									{
										str = "Updating Player \"" + text + "\" in smBalancedPicker";

										if (dictionary.TryGetValue(text, out int[] value))
										{
											ConfigFile.smBalancedPicker.Add(text, value);
										}
									}
								}
							}
						}

						str = "Clearing Copied Balanced Picker List";
						dictionary.Clear();
						List<RoleType> list = new List<RoleType>();
						str = "Getting Available Roles";
						if (shuffledPlayerList.Contains(null))
						{
							shuffledPlayerList.Remove(null);
						}

						foreach (GameObject gameObject2 in shuffledPlayerList)
						{
							if (!(gameObject2 == null))
							{
								RoleType rt = RoleType.None;
								roles.TryGetValue(gameObject2, out rt);
								if (rt != RoleType.None)
								{
									list.Add(rt);
								}
								else
								{
									shuffledPlayerList.Remove(gameObject2);
								}
							}
						}

						List<GameObject> list2 = new List<GameObject>();
						str = "Setting Roles";
						foreach (GameObject gameObject3 in shuffledPlayerList)
						{
							if (!(gameObject3 == null))
							{
								CharacterClassManager component2 = gameObject3.GetComponent<CharacterClassManager>();
								NetworkConnection networkConnection2 = null;
								if (component2 != null)
								{
									networkConnection2 =
										(component2.connectionToClient ?? component2.connectionToServer);
								}

								if (networkConnection2 == null && component2 == null)
								{
									shuffledPlayerList.Remove(gameObject3);
									break;
								}

								string str5 = (networkConnection2 != null) ? networkConnection2.address : "";
								string str3 = (component2 != null) ? component2.UserId : "";
								string text2 = str5 + str3;
								str = "Setting Player \"" + text2 + "\"'s Class";
								RoleType mostLikelyClass = __instance.GetMostLikelyClass(text2, list);
								if (mostLikelyClass != RoleType.None)
								{
									if (!roles.ContainsKey(gameObject3))
									{
										roles.Add(gameObject3, mostLikelyClass);
									}
									else
									{
										roles[gameObject3] = mostLikelyClass;
									}

									ServerLogs.AddLog(ServerLogs.Modules.ClassChange,
										string.Concat(gameObject3.GetComponent<NicknameSync>().MyNick, " (",
											gameObject3.GetComponent<CharacterClassManager>().UserId, ") class set to ",
											__instance.Classes.SafeGet(mostLikelyClass).fullName.Replace("\n", ""),
											" by Smart Class Picker."), ServerLogs.ServerLogType.GameEvent);
									list.Remove(mostLikelyClass);
								}
								else
								{
									list2.Add(gameObject3);
								}
							}
						}

						str = "Reversing Additional Classes List";
						list.Reverse();
						str = "Setting Unknown Players Classes";
						foreach (GameObject gameObject4 in list2)
						{
							if (gameObject4 == null)
								continue;
							if (list.Count > 0)
							{
								RoleType roleType2 = list[0];
								if (!roles.ContainsKey(gameObject4))
								{
									roles.Add(gameObject4, roleType2);
								}
								else
								{
									roles[gameObject4] = roleType2;
								}

								ServerLogs.AddLog(ServerLogs.Modules.ClassChange,
									string.Concat(gameObject4.GetComponent<NicknameSync>().MyNick, " (",
										gameObject4.GetComponent<CharacterClassManager>().UserId, ") class set to ",
										__instance.Classes.SafeGet(roleType2).fullName.Replace("\n", ""),
										" by Smart Class Picker."), ServerLogs.ServerLogType.GameEvent);
								list.Remove(roleType2);
							}
							else
							{
								roles.Add(gameObject4, RoleType.Spectator);
								ServerLogs.AddLog(ServerLogs.Modules.ClassChange,
									gameObject4.GetComponent<NicknameSync>().MyNick + " (" +
									gameObject4.GetComponent<CharacterClassManager>().UserId +
									") class set to SPECTATOR by Smart Class Picker.",
									ServerLogs.ServerLogType.GameEvent);
							}
						}

						str = "Clearing Unknown Players List";
						list2.Clear();
						str = "Clearing Available Classes List";
						list.Clear();
					}
					catch (Exception ex)
					{
						Console.AddLog("Smart Class Picker Failed: " + str + ", " + ex.Message,
							new Color32(byte.MaxValue, 180, 0, byte.MaxValue));
						return true;
					}
				}

				foreach (KeyValuePair<GameObject, RoleType> rtr in roles)
				{
					__instance.SetPlayersClass(rtr.Value, rtr.Key);
				}

				return false;
			}
			catch (Exception exception)
			{
				Log.Error($"SmartClassPickerFix error: {exception}");
				return true;
			}
		}
	}
}