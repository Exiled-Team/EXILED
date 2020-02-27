using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EXILED.ApiObjects;
using UnityEngine;

namespace EXILED.Extensions
{
    public static class Player
    {
        public static Dictionary<int, ReferenceHub> IdHubs = new Dictionary<int, ReferenceHub>();
        public static Dictionary<string, ReferenceHub> StrHubs = new Dictionary<string, ReferenceHub>();
        
        /// <summary>
        /// Returns an IEnumerable of ReferenceHubs for all players on the server.
        /// </summary>
        /// <returns>IEnumerable(ReferenceHub)</returns>
        public static IEnumerable<ReferenceHub> GetHubs() => ReferenceHub.Hubs.Values.Where(h => !h.characterClassManager.IsHost);

        /// <summary>
        /// Gets a player's UserID
        /// </summary>
        /// <param name="rh">Player</param>
        /// <returns>string, can be empty.</returns>
        public static string GetUserId(this ReferenceHub rh) => rh.characterClassManager.UserId;

        /// <summary>
        /// Sets a player's UserID
        /// </summary>
        /// <param name="rh"></param>
        /// <param name="newId"></param>
        /// <returns></returns>
        public static void SetUserId(this ReferenceHub rh, string newId) =>
	        rh.characterClassManager.NetworkSyncedUserId = newId;

        /// <summary>
        /// Gets a player's PlayerID
        /// </summary>
        /// <param name="rh">Player</param>
        /// <returns>int PlayerID</returns>
        public static int GetPlayerId(this ReferenceHub rh) => rh.queryProcessor.PlayerId;

        /// <summary>
        /// Sets a player's PlayerID
        /// </summary>
        /// <param name="rh"></param>
        /// <param name="newId"></param>
        /// <returns></returns>
        public static void SetPlayerId(this ReferenceHub rh, int newId) => rh.queryProcessor.NetworkPlayerId = newId;
        
        /// <summary>
        /// Gets a player's Overwatch status.
        /// </summary>
        /// <param name="rh">Player</param>
        /// <returns>True if in overwatch.</returns>
        public static bool GetOverwatch(this ReferenceHub rh) => rh.serverRoles.OverwatchEnabled;

        /// <summary>
        /// Sets a player's Overwatch status.
        /// </summary>
        /// <param name="rh"></param>
        /// <param name="newStatus"></param>
        /// <returns></returns>
        public static void SetOverwatch(this ReferenceHub rh, bool newStatus) =>
	        rh.serverRoles.OverwatchEnabled = newStatus;
        
        /// <summary>
        /// Gets a player's Current Role.
        /// </summary>
        /// <param name="rh">Player</param>
        /// <returns>RoleType Player's role</returns>
        public static RoleType GetRole(this ReferenceHub rh) => rh.characterClassManager.CurClass;

        /// <summary>
        /// Sets a player's role.
        /// </summary>
        /// <param name="rh"></param>
        /// <param name="newRole"></param>
        public static void SetRole(this ReferenceHub rh, RoleType newRole) =>
	        rh.characterClassManager.SetClassID(newRole);

