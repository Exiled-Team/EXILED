// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
#pragma warning disable 1584
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Exiled.API.Enums;
    using Exiled.API.Features.Items;
    using Exiled.API.Features.Roles;
    using Exiled.API.Features.Toys;

    using InventorySystem.Items.Pickups;

    using LightContainmentZoneDecontamination;

    using MapGeneration.Distributors;

    using Mirror;

    using PlayableScps.ScriptableObjects;

    using UnityEngine;

    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    /// <summary>
    /// A set of tools to easily handle the in-game map.
    /// </summary>
    public static class Map
    {
        /// <summary>
        /// A list of <see cref="Locker"/>s on the map.
        /// </summary>
        internal static readonly List<Locker> LockersValue = new(250);

        /// <summary>
        /// A list of <see cref="PocketDimensionTeleport"/>s on the map.
        /// </summary>
        internal static readonly List<PocketDimensionTeleport> TeleportsValue = new(8);

        /// <summary>
        /// A list of <see cref="Ragdoll"/>s on the map.
        /// </summary>
        internal static readonly List<Ragdoll> RagdollsValue = new();

        /// <summary>
        /// A list of <see cref="AdminToy"/>s on the map.
        /// </summary>
        internal static readonly List<AdminToy> ToysValue = new();

        private static readonly ReadOnlyCollection<PocketDimensionTeleport> ReadOnlyTeleportsValue = TeleportsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<Locker> ReadOnlyLockersValue = LockersValue.AsReadOnly();
        private static readonly ReadOnlyCollection<Ragdoll> ReadOnlyRagdollsValue = RagdollsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<AdminToy> ReadOnlyToysValue = ToysValue.AsReadOnly();

        private static readonly RaycastHit[] CachedFindParentRoomRaycast = new RaycastHit[1];

        private static System.Random random = new();

        /// <summary>
        /// Gets a value indicating whether decontamination has begun in the light containment zone.
        /// </summary>
        public static bool IsLczDecontaminated => DecontaminationController.Singleton._stopUpdating && !DecontaminationController.Singleton.disableDecontamination;

        /// <summary>
        /// Gets all <see cref="PocketDimensionTeleport"/> objects.
        /// </summary>
        public static ReadOnlyCollection<PocketDimensionTeleport> PocketDimensionTeleports => ReadOnlyTeleportsValue;

        /// <summary>
        /// Gets all <see cref="Locker"/> objects.
        /// </summary>
        public static ReadOnlyCollection<Locker> Lockers => ReadOnlyLockersValue;

        /// <summary>
        /// gets all <see cref="Pickup"/>s on the map.
        /// </summary>
        public static ReadOnlyCollection<Pickup> Pickups
        {
            get
            {
                List<Pickup> pickups = new();
                foreach (ItemPickupBase itemPickupBase in Object.FindObjectsOfType<ItemPickupBase>())
                {
                    if (Pickup.Get(itemPickupBase) is Pickup pickup)
                        pickups.Add(pickup);
                }

                return pickups.AsReadOnly();
            }
        }

        /// <summary>
        /// Gets all <see cref="Ragdoll"/> objects.
        /// </summary>
        public static ReadOnlyCollection<Ragdoll> Ragdolls => ReadOnlyRagdollsValue;

        /// <summary>
        /// Gets all <see cref="AdminToy"/> objects.
        /// </summary>
        public static ReadOnlyCollection<AdminToy> Toys => ReadOnlyToysValue;

        /// <summary>
        /// Gets or sets the current seed of the map.
        /// </summary>
        public static int Seed
        {
            get => MapGeneration.SeedSynchronizer.Seed;
            set
            {
                if (!MapGeneration.SeedSynchronizer.MapGenerated)
                    MapGeneration.SeedSynchronizer._singleton.Network_syncSeed = value;
            }
        }

        /// <summary>
        /// Gets the <see cref="global::AmbientSoundPlayer"/>.
        /// </summary>
        public static AmbientSoundPlayer AmbientSoundPlayer { get; internal set; }

        /// <summary>
        /// Tries to find the room that a <see cref="GameObject"/> is inside, first using the <see cref="Transform"/>'s parents, then using a Raycast if no room was found.
        /// </summary>
        /// <param name="objectInRoom">The <see cref="GameObject"/> inside the room.</param>
        /// <returns>The <see cref="Room"/> that the <see cref="GameObject"/> is located inside.</returns>
        public static Room FindParentRoom(GameObject objectInRoom)
        {
            // Avoid errors by forcing Map.Rooms to populate when this is called.
            IEnumerable<Room> rooms = Room.List;

            Room room = null;

            const string playerTag = "Player";

            // First try to find the room owner quickly.
            if (!objectInRoom.CompareTag(playerTag))
            {
                room = objectInRoom.GetComponentInParent<Room>();
            }
            else
            {
                // Check for SCP-079 if it's a player
                Player ply = Player.Get(objectInRoom);

                // Raycasting doesn't make sense,
                // SCP-079 position is constant,
                // let it be 'Outside' instead
                if (ply.Role is Scp079Role role)
                    room = FindParentRoom(role.Camera.GameObject);
            }

            if (room is null)
            {
                // Then try for objects that aren't children, like players and pickups.
                Ray ray = new(objectInRoom.transform.position, Vector3.down);

                if (Physics.RaycastNonAlloc(ray, CachedFindParentRoomRaycast, 10, 1 << 0, QueryTriggerInteraction.Ignore) == 1)
                    return CachedFindParentRoomRaycast[0].collider.gameObject.GetComponentInParent<Room>();

                // Always default to surface transform, since it's static.
                // The current index of the 'Outside' room is the last one
                if (rooms.Count() != 0)
                    return rooms.FirstOrDefault(r => r.gameObject.name == "Outside");
            }

            return room;
        }

        /// <summary>
        /// Broadcasts a message to all <see cref="Player">players</see>.
        /// </summary>
        /// <param name="broadcast">The <see cref="Features.Broadcast"/> to be broadcasted.</param>
        /// <param name="shouldClearPrevious">Clears all players' broadcasts before sending the new one.</param>
        public static void Broadcast(Broadcast broadcast, bool shouldClearPrevious = false)
        {
            if (broadcast.Show)
                Broadcast(broadcast.Duration, broadcast.Content, broadcast.Type, shouldClearPrevious);
        }

        /// <summary>
        /// Broadcasts a message to all <see cref="Player">players</see>.
        /// </summary>
        /// <param name="duration">The duration in seconds.</param>
        /// <param name="message">The message that will be broadcast (supports Unity Rich Text formatting).</param>
        /// <param name="type">The broadcast type.</param>
        /// <param name="shouldClearPrevious">Clears all players' broadcasts before sending the new one.</param>
        public static void Broadcast(ushort duration, string message, global::Broadcast.BroadcastFlags type = global::Broadcast.BroadcastFlags.Normal, bool shouldClearPrevious = false)
        {
            if (shouldClearPrevious)
                ClearBroadcasts();

            Server.Broadcast.RpcAddElement(message, duration, type);
        }

        /// <summary>
        /// Shows a hint to all <see cref="Player">players</see>.
        /// </summary>
        /// <param name="message">The message that will be broadcasted (supports Unity Rich Text formatting).</param>
        /// <param name="duration">The duration in seconds.</param>
        public static void ShowHint(string message, float duration = 3f)
        {
            foreach (Player player in Player.List)
                player.ShowHint(message, duration);
        }

        /// <summary>
        /// Clears all <see cref="Player">players</see>' broadcasts.
        /// </summary>
        public static void ClearBroadcasts() => Server.Broadcast.RpcClearElements();

        /// <summary>
        /// Starts the light containment zone decontamination process.
        /// </summary>
        public static void StartDecontamination()
        {
            DecontaminationController.Singleton.FinishDecontamination();
            DecontaminationController.Singleton.NetworkRoundStartTime = -1f;
        }

        /// <summary>
        /// Turns off all lights in the facility.
        /// </summary>
        /// <param name="duration">The duration of the blackout.</param>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        public static void TurnOffAllLights(float duration, ZoneType zoneTypes = ZoneType.Unspecified)
        {
            foreach (FlickerableLightController controller in FlickerableLightController.Instances)
            {
                Room room = controller.GetComponentInParent<Room>();
                if (zoneTypes.HasFlag(ZoneType.Unspecified) || (room is not null && zoneTypes.HasFlag(room.Zone)))
                    controller.ServerFlickerLights(duration);
            }
        }

        /// <summary>
        /// Turns off all lights in the facility.
        /// </summary>
        /// <param name="duration">The duration of the blackout.</param>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        public static void TurnOffAllLights(float duration, IEnumerable<ZoneType> zoneTypes)
        {
            foreach (ZoneType zone in zoneTypes)
                TurnOffAllLights(duration, zone);
        }

        /// <summary>
        /// Gets a random <see cref="Locker"/>.
        /// </summary>
        /// <returns><see cref="Locker"/> object.</returns>
        public static Locker GetRandomLocker() => Lockers[Random.Range(0, Lockers.Count)];

        /// <summary>
        /// Gets a random <see cref="Pickup"/>.
        /// </summary>
        /// <param name="type">Filters by <see cref="ItemType"/>.</param>
        /// <returns><see cref="Pickup"/> object.</returns>
        public static Pickup GetRandomPickup(ItemType type = ItemType.None)
        {
            List<Pickup> pickups = type != ItemType.None
                ? Pickups.Where(p => p.Type == type).ToList()
                : Pickups.ToList();
            return pickups[Math.Max(0, random.Next(pickups.Count - 1))];
        }

        /// <summary>
        /// Changes the color of a MTF unit.
        /// </summary>
        /// <param name="index">The index of the unit color you want to change.</param>
        /// <param name="color">The new color of the Unit.</param>
        public static void ChangeUnitColor(int index, string color)
        {
            string unit = Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames[index].UnitName;

            Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames.Remove(Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames[index]);
            Respawning.NamingRules.UnitNamingRules.AllNamingRules[Respawning.SpawnableTeamType.NineTailedFox].AddCombination($"<color={color}>{unit}</color>", Respawning.SpawnableTeamType.NineTailedFox);

            foreach (Player ply in Player.List.Where(x => x.UnitName == unit))
            {
                string modifiedUnit = Regex.Replace(unit, "<[^>]*?>", string.Empty);
                if (!string.IsNullOrEmpty(color))
                    modifiedUnit = $"<color={color}>{modifiedUnit}</color>";

                ply.UnitName = modifiedUnit;
            }
        }

        /// <summary>
        /// Plays a random ambient sound.
        /// </summary>
        public static void PlayAmbientSound() => AmbientSoundPlayer.GenerateRandom();

        /// <summary>
        /// Plays an ambient sound.
        /// </summary>
        /// <param name="id">The id of the sound to play.</param>
        public static void PlayAmbientSound(int id)
        {
            if (id >= AmbientSoundPlayer.clips.Length)
                throw new IndexOutOfRangeException($"There are only {AmbientSoundPlayer.clips.Length} sounds available.");

            AmbientSoundPlayer.RpcPlaySound(AmbientSoundPlayer.clips[id].index);
        }

        /// <summary>
        /// Places a Tantrum (SCP-173's ability) in the indicated position.
        /// </summary>
        /// <param name="position">The position where you want to spawn the Tantrum.</param>
        /// <returns>The tantrum's <see cref="GameObject"/>.</returns>
        public static GameObject PlaceTantrum(Vector3 position)
        {
            GameObject gameObject =
                Object.Instantiate(ScpScriptableObjects.Instance.Scp173Data.TantrumPrefab);
            gameObject.transform.position = position;
            NetworkServer.Spawn(gameObject);

            return gameObject;
        }

        /// <summary>
        /// Places a blood decal.
        /// </summary>
        /// <param name="position">The position of the blood decal.</param>
        /// <param name="type">The <see cref="BloodType"/> to place.</param>
        /// <param name="multiplier">A value which determines the spread of the blood decal.</param>
        public static void PlaceBlood(Vector3 position, BloodType type, float multiplier = 1f) => PlayerManager.hostHub.characterClassManager.RpcPlaceBlood(position, (int)type, multiplier);

        /// <summary>
        /// Gets all the near cameras.
        /// </summary>
        /// <param name="position">The position from which starting to search cameras.</param>
        /// <param name="toleration">The maximum toleration to define the radius from which get the cameras.</param>
        /// <returns>A <see cref="IEnumerable{T}"/> of <see cref="Camera"/> which contains all the found cameras.</returns>
        public static IEnumerable<Camera> GetNearCameras(Vector3 position, float toleration = 15f) => Camera.Get(cam => (position - cam.Position).sqrMagnitude <= toleration * toleration);

        /// <summary>
        /// Clears the lazy loading game object cache.
        /// </summary>
        internal static void ClearCache()
        {
            Room.RoomsValue.Clear();
            Door.DoorVariantToDoor.Clear();
            Camera.CamerasValue.Clear();
            Window.WindowValue.Clear();
            Lift.LiftsValue.Clear();
            TeslaGate.TeslasValue.Clear();
            Generator.GeneratorValues.Clear();
            TeleportsValue.Clear();
            LockersValue.Clear();
            Firearm.AvailableAttachmentsValue.Clear();
            Scp079Interactable.InteractablesByRoomId.Clear();
        }
    }
}
