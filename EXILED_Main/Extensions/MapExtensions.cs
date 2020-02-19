using System;
using System.Collections.Generic;
using System.Linq;
using EXILED.ApiObjects;
using UnityEngine;
using Object = UnityEngine.Object;

namespace EXILED.Extensions
{
	public static class Map
	{
		private static Inventory _hostInventory;
		private static AlphaWarheadController _alphaWarheadController;
		private static Broadcast _broadcast;
		public static Inventory HostInventory
		{
			get
			{
				if (_hostInventory == null)
				{
					_hostInventory = Player.GetPlayer(PlayerManager.localPlayer).inventory;
				}
				return _hostInventory;
			}
		}
		public static AlphaWarheadController AlphaWarheadController
		{
			get
			{
				if (_alphaWarheadController == null)
				{
					_alphaWarheadController = PlayerManager.localPlayer.GetComponent<AlphaWarheadController>();
				}
				return _alphaWarheadController;
			}
		}
		internal static Broadcast BroadcastComponent
		{
			get
			{
				if(_broadcast == null)
				{
					_broadcast = PlayerManager.localPlayer.GetComponent<Broadcast>();
				}
				return _broadcast;
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
		public static void ClearBroadcasts() => BroadcastComponent.RpcClearElements();
		/// <summary>
		/// Starts the warhead.
		/// </summary>
		[Obsolete("Use StartNuke")]
		public static void StartWarhead() => AlphaWarheadController.StartDetonation();
		
		/// <summary>
		/// Stops the warhead.
		/// </summary>
		[Obsolete("Use StopNuke")]
		public static void StopWarhead() => AlphaWarheadController.CancelDetonation();
		
		/// <summary>
		/// Detonates the warhead.
		/// </summary>
		[Obsolete("Use DetonateNuke")]
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
		/// <param name="role">RoleType</param>
		/// <returns>Vector3 spawnPoint</returns>
		public static Vector3 GetRandomSpawnPoint(RoleType role)
		{
			GameObject randomPosition = Object.FindObjectOfType<SpawnpointManager>().GetRandomPosition(role);
			
			return randomPosition == null ? Vector3.zero : randomPosition.transform.position;
		}

		public static IEnumerable<Room> GetRooms() => Object.FindObjectsOfType<Component>().Where(c => c.CompareTag("Room")).Select(comp => new Room(){Name = comp.name, Position = comp.transform.position, Transform = comp.transform});
	}
}
