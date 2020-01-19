using System;
using System.Collections.Generic;
using System.Reflection;
using GameCore;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EXILED
{
	public abstract class Plugin
	{
		internal static bool debug = false;
		public static YamlConfig Config;
		public abstract string getName { get; }
		public abstract void OnEnable();
		public abstract void OnDisable();
		public abstract void OnReload();

		//Used to send INFO level messages to the game console.
		public static void Info(string message)
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[INFO] [{assembly.GetName().Name}]: {message}");
		}

		//Used to send DEBUG level messages to the game console. Server must have EXILED_Debug enabled.
		public static void Debug(string message)
		{
			if (!debug)
				return;
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[DEBUG] [{assembly.GetName().Name}]: {message}");
		}

		//Used to send ERROR level messages to the game console. This should be used to send errors only. It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
		public static void Error(string message)
		{
			Assembly assembly = Assembly.GetCallingAssembly();
			ServerConsole.AddLog($"[ERROR] [{assembly.GetName().Name}]: {message}");
		}

		//Gets a list of all current player ReferenceHubs.
		public static List<ReferenceHub> GetHubs()
		{
			List<ReferenceHub> hubs = new List<ReferenceHub>();
			foreach (GameObject obj in PlayerManager.players)
				hubs.Add(ReferenceHub.GetHub(obj));

			return hubs;
		}

		//Gets the spawn point of the selected role.
		public static Vector3 GetRandomSpawnPoint(RoleType role)
		{
			GameObject randomPosition = Object.FindObjectOfType<SpawnpointManager>().GetRandomPosition(role);
			if (randomPosition == null)
				return Vector3.zero;
			return randomPosition.transform.position;
		}

		//Gets the team value of the given role - the normal assembly does not offer a public means of doing this, ree.
		public static Team GetTeam(RoleType role)
		{
			switch (role)
			{
				case RoleType.ChaosInsurgency:
					return Team.CHI;
				case RoleType.Scientist:
					return Team.RSC;
				case RoleType.ClassD:
					return Team.CDP;
				case RoleType.Scp049:
				case RoleType.Scp93953:
				case RoleType.Scp93989:
				case RoleType.Scp0492:
				case RoleType.Scp079:
				case RoleType.Scp096:
				case RoleType.Scp106:
				case RoleType.Scp173:
					return Team.SCP;
				case RoleType.Spectator:
					return Team.RIP;
				case RoleType.FacilityGuard:
				case RoleType.NtfCadet:
				case RoleType.NtfLieutenant:
				case RoleType.NtfCommander:
				case RoleType.NtfScientist:
					return Team.MTF;
				case RoleType.Tutorial:
					return Team.TUT;
				default:
					return Team.RIP;
			}
		}

		//Gets the reference hub of a specific game object.
		public static ReferenceHub GetPlayer(GameObject obj) => ReferenceHub.GetHub(obj);

		//Gets the reference hub of a player based on a string that can be either their playerID, userID, or name, checking in that order. Returns null if no match is found.
		public static ReferenceHub GetPlayer(string args)
		{
			try
			{
				GameObject ply = null;
				if (short.TryParse(args, out short pID))
				{
					Debug("Trying to find by PID..");
					foreach (GameObject pl in PlayerManager.players)
						if (pl.GetComponent<ReferenceHub>()?.queryProcessor.PlayerId == pID)
						{
							ply = pl;
							Debug("Found PID match.");
						}
				}
				else if (args.EndsWith("@steam") || args.EndsWith("@discord") || args.EndsWith("@northwood") ||
				         args.EndsWith("@patreon"))
				{
					Debug("Trying to find by SID..");
					foreach (GameObject pl in PlayerManager.players)
						if (pl.GetComponent<ReferenceHub>()?.characterClassManager.UserId == args)
						{
							ply = pl;
							Debug("Found SID match.");
						}
				}
				else
				{
					Debug($"Trying to find by name.. {args}");
					if (args == "WORLD" || args == "SCP-018" || args == "SCP-575" || args == "SCP-207")
						return null;
					int maxNameLength = 31, lastnameDifference = 31;
					string str1 = args.ToLower();
					foreach (GameObject pl in PlayerManager.players)
					{
						ReferenceHub rh;
						try
						{
							rh = pl.GetComponent<ReferenceHub>();
						}
						catch (Exception e)
						{
							ServerConsole.AddLog(e.ToString());
							continue;
						}

						if (!rh.nicknameSync.MyNick.ToLower().Contains(args.ToLower()))
							continue;
						if (str1.Length < maxNameLength)
						{
							int x = maxNameLength - str1.Length;
							int y = maxNameLength - rh.nicknameSync.MyNick.Length;
							string str2 = rh.nicknameSync.MyNick;
							for (int i = 0; i < x; i++) str1 += "z";

							for (int i = 0; i < y; i++) str2 += "z";

							int nameDifference = LevenshteinDistance.Compute(str1, str2);
							if (nameDifference < lastnameDifference)
							{
								lastnameDifference = nameDifference;
								ply = pl;
								Debug("Found name match.");
							}
						}
					}
				}

				ReferenceHub hub = ReferenceHub.GetHub(ply);
				if (hub == null)
					return null;
				return hub;
			}
			catch (Exception)
			{
				return null;
			}
		}
	}
}