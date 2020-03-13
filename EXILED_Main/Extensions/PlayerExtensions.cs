using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using EXILED.ApiObjects;
using Mirror;
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

        /// <summary>
        /// Mutes a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="rh"></param>
        public static void Mute(this ReferenceHub rh) => rh.characterClassManager.NetworkMuted = true;

        /// <summary>
        /// Unmutes a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="rh"></param>
        public static void Unmute(this ReferenceHub rh) => rh.characterClassManager.NetworkMuted = false;

        /// <summary>
        /// Gets a <see cref="ReferenceHub">player</see> mute status.
        /// </summary>
        /// <param name="rh"></param>
        /// <returns>True if muted, false if not</returns>
        public static bool IsMuted(this ReferenceHub rh) => rh.characterClassManager.NetworkMuted;

        /// <summary>
        /// Intercom mutes a <see cref="ReferenceHub">player</see> mute status.
        /// </summary>
        /// <param name="rh"></param>
        public static void IntercomMute(this ReferenceHub rh) => rh.characterClassManager.NetworkIntercomMuted = true;

        /// <summary>
        /// Intercom unmutes a <see cref="ReferenceHub">player</see> mute status.
        /// </summary>
        /// <param name="rh"></param>
        /// <returns>True if intercom muted, false if not</returns>
        public static void IntercomUnmute(this ReferenceHub rh) => rh.characterClassManager.NetworkIntercomMuted = false;

        /// <summary>
        /// Gets a <see cref="ReferenceHub">player</see> intercom mute status.
        /// </summary>
        /// <param name="rh"></param>
        /// <returns></returns>
        public static bool IsIntercomMuted(this ReferenceHub rh) => rh.characterClassManager.NetworkIntercomMuted;

        /// <summary>
        /// Gets the GodMode status of a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="rh"></param>
        /// <param name="newStatus"></param>
        public static bool GetGodMode(this ReferenceHub rh) => rh.characterClassManager.GodMode;

        /// <summary>
        /// Sets the GodMode status of a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="rh"></param>
        /// <param name="newStatus"></param>
        public static void SetGodMode(this ReferenceHub rh, bool newStatus) => rh.characterClassManager.GodMode = newStatus;

        /// <summary>
        /// Gets the health of a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="rh">Player</param>
        /// <returns></returns>
        public static float GetHealth(this ReferenceHub rh) => rh.playerStats.health;

		/// <summary>
		/// Sets the health of a <see cref="ReferenceHub">player</see>.
		/// </summary>
		/// <param name="rh">Player</param>
		/// <param name="amount">Health amount</param>
		public static void SetHealth(this ReferenceHub rh, float amount) => rh.playerStats.health = amount;

        /// <summary>
        /// Adds the specified amount of health to a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="rh"></param>
        /// <param name="amount"></param>
        public static void AddHealth(this ReferenceHub rh, float amount) => rh.playerStats.health += amount;

        /// <summary>
        /// Gets the maximum amount of health of a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="rh"></param>
        /// <returns>float</returns>
        public static float GetMaxHealth(this ReferenceHub rh) => rh.playerStats.maxHP;

        /// <summary>
        /// Sets the maximum amount of health of a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="rh"></param>
        /// <returns>float</returns>
        public static void SetMaxHealth(this ReferenceHub rh, float amount) => rh.playerStats.maxHP = (int)amount;

        /// <summary>
        /// Gets the adrenaline health of a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="rh"></param>
        /// <returns></returns>
        public static byte GetAdrenalineHealth(this ReferenceHub rh) => rh.playerStats.syncArtificialHealth;

        /// <summary>
        /// Sets the adrenaline health of a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="rh"></param>
        /// <returns></returns>
        public static void SetAdrenalineHealth(this ReferenceHub rh, byte amount) => rh.playerStats.syncArtificialHealth = amount;

        /// <summary>
        /// Adds the specified amount of adrenaline health to a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="rh"></param>
        /// <param name="amount"></param>
        /// 
        [Obsolete("Use AddAdrenalineHealth instead.", true)]
        public static void AddArtificialHealth(this ReferenceHub rh, byte amount) => AddAdrenalineHealth(rh, amount);

        /// <summary>
        /// Adds the specified amount of adrenaline health to a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="rh"></param>
        /// <param name="amount"></param>
        public static void AddAdrenalineHealth(this ReferenceHub rh, byte amount) => rh.playerStats.syncArtificialHealth += amount;

        /// <summary>
        /// Gets the maximum amount of adrenaline health of a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="rh"></param>
        /// <returns></returns>
        public static float GetMaxAdrenalineHealth(this ReferenceHub rh) => rh.playerStats.maxArtificialHealth;

		/// <summary>
		/// Get the item in the player's hand, returns the default value if empty.
		/// </summary>
		/// <param name="rh"></param>
		/// <returns>SyncItemInfo or default(SyncItemInfo)</returns>
		public static Inventory.SyncItemInfo GetCurrentItem(this ReferenceHub rh) => rh.inventory.GetItemInHand();
		/// <summary>
		/// Get a list of all items in a player's inventory. Can be empty.
		/// </summary>
		/// <param name="rh"></param>
		/// <returns>List<SyncItemInfo></returns>
		public static List<Inventory.SyncItemInfo> GetAllItems(this ReferenceHub rh) => rh.inventory.items.ToList();

		/// <summary>
		/// Sets the player's current item in their hand.
		/// </summary>
		/// <param name="rh"></param>
		/// <param name="type"></param>
		public static void SetCurrentItem(this ReferenceHub rh, ItemType type) => rh.inventory.SetCurItem(type);

		/// <summary>
		/// Add an item of the specified type with default durability(ammo/charge) and no mods to the player's inventory.
		/// </summary>
		/// <param name="rh"></param>
		/// <param name="type"></param>
		public static void AddItem(this ReferenceHub rh, ItemType type) => rh.inventory.AddNewItem(type);
		/// <summary>
		/// Add an item with the specified info to a player's inventory.
		/// </summary>
		/// <param name="rh"></param>
		/// <param name="info"></param>
		public static void AddItem(this ReferenceHub rh, Inventory.SyncItemInfo info) => rh.inventory.AddNewItem(info.id, info.durability, info.modSight, info.modBarrel, info.modOther);

		/// <summary>
		/// Sets the player's inventory to the provided list of items, clearing any items they already possess.
		/// </summary>
		/// <param name="rh"></param>
		/// <param name="items"></param>
		public static void SetInventory(this ReferenceHub rh, List<Inventory.SyncItemInfo> items)
		{
			rh.ClearInventory();
			foreach (Inventory.SyncItemInfo item in items)
				rh.inventory.AddNewItem(item.id, item.durability, item.modSight, item.modBarrel, item.modOther);
		}
		
		/// <summary>
		/// Clears a player's inventory.
		/// </summary>
		/// <param name="rh"></param>

		public static void ClearInventory(this ReferenceHub rh) => rh.inventory.items.Clear();

        /// <summary>
        /// Gets the reloading status of a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="rh"></param>
        public static bool IsReloading(this ReferenceHub rh) => rh.weaponManager.IsReloading();

        /// <summary>
        /// Gets the zooming status of a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="rh"></param>
        public static bool IsZooming(this ReferenceHub rh) => rh.weaponManager.ZoomInProgress();

        /// <summary>
        /// Sets the amount of a specified <see cref="AmmoType">ammo type</see>.
        /// </summary>
        /// <param name="rh"></param>
        /// <param name="ammoType"></param>
        /// <param name="amount"></param>
        public static void SetAmmo(this ReferenceHub rh, AmmoType ammoType, int amount) => rh.ammoBox.SetOneAmount((int)ammoType, amount.ToString());

        /// <summary>
        /// Gets the amount of a specified <see cref="AmmoType">ammo type</see>.
        /// </summary>
        /// <param name="rh"></param>
        /// <param name="ammoType"></param>
        public static int GetAmmo(this ReferenceHub rh, AmmoType ammoType) => rh.ammoBox.GetAmmo((int)ammoType);

        /// <summary>
        /// Bans a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="hub"></param>
        /// <param name="dur"></param>
        /// <param name="reason"></param>
        /// <param name="issuer"></param>
        public static void BanPlayer(this ReferenceHub hub, int dur, string reason, string issuer = "Console") => hub.gameObject.BanPlayer(dur, reason, issuer);

        /// <summary>
        /// Bans a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="dur"></param>
        /// <param name="reason"></param>
        /// <param name="issuer"></param>
        public static void BanPlayer(this GameObject obj, int dur, string reason, string issuer = "Console") => PlayerManager.localPlayer.GetComponent<BanPlayer>().BanUser(obj, dur, reason, issuer, false);

        /// <summary>
        /// Kicks a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="hub"></param>
        /// <param name="reason"></param>
        /// <param name="issuer"></param>
        public static void KickPlayer(this ReferenceHub hub, string reason, string issuer = "Console") => hub.BanPlayer(0, reason, issuer);

        /// <summary>
        /// Kicks a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="reason"></param>
        /// <param name="issuer"></param>
        public static void KickPlayer(this GameObject obj, string reason, string issuer = "Console") => obj.BanPlayer(0, reason, issuer);

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

                foreach (GameObject target in PlayerManager.players)
                {
                    MethodInfo sendSpawnMessage = typeof(NetworkServer).GetMethod("SendSpawnMessage", BindingFlags.Instance
                        | BindingFlags.InvokeMethod | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Public);

                    sendSpawnMessage?.Invoke(null, new object[] { player.GetComponent<NetworkIdentity>(), target.GetComponent<NetworkIdentity>().connectionToClient });
                }
            }
            catch (Exception exception)
            {
                Log.Info($"Set Scale error: {exception}");
            }
        }

        /// <summary>
        /// Gets the <see cref="ReferenceHub">player</see>'s scale.
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public static Vector3 GetScale(this ReferenceHub player) => player.transform.localScale;

        /// <summary>
        /// Disconnects a <see cref="ReferenceHub">player</see>.
        /// </summary>
        /// <param name="player"></param>
        public static void Disconnect(this ReferenceHub player)
        {
            player.scp079PlayerScript.connectionToClient.Disconnect();
            player.scp079PlayerScript.connectionToClient.Dispose();
        }

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
        public static List<ReferenceHub> GetPlayers(this Team team)
        {
            List<ReferenceHub> playersByTeam = new List<ReferenceHub>();

            foreach (var player in GetHubs())
            {
                if (player.GetTeam() == team)
                    playersByTeam.Add(player);
            }

            return playersByTeam;
        }

        /// <summary>
        /// Gets a list of <see cref="ReferenceHub">player</see>s, filtered by <see cref="RoleType">team</see>.
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public static IEnumerable<ReferenceHub> GetPlayers(this RoleType role)
        {
            List<ReferenceHub> playersByRole = new List<ReferenceHub>();

            foreach (var player in GetHubs())
            {
                if (player.GetRole() == role)
                    playersByRole.Add(player);
            }

            return playersByRole;
        }
    }
}
