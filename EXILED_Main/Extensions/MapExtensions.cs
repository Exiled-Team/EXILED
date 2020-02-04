using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EXILED.Extensions
{
	public static class Map
	{
		private static Inventory _hostInventory;
		private static AlphaWarheadController _alphaWarheadController;
		public static Inventory HostInventory
		{
			get
			{
				if (_hostInventory == null)
				{
					// Benchmarked by Petris (SL Programmer), GetHub is faster than a single GetComponent
					_hostInventory = ReferenceHub.GetHub(PlayerManager.localPlayer).inventory;
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
		/// Starts the warhead.
		/// </summary>
		public static void StartWarhead() => AlphaWarheadController.StartDetonation();
		
		/// <summary>
		/// Stops the warhead.
		/// </summary>
		public static void StopWarhead() => AlphaWarheadController.CancelDetonation();
		
		/// <summary>
		/// Detonates the warhead.
		/// </summary>
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
	}
}
