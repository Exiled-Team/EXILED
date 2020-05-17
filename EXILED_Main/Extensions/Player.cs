using EXILED.ApiObjects;
using Mirror;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace EXILED.Extensions
{
	public static class Player
	{
		private static MethodInfo sendSpawnMessage;
		public static MethodInfo SendSpawnMessage
		{
			get
			{
				if (sendSpawnMessage == null)
					sendSpawnMessage = typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.Instance | BindingFlags.InvokeMethod
						| BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);

				return sendSpawnMessage;
			}
		}

		public static Dictionary<int, ReferenceHub> IdHubs = new Dictionary<int, ReferenceHub>();
		public static Dictionary<string, ReferenceHub> StrHubs = new Dictionary<string, ReferenceHub>();

		/// <summary>
		/// Returns an IEnumerable of ReferenceHubs for all players on the server.
		/// </summary>
		/// <returns>IEnumerable(ReferenceHub)</returns>
		public static IEnumerable<ReferenceHub> GetHubs() => ReferenceHub.Hubs.Values.Where(h => !h.IsHost());

		/// <summary>
		/// Gets a player's UserID
		/// </summary>
		/// <param name="player">Player</param>
		/// <returns>string, can be empty.</returns>
		public static string GetUserId(this ReferenceHub player) => player.characterClassManager.UserId;

		/// <summary>
		/// Sets a player's UserID
		/// </summary>
		/// <param name="player"></param>
		/// <param name="newId"></param>
		/// <returns></returns>
		public static void SetUserId(this ReferenceHub player, string newId) => player.characterClassManager.NetworkSyncedUserId = newId;

		/// <summary>
		/// Gets a player's PlayerID
		/// </summary>
		/// <param name="player">Player</param>
		/// <returns>int PlayerID</returns>
		public static int GetPlayerId(this ReferenceHub player) => player.queryProcessor.PlayerId;

		/// <summary>
		/// Sets a player's PlayerID
		/// </summary>
		/// <param name="player"></param>
		/// <param name="newId"></param>
		/// <returns></returns>
		public static void SetPlayerId(this ReferenceHub player, int newId) => player.queryProcessor.NetworkPlayerId = newId;

		/// <summary>
		/// Gets a player's Overwatch status.
		/// </summary>
		/// <param name="player">Player</param>
		/// <returns>True if in overwatch.</returns>
		public static bool GetOverwatch(this ReferenceHub player) => player.serverRoles.OverwatchEnabled;

		/// <summary>
		/// Sets a player's Overwatch status.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="newStatus"></param>
		/// <returns></returns>
		public static void SetOverwatch(this ReferenceHub player, bool newStatus) => player.serverRoles.SetOverwatchStatus(newStatus);
		
		/// <summary>
		/// Check if a <see cref="RoleType">RoleType</see> is any SCP.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static bool IsAnyScp( this RoleType type ) => 
		type == RoleType.Scp173 ||type == RoleType.Scp049 || type == RoleType.Scp93989     
		|| type == RoleType.Scp93953 || type == RoleType.Scp0492 || type == RoleType.Scp079 
		|| type == RoleType.Scp106 || type == RoleType.Scp096;
		
		/// <summary>
		/// Check if a <see cref="ReferenceHub">player</see> is any SCP.
		/// </summary>
		/// <param name="hub"></param>
		/// <returns></returns>
		public static bool IsScp(this ReferenceHub hub) => hub.characterClassManager.IsAnyScp();
		
		/// <summary>
		/// Checks if a player's role type is any NTF type <see cref="ReferenceHub"/>.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
       		public static bool IsNTF(this ReferenceHub hub)
        	{
            		switch (hub.GetRole())
            		{
                		case RoleType.NtfCadet:
                		case RoleType.NtfScientist:
                		case RoleType.NtfLieutenant:
                		case RoleType.NtfCommander:
                    			return true;
                		default:
                    			return false;
            		}
        	}
		
		/// <summary>
		/// Gets a player's Current Role.
		/// </summary>
		/// <param name="player">Player</param>
		/// <returns>RoleType Player's role</returns>
		public static RoleType GetRole(this ReferenceHub player) => player.characterClassManager.CurClass;

		/// <summary>
		/// Sets a player's role.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="newRole"></param>
		public static void SetRole(this ReferenceHub player, RoleType newRole) => player.characterClassManager.SetPlayersClass(newRole, player.gameObject);

		public static void SetRole(this ReferenceHub player, RoleType newRole, bool keepPosition)
		{
			if (keepPosition)
			{
				player.characterClassManager.NetworkCurClass = newRole;
				player.playerStats.SetHPAmount(player.characterClassManager.Classes.SafeGet(player.GetRole()).maxHP);
			}
			else
				SetRole(player, newRole);
		}

		/// <summary>
		/// Gets the position of a <see cref="ReferenceHub"/>
		/// </summary>
		public static Vector3 GetPosition(this ReferenceHub player) => player.plyMovementSync.GetRealPosition();

		/// <summary>
		/// Gets the rotations from a <see cref="ReferenceHub"/>
		/// </summary>
		/// <returns>A <see cref="Vector2"/>, representing the directions he's looking at</returns>
		public static Vector2 GetRotations(this ReferenceHub player) => player.plyMovementSync.NetworkRotations;

		/// <summary>
		/// Gets the rotation of a <see cref="ReferenceHub"/>.
		/// </summary>
		/// <returns>The direction he's looking at, useful for Raycasts</returns>
		public static Vector3 GetRotationVector(this ReferenceHub player) => player.characterClassManager.Scp049.plyCam.transform.forward;

		/// <summary>
		/// Sets the position of a <see cref="ReferenceHub"/> using a <see cref="Vector3"/>.
		/// </summary>
		public static void SetPosition(this ReferenceHub player, Vector3 position) => player.SetPosition(position.x, position.y, position.z);

		/// <summary>
		/// Sets the position of a <see cref="ReferenceHub"/> using the x, y, and z of the destination position.
		/// </summary>
		public static void SetPosition(this ReferenceHub player, float x, float y, float z) => player.plyMovementSync.OverridePosition(new Vector3(x, y, z), player.transform.rotation.eulerAngles.y);

		/// <summary>
		/// Sets the rotation of a <see cref="ReferenceHub"/> using a <see cref="Vector2"/>.
		/// </summary>
		public static void SetRotation(this ReferenceHub player, Vector2 rotations) => player.SetRotation(rotations.x, rotations.y);

		/// <summary>
		/// Sets the rotation of a <see cref="ReferenceHub"/> using the x and y values of the desired rotation.
		/// </summary>
		public static void SetRotation(this ReferenceHub player, float x, float y) => player.plyMovementSync.NetworkRotations = new Vector2(x, y);

		/// <summary>
		/// Sets the rank of a <see cref="ReferenceHub"/> to a <see cref="UserGroup"/>.
		/// Can be null.
		/// </summary>
		public static UserGroup GetRank(this ReferenceHub player) => player.serverRoles.Group;

		/// <summary>
		/// Sets the rank color of a <see cref="ReferenceHub"/> to a given color with a <see cref="string"/>.
		/// </summary>
		public static void SetRankColor(this ReferenceHub player, string color) => player.serverRoles.SetColor(color);

		/// <summary>
		/// Sets the rank name of a <see cref="ReferenceHub"/> to a given <see cref="string"/>.
		/// </summary>
		public static void SetRankName(this ReferenceHub player, string name) => player.serverRoles.SetText(name);

		/// <summary>
		/// Sets the rank of a <see cref="ReferenceHub"/> by giving a <paramref name="name"/>, <paramref name="color"/>, and setting if it should be shown with <paramref name="show"/>.
		/// </summary>
		public static void SetRank(this ReferenceHub player, string name, string color, bool show)
		{
			// Developer note: I bet I just needed to use the show once. But hey, better be safe than sorry.
			UserGroup ug = new UserGroup()
			{
				BadgeColor = color,
				BadgeText = name,
				HiddenByDefault = !show,
				Cover = show
			};

			player.serverRoles.SetGroup(ug, false, false, show);
		}

		/// <summary>
		/// Sets the rank of a <see cref="ReferenceHub">player</see> by giving a <paramref name="name"/>, <paramref name="color"/>, and setting if it should be shown with <paramref name="show"/>, as well as the <paramref name="rankName"/>, the server should use for permissions.
		/// </summary>
		public static void SetRank(this ReferenceHub player, string name, string color, bool show, string rankName)
		{
			// Developer note: I bet I just needed to use the show once. But hey, better be safe than sorry.
			if (ServerStatic.GetPermissionsHandler()._groups.ContainsKey(rankName))
			{
				ServerStatic.GetPermissionsHandler()._groups[rankName].BadgeColor = color;
				ServerStatic.GetPermissionsHandler()._groups[rankName].BadgeText = name;
				ServerStatic.GetPermissionsHandler()._groups[rankName].HiddenByDefault = !show;
				ServerStatic.GetPermissionsHandler()._groups[rankName].Cover = show;

				player.serverRoles.SetGroup(ServerStatic.GetPermissionsHandler()._groups[rankName], false, false, show);
			}
			else
			{
				UserGroup ug = new UserGroup()
				{
					BadgeColor = color,
					BadgeText = name,
					HiddenByDefault = !show,
					Cover = show
				};

				ServerStatic.GetPermissionsHandler()._groups.Add(rankName, ug);
				player.serverRoles.SetGroup(ug, false, false, show);
			}

			if (ServerStatic.GetPermissionsHandler()._members.ContainsKey(player.GetUserId()))
			{
				ServerStatic.GetPermissionsHandler()._members[player.GetUserId()] = rankName;
			}
			else
			{
				ServerStatic.GetPermissionsHandler()._members.Add(player.GetUserId(), rankName);
			}
		}

		/// <summary>
		/// Sets the rank of a <see cref="ReferenceHub"/> to a <see cref="UserGroup"/>.
		/// </summary>
		public static void SetRank(this ReferenceHub player, UserGroup userGroup) => player.serverRoles.SetGroup(userGroup, false, false, false);

		/// <summary>
		/// Gets the nickname of a <see cref="ReferenceHub"/>
		/// </summary>
		public static string GetNickname(this ReferenceHub player) => player.nicknameSync.Network_myNickSync;

		/// <summary>
		/// Sets the nickname of a <see cref="ReferenceHub"/> to <paramref name="nickname"/>
		/// </summary>
		public static void SetNickname(this ReferenceHub player, string nickname)
		{
			player.nicknameSync.Network_myNickSync = nickname;
			MEC.Timing.RunCoroutine(BlinkTag(player));
		}

		private static IEnumerator<float> BlinkTag(ReferenceHub player)
		{
			yield return MEC.Timing.WaitForOneFrame;

			player.HideTag();

			yield return MEC.Timing.WaitForOneFrame;

			player.ShowTag();
		}

		/// <summary>
		/// Hides the tag of a <see cref="ReferenceHub"/>.
		/// </summary>
		/// <param name="player"></param>
		private static void HideTag(this ReferenceHub player) => player.characterClassManager.CallCmdRequestHideTag();

		/// <summary>
		/// Shows the tag of a <see cref="ReferenceHub"/>.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="isGlobal"></param>
		private static void ShowTag(this ReferenceHub player, bool isGlobal = false) => player.characterClassManager.CallCmdRequestShowTag(isGlobal);

		/// <summary>
		/// Gives an item to the specified <see cref="ReferenceHub"/>.
		/// </summary>
		/// <param name="itemType">Your <see cref="ItemType"/></param>
		/// <param name="duration">The durability (most of the times ammo) of the item</param>
		/// <param name="sight">0 is no sight, 1 is the first sight in the Weapon Manager</param>
		/// <param name="barrel">0 is no custom barrel, 1 is the first barrel in the Weapon Manager</param>
		/// <param name="other">0 is no extra attachment, other numbers are the ammo counter, flashlight, etc.</param>
		[Obsolete("Use AddItem instead.", true)]
		public static void GiveItem(this ReferenceHub player, ItemType itemType, float duration = float.NegativeInfinity, int sight = 0, int barrel = 0, int other = 0) => player.inventory.AddNewItem(itemType, duration, sight, barrel, other);

		// Adapted from https://github.com/galaxy119/SamplePlugin/blob/master/SamplePlugin/Extensions.cs
		/// <summary>
		/// Sends a message to the RA console of a <paramref name="sender"/>. <paramref name="pluginName"/> will be the name of your <see cref="Assembly"/>
		/// </summary>
		public static void RAMessage(this CommandSender sender, string message, bool success = true, string pluginName = null)
		{
			sender.RaReply((pluginName ?? Assembly.GetCallingAssembly().GetName().Name) + "#" + message, success, true, string.Empty);
		}

		/// <summary>
		/// A simple broadcast to a <see cref="ReferenceHub"/>. Doesn't get logged to the console and can be monospace.
		/// </summary>
		public static void Broadcast(this ReferenceHub player, uint time, string message, bool monospace = false) => Map.BroadcastComponent.TargetAddElement(player.scp079PlayerScript.connectionToClient, message, time, monospace);

		/// <summary>
		/// A simple broadcast to a <see cref="ReferenceHub"/>. Doesn't get logged to the console.
		/// </summary>
		[Obsolete("Append ', false' to your broadcasts to use the new, updated method.", true)]
		public static void Broadcast(this ReferenceHub player, uint time, string message) => Map.BroadcastComponent.TargetAddElement(player.scp079PlayerScript.connectionToClient, message, time, false);

		/// <summary>
		/// Clears the brodcast of a <see cref="ReferenceHub"/>. Doesn't get logged to the console.
		/// </summary>
		/// <param name="player"></param>
		public static void ClearBroadcasts(this ReferenceHub player) => Map.BroadcastComponent.TargetClearElements(player.scp079PlayerScript.connectionToClient);

		/// <summary>
		/// Gets the <see cref="Team"/> a <see cref="ReferenceHub"/> belongs to.
		/// </summary>
		/// <param name="player">Player</param>
		/// <returns>Team</returns>
		public static Team GetTeam(this ReferenceHub player) => player.GetRole().GetTeam();

		/// <summary>
		/// Gets the <see cref="Team"/> a <see cref="RoleType"/> belongs to.
		/// </summary>
		/// <param name="roleType"></param>
		/// <returns></returns>
		public static Team GetTeam( this RoleType roleType ) 
		{
			switch(roleType) 
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

		/// <summary>
		/// Gets the <see cref="ReferenceHub">player</see>'s <see cref="Side">side</see> they're currently in.
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		public static Side GetSide( this RoleType type ) => type.GetTeam().GetSide();

		/// <summary>
		/// Gets the <see cref="ReferenceHub">player</see>'s <see cref="Side">side</see> they're currently in.
		/// </summary>
		/// <param name="team"></param>
		/// <returns></returns>
		public static Side GetSide( this Team team) 
			{
			switch(team) 
			{
				case Team.SCP:
					return Side.SCP;
				case Team.MTF:
				case Team.RSC:
					return Side.MTF;
				case Team.CHI:
				case Team.CDP:
					return Side.CHAOS;
				case Team.TUT:
					return Side.TUTORIAL;
				case Team.RIP:
				default: return Side.NONE;
			}
		}

		/// <summary>
		/// Gets the <see cref="ReferenceHub">player</see>'s <see cref="Side">side</see> they're currently in.
		/// </summary>
		/// <param name="hub"></param>
		/// <returns></returns>
		public static Side GetSide( this ReferenceHub hub ) => hub.GetTeam().GetSide();

		/// <summary>
		/// Gets the Reference hub belonging to the GameObject, if any.
		/// </summary>
		/// <param name="player">object</param>
		/// <returns>ReferenceHub or null</returns>
		public static ReferenceHub GetPlayer(this GameObject player) => ReferenceHub.GetHub(player);

		/// <summary>
		/// Gets the reference hub belonging to the player with the specified PlayerID
		/// </summary>
		/// <param name="playerId">PlayerID</param>
		/// <returns>ReferenceHub or null</returns>
		public static ReferenceHub GetPlayer(int playerId)
		{
			if (IdHubs.ContainsKey(playerId))
				return IdHubs[playerId];

			foreach (ReferenceHub hub in GetHubs())
			{
				if (hub.GetPlayerId() == playerId)
				{
					IdHubs.Add(playerId, hub);

					return hub;
				}
			}

			return null;
		}

		/// <summary>
		/// Gets the reference hub belonging to the player who's name most closely matches the string given, if any.
		/// </summary>
		/// <param name="args">Player's Name</param>
		/// <returns>ReferenceHub or null</returns>
		public static ReferenceHub GetPlayer(string args)
		{
			try
			{
				if (StrHubs.ContainsKey(args))
					return StrHubs[args];

				ReferenceHub playerFound = null;

				if (short.TryParse(args, out short playerId))
					return GetPlayer(playerId);

				if (args.EndsWith("@steam") || args.EndsWith("@discord") || args.EndsWith("@northwood") || args.EndsWith("@patreon"))
				{
					Log.Debug("Trying to find by UserID...");

					foreach (ReferenceHub player in GetHubs())
					{
						if (player.GetUserId() == args)
						{
							playerFound = player;

							Log.Debug("Found UserID match.");
						}
					}
				}
				else
				{
					Log.Debug($"Trying to find by name... {args}");

					if (args == "WORLD" || args == "SCP-018" || args == "SCP-575" || args == "SCP-207")
						return null;

					int maxNameLength = 31, lastnameDifference = 31;
					string str1 = args.ToLower();

					foreach (ReferenceHub player in GetHubs())
					{
						if (!player.GetNickname().ToLower().Contains(args.ToLower()))
							continue;

						if (str1.Length < maxNameLength)
						{
							int x = maxNameLength - str1.Length;
							int y = maxNameLength - player.GetNickname().Length;
							string str2 = player.GetNickname();

							for (int i = 0; i < x; i++) str1 += "z";

							for (int i = 0; i < y; i++) str2 += "z";

							int nameDifference = LevenshteinDistance.Compute(str1, str2);
							if (nameDifference < lastnameDifference)
							{
								lastnameDifference = nameDifference;
								playerFound = player;

								Log.Debug("Found name match.");
							}
						}
					}
				}

				if (playerFound != null)
					StrHubs.Add(args, playerFound);

				return playerFound;
			}
			catch (Exception exception)
			{
				Log.Error($"GetPlayer error: {exception}");
				return null;
			}
		}

		/// <summary>
		/// Get the global badge of the player.
		/// </summary>
		/// <param name="player">Player's ReferenceHub</param>
		/// <returns>GlobalBadge or null</returns>
		public static GlobalBadge GetGlobalBadge(this ReferenceHub player)
		{
			string token = player.serverRoles.NetworkGlobalBadge;
			if (string.IsNullOrEmpty(token)) { return null; }
			Dictionary<string, string> dictionary = (from rwr in token.Split(new string[]
		   {
						   "<br>"
		  }, StringSplitOptions.None)
													 select rwr.Split(new string[]
													 {
						   ": "
													 }, StringSplitOptions.None)).ToDictionary((string[] split) => split[0], (string[] split) => split[1]);

			int BadgeType = 0;
			if (int.TryParse(dictionary["Badge type"], out int type)) { BadgeType = type; }

			return new GlobalBadge
			{
				BadgeText = dictionary["Badge text"],
				BadgeColor = dictionary["Badge color"],
				Type = BadgeType
			};
		}

		/// <summary>
		/// Get the current room a player are in (from Smod2).
		/// </summary>
		/// <param name="player">Player's ReferenceHub</param>
		/// <returns>Transform or null</returns>

		public static Room GetCurrentRoom(this ReferenceHub player)
		{
			Vector3 playerPos = player.GetPosition();
			Vector3 end = playerPos - new Vector3(0f, 10f, 0f);
			bool flag = Physics.Linecast(playerPos, end, out RaycastHit raycastHit, -84058629);

			if (!flag || raycastHit.transform == null)
				return null;

			Transform transform = raycastHit.transform;

			while (transform.parent != null && transform.parent.parent != null)
				transform = transform.parent;

			foreach (Room room in Map.Rooms)
				if (room.Position == transform.position)
					return room;

			return new Room
			{
				Name = transform.name,
				Position = transform.position,
				Transform = transform
			};
		}

		/// <summary>
		/// Mutes a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		public static void Mute(this ReferenceHub player) => player.characterClassManager.NetworkMuted = true;

		/// <summary>
		/// Unmutes a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		public static void Unmute(this ReferenceHub player) => player.characterClassManager.NetworkMuted = false;

		/// <summary>
		/// Gets a <see cref="ReferenceHub">player</see> mute status.
		/// </summary>
		/// <param name="player"></param>
		/// <returns>True if muted, false if not</returns>
		public static bool IsMuted(this ReferenceHub player) => player.characterClassManager.NetworkMuted;

		/// <summary>
		/// Intercom mutes a <see cref="ReferenceHub">player</see> mute status.
		/// </summary>
		/// <param name="player"></param>
		public static void IntercomMute(this ReferenceHub player) => player.characterClassManager.NetworkIntercomMuted = true;

		/// <summary>
		/// Intercom unmutes a <see cref="ReferenceHub">player</see> mute status.
		/// </summary>
		/// <param name="player"></param>
		/// <returns>True if intercom muted, false if not</returns>
		public static void IntercomUnmute(this ReferenceHub player) => player.characterClassManager.NetworkIntercomMuted = false;

		/// <summary>
		/// Gets a <see cref="ReferenceHub">player</see> intercom mute status.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static bool IsIntercomMuted(this ReferenceHub player) => player.characterClassManager.NetworkIntercomMuted;

		/// <summary>
		/// Gets a <see cref="ReferenceHub">player</see> host status.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static bool IsHost(this ReferenceHub player) => player.characterClassManager.IsHost;

		/// <summary>
		/// Gets the GodMode status of a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		public static bool GetGodMode(this ReferenceHub player) => player.characterClassManager.GodMode;

		/// <summary>
		/// Sets the GodMode status of a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="isEnabled"></param>
		public static void SetGodMode(this ReferenceHub player, bool isEnabled) => player.characterClassManager.GodMode = isEnabled;

		/// <summary>
		/// Gets the health of a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player">Player</param>
		/// <returns></returns>
		public static float GetHealth(this ReferenceHub player) => player.playerStats.health;

		/// <summary>
		/// Sets the health of a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player">Player</param>
		/// <param name="amount">Health amount</param>
		public static void SetHealth(this ReferenceHub player, float amount) => player.playerStats.health = amount;

		/// <summary>
		/// Adds the specified amount of health to a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="amount"></param>
		public static void AddHealth(this ReferenceHub player, float amount) => player.playerStats.health += amount;
		
		/// <summary>
		/// Adds the specified amount of health to a <see cref="ReferenceHub">player</see> without exceeding the maximum health value.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="amount"></param>
		public static void Heal(this ReferenceHub player, float amount) => player.playerStats.health = Mathf.Clamp(player.playerStats.health + amount, 1, player.playerStats.maxHP);
		
		/// <summary>
		/// Set the current amount of health of a <see cref="ReferenceHub">player</see> to their maximum amount of health.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="amount"></param>
		public static void Heal(this ReferenceHub player) => player.playerStats.health = player.playerStats.maxHP;
		
		/// <summary>
		/// Gets the maximum amount of health of a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		/// <returns>float</returns>
		public static float GetMaxHealth(this ReferenceHub player) => player.playerStats.maxHP;

		/// <summary>
		/// Sets the maximum amount of health of a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		/// <returns>float</returns>
		public static void SetMaxHealth(this ReferenceHub player, float amount) => player.playerStats.maxHP = (int)amount;

		/// <summary>
		/// Gets the adrenaline health of a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static byte GetAdrenalineHealth(this ReferenceHub player) => (byte)player.playerStats.unsyncedArtificialHealth;

		/// <summary>
		/// Sets the adrenaline health of a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static void SetAdrenalineHealth(this ReferenceHub player, byte amount) => player.playerStats.unsyncedArtificialHealth = amount;

		/// <summary>
		/// Adds the specified amount of adrenaline health to a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="amount"></param>
		/// 
		[Obsolete("Use AddAdrenalineHealth instead.", true)]
		public static void AddArtificialHealth(this ReferenceHub player, byte amount) => AddAdrenalineHealth(player, amount);

		/// <summary>
		/// Adds the specified amount of adrenaline health to a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="amount"></param>
		public static void AddAdrenalineHealth(this ReferenceHub player, byte amount) => player.playerStats.unsyncedArtificialHealth += amount;

		/// <summary>
		/// Gets the maximum amount of adrenaline health of a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static float GetMaxAdrenalineHealth(this ReferenceHub player) => player.playerStats.maxArtificialHealth;

		/// <summary>
		/// Get the item in the player's hand, returns the default value if empty.
		/// </summary>
		/// <param name="player"></param>
		/// <returns>SyncItemInfo or default(SyncItemInfo)</returns>
		public static Inventory.SyncItemInfo GetCurrentItem(this ReferenceHub player) => player.inventory.GetItemInHand();
		/// <summary>
		/// Get a list of all items in a player's inventory. Can be empty.
		/// </summary>
		/// <param name="player"></param>
		/// <returns>List<SyncItemInfo></returns>
		public static List<Inventory.SyncItemInfo> GetAllItems(this ReferenceHub player) => player.inventory.items.ToList();

		/// <summary>
		/// Sets the player's current item in their hand.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="itemType"></param>
		public static void SetCurrentItem(this ReferenceHub player, ItemType itemType) => player.inventory.SetCurItem(itemType);

		/// <summary>
		/// Add an item of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="itemType"></param>
		public static void AddItem(this ReferenceHub player, ItemType itemType) => player.inventory.AddNewItem(itemType);

		/// <summary>
		/// Add an item with the specified info to a player's inventory.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="item"></param>
		public static void AddItem(this ReferenceHub player, Inventory.SyncItemInfo item) => player.inventory.AddNewItem(item.id, item.durability, item.modSight, item.modBarrel, item.modOther);
		
		/// <summary>
		/// Drop an item from the player's inventory.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="item"></param>
		public static void DropItem(this ReferenceHub player, Inventory.SyncItemInfo item)
		{
			player.inventory.SetPickup(item.id, item.durability, player.GetPosition(), player.inventory.camera.transform.rotation, item.modSight, item.modBarrel, item.modOther);
			player.inventory.items.Remove(item);
		}

		/// <summary>
		/// Remove an item from the player's inventory.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="item"></param>
		public static void RemoveItem(this ReferenceHub player, Inventory.SyncItemInfo item) => player.inventory.items.Remove(item);

		/// <summary>
		/// Sets the player's inventory to the provided list of items, clearing any items they already possess.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="items"></param>
		public static void SetInventory(this ReferenceHub player, List<Inventory.SyncItemInfo> items)
		{
			player.ClearInventory();

			foreach (Inventory.SyncItemInfo item in items)
				player.inventory.AddNewItem(item.id, item.durability, item.modSight, item.modBarrel, item.modOther);
		}

		/// <summary>
		/// Clears a player's inventory.
		/// </summary>
		/// <param name="player"></param>

		public static void ClearInventory(this ReferenceHub player) => player.inventory.items.Clear();

		/// <summary>
		/// Gets the reloading status of a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		public static bool IsReloading(this ReferenceHub player) => player.weaponManager.IsReloading();

		/// <summary>
		/// Gets the zooming status of a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		public static bool IsZooming(this ReferenceHub player) => player.weaponManager.ZoomInProgress();

		/// <summary>
		/// Sets the amount of a specified <see cref="AmmoType">ammo type</see>.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="ammoType"></param>
		/// <param name="amount"></param>
		public static void SetAmmo(this ReferenceHub player, AmmoType ammoType, int amount) => player.ammoBox.SetOneAmount((int)ammoType, amount.ToString());

		/// <summary>
		/// Gets the amount of a specified <see cref="AmmoType">ammo type</see>.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="ammoType"></param>
		public static int GetAmmo(this ReferenceHub player, AmmoType ammoType) => player.ammoBox.GetAmmo((int)ammoType);

		/// <summary>
		/// Bans a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="duration"></param>
		/// <param name="reason"></param>
		/// <param name="issuer"></param>
		public static void BanPlayer(this ReferenceHub player, int duration, string reason, string issuer = "Console") => player.gameObject.BanPlayer(duration, reason, issuer);

		/// <summary>
		/// Bans a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="duration"></param>
		/// <param name="reason"></param>
		/// <param name="issuer"></param>
		public static void BanPlayer(this GameObject player, int duration, string reason, string issuer = "Console") => PlayerManager.localPlayer.GetComponent<BanPlayer>().BanUser(player, duration, reason, issuer, false);

		/// <summary>
		/// Kicks a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="reason"></param>
		/// <param name="issuer"></param>
		public static void KickPlayer(this ReferenceHub player, string reason, string issuer = "Console") => player.BanPlayer(0, reason, issuer);

		/// <summary>
		/// Kicks a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="reason"></param>
		/// <param name="issuer"></param>
		public static void KickPlayer(this GameObject player, string reason, string issuer = "Console") => player.BanPlayer(0, reason, issuer);

		/// <summary>
		/// Handcuff a player by another player.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="target"></param>
		public static void HandcuffPlayer(this ReferenceHub player, ReferenceHub target)
		{
			Handcuffs handcuffs = target.handcuffs;

			if (handcuffs == null) { return; }

			if (handcuffs.CufferId < 0 && player.inventory.items.Any((Inventory.SyncItemInfo item) => item.id == ItemType.Disarmer) && Vector3.Distance(player.transform.position, target.transform.position) <= 130f)
			{
				handcuffs.NetworkCufferId = player.GetPlayerId();
			}
		}

		/// <summary>
		/// Uncuff the player.
		/// </summary>
		/// <param name="player"></param>
		public static void UncuffPlayer(this ReferenceHub player) => player.handcuffs.NetworkCufferId = -1;
		
		/// <summary>
		/// Returns true if the player is handcuffed.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static bool IsHandCuffed(this ReferenceHub player) => player.handcuffs.CufferId != -1;

		/// <summary>
		/// Returns the handcuffer of the player, returns null if no cuffer.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static bool GetHandCuffer(this ReferenceHub player) => GetPlayer(player.handcuffs.CufferId);

		/// <summary>
		/// Returns the IP address of the player.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static string GetIpAddress(this ReferenceHub player) => player.queryProcessor._ipAddress;

		/// <summary>
		/// Sets the <see cref="ReferenceHub">player</see>'s scale.
		/// </summary>
		/// <param name="player"></param>
		public static void SetScale(this ReferenceHub player, float scale) => player.SetScale(Vector3.one * scale);

		/// <summary>
		/// Sets the <see cref="ReferenceHub">player</see>'s scale.
		/// </summary>
		/// <param name="player"></param>
		public static void SetScale(this ReferenceHub player, Vector3 scale) => player.SetScale(scale.x, scale.y, scale.z);

		/// <summary>
		/// Sets the <see cref="ReferenceHub">player</see>'s scale.
		/// </summary>
		/// <param name="player"></param>
		public static void SetScale(this ReferenceHub player, float x, float y, float z)
		{
			try
			{
				player.transform.localScale = new Vector3(x, y, z);

				foreach (ReferenceHub target in GetHubs())
					SendSpawnMessage?.Invoke(null, new object[] { player.GetComponent<NetworkIdentity>(), target.GetConnection() });
			}
			catch (Exception exception)
			{
				Log.Error($"SetScale error: {exception}");
			}
		}

		/// <summary>
		/// Gets the <see cref="ReferenceHub">player</see>'s scale.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static Vector3 GetScale(this ReferenceHub player) => player.transform.localScale;

		/// <summary>
		/// Gets the <see cref="NetworkConnection">connection</see> of a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		public static NetworkConnection GetConnection(this ReferenceHub player) => player.scp079PlayerScript.connectionToClient;

		/// <summary>
		/// Disconnects a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		public static void Disconnect(this ReferenceHub player, string reason = null) => ServerConsole.Disconnect(player.gameObject, string.IsNullOrEmpty(reason) ? "" : reason);

		/// <summary>
		/// Kills a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="player"></param>
		public static void Kill(this ReferenceHub player, DamageTypes.DamageType damageType = default) => player.playerStats.HurtPlayer(new PlayerStats.HitInfo(-1f, "WORLD", damageType, 0), player.gameObject);

		/// <summary>
		/// Gets a list of <see cref="ReferenceHub">player</see>s, filtered by <see cref="Team">team</see>.
		/// </summary>
		/// <param name="team"></param>
		/// <returns></returns>
		public static IEnumerable<ReferenceHub> GetHubs(this Team team) => GetHubs().Where(player => player.GetTeam() == team);

		/// <summary>
		/// Gets a list of <see cref="ReferenceHub">player</see>s, filtered by <see cref="RoleType">team</see>.
		/// </summary>
		/// <param name="role"></param>
		/// <returns></returns>
		public static IEnumerable<ReferenceHub> GetHubs(this RoleType role) => GetHubs().Where(player => player.GetRole() == role);

		/// <summary>
		/// Gets the group name of a <see cref="ReferenceHub"/>.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static string GetGroupName(this ReferenceHub player) => ServerStatic.PermissionsHandler._members[player.GetUserId()];

		/// <summary>
		/// Gets the bypass mode status of a <see cref="ReferenceHub"/>.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
		public static bool GetBypassMode(this ReferenceHub player) => player.serverRoles.BypassMode;

		/// <summary>
		/// Sets the bypass mode status of a <see cref="ReferenceHub"/>.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="isEnabled"></param>
		public static void SetBypassMode(this ReferenceHub player, bool isEnabled) => player.serverRoles.BypassMode = isEnabled;

		/// <summary>
		/// Sends a console message to a <see cref="ReferenceHub"/>
		/// </summary>
		/// <param name="player"></param>
		/// <param name="message"></param>
		/// <param name="color"></param>
		public static void SendConsoleMessage(this ReferenceHub player, string message, string color)
		{
			player.characterClassManager.TargetConsolePrint(player.GetConnection(), message, color);
		}

		/// <summary>
		/// Sets the players Friendly Fire value.
		/// Note: This only allows them to DEAL FF damage, not TAKE FF damage.
		/// </summary>
		/// <param name="player"></param>
		/// <param name="value"></param>
		public static void SetFriendlyFire(this ReferenceHub player, bool value) =>
			player.weaponManager.NetworkfriendlyFire = value;
		
		/// <summary>
		/// Gets the badge name of a <see cref="ReferenceHub"/>.
		/// </summary>
		/// <param name="player"></param>
		/// <returns></returns>
        	public static string GetBadgeName(this ReferenceHub rh) => rh.serverRoles.Group.BadgeText;
	}
}
