using EXILED.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace EXILED
{
	public abstract class Plugin
	{
		[Obsolete("Use Log.debug")]
		internal static bool debug = Log.debug;

		public static YamlConfig Config;
		public abstract string getName { get; }
		public abstract void OnEnable();
		public abstract void OnDisable();
		public abstract void OnReload();

		[Obsolete("Use Log.Info")]
		public static void Info(string message) => Log.Info(message);

		[Obsolete("Use Log.Debug")]
		public static void Debug(string message) => Log.Debug(message);

		[Obsolete("Use Log.Warn")]
		public static void Warn(string message) => Log.Warn(message);

		[Obsolete("Use Log.Error")]
		public static void Error(string message) => Log.Error(message);

		[Obsolete("Use Player.GetHubs")]
		public static List<ReferenceHub> GetHubs() => Player.GetHubs().ToList();

		[Obsolete("Use Map.GetRandomSpawnPoint")]
		public static Vector3 GetRandomSpawnPoint(RoleType role) => Map.GetRandomSpawnPoint(role);

		[Obsolete("Use Player.GetTeam")]
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

		[Obsolete("Use Player.GetPlayer")]
		public static ReferenceHub GetPlayer(GameObject obj) => ReferenceHub.GetHub(obj);

		[Obsolete("Use Player.GetPlayer")]
		public static ReferenceHub GetPlayer(int pId) => Player.GetPlayer(pId);

		[Obsolete("Use Player.GetPlayer")]
		public static ReferenceHub GetPlayer(string args) => Player.GetPlayer(args);
	}
}