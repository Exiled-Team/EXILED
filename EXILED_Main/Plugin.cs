using System;
using System.Collections.Generic;
using System.Linq;
using EXILED.Extensions;
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
		public static Team GetTeam(RoleType role) => Player.GetTeam(role);
		
		[Obsolete("Use Player.GetPlayer")]
		public static ReferenceHub GetPlayer(GameObject obj) => ReferenceHub.GetHub(obj);
		
		[Obsolete("Use Player.GetPlayer")]
		public static ReferenceHub GetPlayer(int pId) => Player.GetPlayer(pId);
		
		[Obsolete("Use Player.GetPlayer")]
		public static ReferenceHub GetPlayer(string args) => Player.GetPlayer(args);
	}
}