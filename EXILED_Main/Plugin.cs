using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EXILED.Extensions;
using GameCore;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EXILED
{
	public abstract class Plugin
	{
		internal static bool debug = Log.debug;
		public static YamlConfig Config;
		public abstract string getName { get; }
		public abstract void OnEnable();
		public abstract void OnDisable();
		public abstract void OnReload();
		
		
		//Used to send INFO level messages to the game console.
		[Obsolete("Use Log.Info")]
		public static void Info(string message) => Log.Info(message);

		//Used to send DEBUG level messages to the game console. Server must have EXILED_Debug enabled.
		[Obsolete("Use Log.Debug")]
		public static void Debug(string message) => Log.Debug(message);

		[Obsolete("Use Log.Warn")]
		public static void Warn(string message) => Log.Warn(message);

		//Used to send ERROR level messages to the game console. This should be used to send errors only. It's recommended to send any messages in the catch block of a try/catch as errors with the exception string.
		[Obsolete("Use Log.Error")]
		public static void Error(string message) => Log.Error(message);

		//Gets a list of all current player ReferenceHubs.
		[Obsolete("Use Player.GetHubs")]
		public static List<ReferenceHub> GetHubs() => Player.GetHubs().ToList();

		//Gets the spawn point of the selected role.
		[Obsolete("Use Map.GetRandomSpawnPoint")]
		public static Vector3 GetRandomSpawnPoint(RoleType role) => Map.GetRandomSpawnPoint(role);

		//Gets the team value of the given role - the normal assembly does not offer a public means of doing this, ree.
		[Obsolete("Use Player.GetTeam")]
		public static Team GetTeam(RoleType role) => Player.GetTeam(role);

		//Gets the reference hub of a specific game object.
		[Obsolete("Use Player.GetPlayer")]
		public static ReferenceHub GetPlayer(GameObject obj) => ReferenceHub.GetHub(obj);
		
		[Obsolete("Use Player.GetPlayer")]
		public static ReferenceHub GetPlayer(int pId) => Player.GetPlayer(pId);

		//Gets the reference hub of a player based on a string that can be either their playerID, userID, or name, checking in that order. Returns null if no match is found.
		[Obsolete("Use Player.GetPlayer")]
		public static ReferenceHub GetPlayer(string args) => Player.GetPlayer(args);
	}
}