        /// <summary>
        /// Gets the position of a <see cref="ReferenceHub">player</see>
        /// </summary>
        public static Vector3 GetPosition(this ReferenceHub rh) => rh.plyMovementSync.GetRealPosition();
        /// <summary>
        /// Gets the rotations from a <see cref="ReferenceHub">player</see>
        /// </summary>
        /// <returns>A <see cref="Vector2"/>, representing the directions he's looking at</returns>
        public static Vector2 GetRotations(this ReferenceHub rh) => rh.plyMovementSync.NetworkRotations;
        /// <summary>
        /// Gets the rotation of a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <returns>The direction he's looking at, useful for Raycasts</returns>
        public static Vector3 GetRotationVector(this ReferenceHub rh) => rh.characterClassManager.Scp049.plyCam.transform.forward;
        /// <summary>
        /// Sets the position of a <see cref="ReferenceHub">player</see> using a <see cref="Vector3"/>.
        /// </summary>
        public static void SetPosition(this ReferenceHub rh, Vector3 position) => rh.plyMovementSync.OverridePosition(position, rh.transform.rotation.eulerAngles.y);
        /// <summary>
        /// Sets the position of a <see cref="ReferenceHub">player</see> using the x, y, and z of the destination position.
        /// </summary>
        public static void SetPosition(this ReferenceHub rh, float x, float y, float z) => rh.plyMovementSync.OverridePosition(new Vector3(x, y, z), rh.transform.rotation.eulerAngles.y);
        /// <summary>
        /// Sets the rotation of a <see cref="ReferenceHub">player</see> using a <see cref="Vector2"/>.
        /// </summary>
        public static void SetRotation(this ReferenceHub rh, Vector2 rotations) => rh.plyMovementSync.NetworkRotations = rotations;
        /// <summary>
        /// Sets the rotation of a <see cref="ReferenceHub">player</see> using the x and y values of the desired rotation.
        /// </summary>
        public static void SetRotation(this ReferenceHub rh, float x, float y) => rh.plyMovementSync.NetworkRotations = new Vector2(x, y);
        /// <summary>
        /// Sets the rank of a <see cref="ReferenceHub">player</see> to a <see cref="UserGroup"/>.
        /// </summary>
        public static UserGroup GetRank(this ReferenceHub rh) => rh.serverRoles.Group;
        /// <summary>
        /// Sets the rank color of a <see cref="ReferenceHub">player</see> to a given color with a <see cref="string"/>.
        /// </summary>
        public static void SetRankColor(this ReferenceHub rh, string color) => rh.serverRoles.SetColor(color);
        /// <summary>
        /// Sets the rank name of a <see cref="ReferenceHub">player</see> to a given <see cref="string"/>.
        /// </summary>
        public static void SetRankName(this ReferenceHub rh, string name) => rh.serverRoles.SetText(name);
        /// <summary>
        /// Sets the rank of a <see cref="ReferenceHub">player</see> by giving a <paramref name="name"/>, <paramref name="color"/>, and setting if it should be shown with <paramref name="show"/>.
        /// </summary>
        public static void SetRank(this ReferenceHub rh, string name, string color, bool show)
        {
            // Developer note: I bet I just needed to use the show once. But hey, better be safe than sorry.
            UserGroup ug = new UserGroup()
            {
                BadgeColor = color,
                BadgeText = name,
                HiddenByDefault = !show,
                Cover = show
            };
            rh.serverRoles.SetGroup(ug, false, false, show);
        }
        /// <summary>
        /// Sets the rank of a <see cref="ReferenceHub"/> to a <see cref="UserGroup"/>.
        /// </summary>
        public static void SetRank(this ReferenceHub rh, UserGroup userGroup) => rh.serverRoles.SetGroup(userGroup, false, false, false);
        
        /// <summary>
        /// Gets the nickname of a <see cref="ReferenceHub">player</see>
        /// </summary>
        public static string GetNickname(this ReferenceHub rh) => rh.nicknameSync.Network_myNickSync;

        /// <summary>
        /// Sets the nickname of a <see cref="ReferenceHub"/> to <paramref name="name"/>
        /// </summary>
        public static void SetNickname(this ReferenceHub rh, string name)
        {
            rh.nicknameSync.Network_myNickSync = name;
            MEC.Timing.RunCoroutine(BlinkTag(rh));
        }
        private static IEnumerator<float> BlinkTag(ReferenceHub rh)
        {
            yield return MEC.Timing.WaitForOneFrame;
            rh.characterClassManager.CallCmdRequestHideTag();
            yield return MEC.Timing.WaitForOneFrame;
            rh.characterClassManager.CallCmdRequestShowTag(false);
        }
        /// <summary>
        /// Gives an item to the specified player.
        /// </summary>
        /// <param name="itemType">Your <see cref="ItemType"/></param>
        /// <param name="dur">The durability (most of the times ammo) of the item</param>
        /// <param name="sight">0 is no sight, 1 is the first sight in the Weapon Manager</param>
        /// <param name="barrel">0 is no custom barrel, 1 is the first barrel in the Weapon Manager</param>
        /// <param name="other">0 is no extra attachment, other numbers are the ammo counter, flashlight, etc.</param>
        public static void GiveItem(this ReferenceHub rh, ItemType itemType, float dur = float.NegativeInfinity, int sight = 0, int barrel = 0, int other = 0) => rh.inventory.AddNewItem(itemType, dur, sight, barrel, other);

