using EXILED.ApiObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EXILED.Extensions
{
	public static class Map
	{
		private static Inventory _hostInventory;
		private static AlphaWarheadController _alphaWarheadController;
		private static Broadcast _broadcast;
		private static AlphaWarheadNukesitePanel _alphaWarheadNukesitePanel;
		private static DecontaminationLCZ _decontaminationLCZ;
		private static List<Room> _rooms = new List<Room>();

		public static Inventory HostInventory
		{
			get
			{
				if (_hostInventory == null)
					_hostInventory = Player.GetPlayer(PlayerManager.localPlayer).inventory;

				return _hostInventory;
			}
		}

		public static AlphaWarheadController AlphaWarheadController
		{
			get
			{
				if (_alphaWarheadController == null)
					_alphaWarheadController = PlayerManager.localPlayer.GetComponent<AlphaWarheadController>();

				return _alphaWarheadController;
			}
		}

		internal static Broadcast BroadcastComponent
		{
			get
			{
				if (_broadcast == null)
					_broadcast = PlayerManager.localPlayer.GetComponent<Broadcast>();

				return _broadcast;
			}
		}

		internal static AlphaWarheadNukesitePanel AlphaWarheadNukesitePanel
		{
			get
			{
				if (_alphaWarheadNukesitePanel == null)
					_alphaWarheadNukesitePanel = Object.FindObjectOfType<AlphaWarheadNukesitePanel>();

				return _alphaWarheadNukesitePanel;
			}
		}

		internal static DecontaminationLCZ DecontaminationLCZ
		{
			get
			{
				if (_decontaminationLCZ == null)
					_decontaminationLCZ = PlayerManager.localPlayer.GetComponent<DecontaminationLCZ>();

				return _decontaminationLCZ;
			}
		}

		public static List<Room> Rooms
		{
			get
			{
				if (_rooms?.Count == 0)
					_rooms = Object.FindObjectsOfType<Transform>().Where(t => t.CompareTag("Room")).Select(obj => new Room { Name = obj.name, Position = obj.position, Transform = obj }).ToList();

				return _rooms;
			}
		}

		/// <summary>
		/// Spawns an item of type <paramref name="itemType"/> in a desired <paramref name="position"/>.
		/// </summary>
		/// <param name="itemType">The type of the item to be spawned</param>
		/// <param name="durability">The durability (or ammo, depends on the weapon) of the item</param>
		/// <param name="position">Where the item will be spawned</param>
		/// <param name="rotation">The rotation. We recommend you to use <see cref="Quaternion.Euler(float, float, float)"/></param>
		/// <param name="sight">The sight the weapon will have (0 is nothing, 1 is the first sight available in the weapon manager, and so on)</param>
		/// <param name="barrel">The barrel of the weapon (0 is no custom barrel, 1 is the first barrel available, and so on)</param>
		/// <param name="other">Other attachments like flashlight, laser or ammo counter</param>
		/// <returns>The <see cref="Pickup"/></returns>
		public static Pickup SpawnItem(ItemType itemType, float durability, Vector3 position, Quaternion rotation = default, int sight = 0, int barrel = 0, int other = 0)
			=> HostInventory.SetPickup(itemType, durability, position, rotation, sight, barrel, other);

		/// <summary>
		/// Broadcasts a message to all players.
		/// </summary>
		/// <param name="message">What will be broadcasted (supports Unity Rich Text formatting)</param>
		/// <param name="duration">The duration in seconds</param>
		/// <param name="monospace">If the message should be in monospace</param>
		public static void Broadcast(string message, uint duration, bool monospace = false)
			=> BroadcastComponent.RpcAddElement(message, duration, monospace);

		/// <summary>
		/// Clears all players' broadcasts.
		/// </summary>
		public static void ClearBroadcasts() => BroadcastComponent.RpcClearElements();

		/// <summary>
		/// Starts the warhead.
		/// </summary>
		[Obsolete("Use StartNuke.")]
		public static void StartWarhead() => AlphaWarheadController.StartDetonation();

		/// <summary>
		/// Stops the warhead.
		/// </summary>
		[Obsolete("Use StopNuke.")]
		public static void StopWarhead() => AlphaWarheadController.CancelDetonation();

		/// <summary>
		/// Detonates the warhead.
		/// </summary>
		[Obsolete("Use DetonateNuke.")]
		public static void DetonateWarhead() => AlphaWarheadController.Detonate();

		/// <summary>
		/// Starts the nuke.
		/// </summary>
		public static void StartNuke() => AlphaWarheadController.StartDetonation();

		/// <summary>
		/// Stops the nuke.
		/// </summary>
		public static void StopNuke() => AlphaWarheadController.CancelDetonation();

		/// <summary>
		/// Detonates the nuke.
		/// </summary>
		public static void DetonateNuke() => AlphaWarheadController.Detonate();

		/// <summary>
		/// Gets the random spawn point of the indicated role.
		/// </summary>
		/// <param name="roleType">RoleType</param>
		/// <returns>Vector3 spawnPoint</returns>
		public static Vector3 GetRandomSpawnPoint(RoleType roleType)
		{
			GameObject randomPosition = Object.FindObjectOfType<SpawnpointManager>().GetRandomPosition(roleType);

			return randomPosition == null ? Vector3.zero : randomPosition.transform.position;
		}

		/// <summary>
		/// Enable/Disable the nuke lever or gets its status.
		/// </summary>
		[Obsolete("Use Rooms property instead.", true)]
		public static IEnumerable<Room> GetRooms() => Rooms;

		/// <summary>
		/// Gets the nuke lever status.
		/// </summary>
		public static bool IsNukeLeverEnabled
		{
			get => AlphaWarheadNukesitePanel.Networkenabled;
			set => AlphaWarheadNukesitePanel.Networkenabled = value;
		}

		/// <summary>
		/// Gets the nuke detonation status.
		/// </summary>
		public static bool IsNukeDetonated => AlphaWarheadController.detonated;

		/// <summary>
		/// Gets the nuke detonation status.
		/// </summary>
		public static bool IsNukeInProgress => AlphaWarheadController.inProgress;

		/// <summary>
		/// Gets/sets the nuke detonation timer.
		/// </summary>
		public static float NukeDetonationTimer
		{
			get => AlphaWarheadController.NetworktimeToDetonation;
			set => AlphaWarheadController.NetworktimeToDetonation = value;
		}

		/// <summary>
		/// Gets the LCZ decontamination status.
		/// </summary>
		public static bool IsLCZDecontaminated => DecontaminationLCZ.GetCurAnnouncement() > 5;

		/// <summary>
		/// Starts the Decontamination process.
		/// </summary>
		public static void StartDecontamination(bool isAnnouncementGlobal = true) => DecontaminationLCZ.RpcPlayAnnouncement(5, isAnnouncementGlobal);
	}
}