        // Adapted from https://github.com/galaxy119/SamplePlugin/blob/master/SamplePlugin/Extensions.cs
        /// <summary>
        /// Sends a message to the RA console of a <paramref name="sender"/>. <paramref name="pluginName"/> will be the name of your <see cref="Assembly"/>
        /// </summary>
        public static void RAMessage(this CommandSender sender, string message, bool success = true, string pluginName = null) =>
            sender.RaReply((pluginName ?? Assembly.GetCallingAssembly().GetName().Name) + "#" + message, success, true, string.Empty);
        /// <summary>
        /// A simple broadcast to a player. Doesn't get logged to the console and can be monospace.
        /// </summary>
        public static void Broadcast(this ReferenceHub rh, uint time, string message, bool monospace = false) => Map.BroadcastComponent.TargetAddElement(rh.scp079PlayerScript.connectionToClient, message, time, monospace);
        /// <summary>
        /// A simple broadcast to a player. Doesn't get logged to the console.
        /// </summary>
        [Obsolete("Append ', false' to your broadcasts to use the new, updated method.", true)]
        public static void Broadcast(this ReferenceHub rh, uint time, string message) => Map.BroadcastComponent.TargetAddElement(rh.scp079PlayerScript.connectionToClient, message, time, false);
        /// <summary>
        /// Clears the brodcast of a player. Doesn't get logged to the console.
        /// </summary>
        /// <param name="rh"></param>
        public static void ClearBroadcasts(this ReferenceHub rh) => Map.BroadcastComponent.TargetClearElements(rh.scp079PlayerScript.connectionToClient);
        /// <summary>
        /// Gets the team a player belongs to.
        /// </summary>
        /// <param name="hub">Player</param>
        /// <returns>Team</returns>
        public static Team GetTeam(this ReferenceHub hub)
        {
	        switch (hub.characterClassManager.CurClass)
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
        /// Gets the Reference hub belonging to the GameObject, if any.
        /// </summary>
        /// <param name="obj">object</param>
        /// <returns>ReferenceHub or null</returns>
        public static ReferenceHub GetPlayer(this GameObject obj) => ReferenceHub.GetHub(obj);

        /// <summary>
        /// Gets the reference hub belonging to the player with the specified PlayerID
        /// </summary>
        /// <param name="pId">PlayerID</param>
        /// <returns>ReferenceHub or null</returns>
        public static ReferenceHub GetPlayer(int pId)
		{
			if (IdHubs.ContainsKey(pId))
				return IdHubs[pId];
			foreach (ReferenceHub hub in GetHubs())
				if (hub.queryProcessor.PlayerId == pId)
				{
					IdHubs.Add(pId, hub);
					return hub;
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
				
				GameObject ply = null;
				if (short.TryParse(args, out short pID))
				{
					return GetPlayer(pID);
				}

				if (args.EndsWith("@steam") || args.EndsWith("@discord") || args.EndsWith("@northwood") ||
				    args.EndsWith("@patreon"))
				{
					Log.Debug("Trying to find by SID..");
					foreach (GameObject pl in PlayerManager.players)
						if (pl.GetComponent<ReferenceHub>()?.characterClassManager.UserId == args)
						{
							ply = pl;
							Log.Debug("Found SID match.");
						}
				}
				else
				{
					Log.Debug($"Trying to find by name.. {args}");
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
								Log.Debug("Found name match.");
							}
						}
					}
				}

				ReferenceHub hub = ReferenceHub.GetHub(ply);
				if (hub != null)
					StrHubs.Add(args, hub);

				return hub;
			}
			catch (Exception)
			{
				return null;
			}
		}
		
		/// <summary>
		/// Get the current room a player are in (from Smod2).
		/// </summary>
		/// <param name="rh">Player's ReferenceHub</param>
		/// <returns>Transform or null</returns>
		private static List<Room> rooms = new List<Room>();
		public static Room GetCurrentRoom(this ReferenceHub rh)
		{
			Vector3 playerPos = rh.plyMovementSync.GetRealPosition();
			Vector3 end = playerPos - new Vector3(0f, 10f, 0f);
			bool flag = Physics.Linecast(playerPos, end, out RaycastHit raycastHit, -84058629);
            if (!flag || raycastHit.transform == null)
            {
                return null;
            }
            Transform transform = raycastHit.transform;
            while (transform.parent != null && transform.parent.parent != null)
            {
                transform = transform.parent;
            }

            foreach (Room room in rooms)
	            if (room.Position == transform.position)
		            return room;
            return new Room
            {
	            Name = transform.name,
	            Position = transform.position,
	            Transform = transform
            };
        }
    }
}
