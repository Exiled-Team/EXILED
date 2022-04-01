// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features {
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text.RegularExpressions;

    using Exiled.API.Enums;
    using Exiled.API.Features.Items;

    using Interactables.Interobjects.DoorUtils;

    using InventorySystem.Items.Pickups;

    using LightContainmentZoneDecontamination;

    using MapGeneration.Distributors;

    using Mirror;

    using NorthwoodLib.Pools;

    using PlayableScps.ScriptableObjects;

    using UnityEngine;

    using Object = UnityEngine.Object;
    using Random = UnityEngine.Random;

    /// <summary>
    /// A set of tools to easily handle the in-game map.
    /// </summary>
    public static class Map {
        /// <summary>
        /// A list of <see cref="Room"/>s on the map.
        /// </summary>
        internal static readonly List<Room> RoomsValue = new List<Room>(250);

        /// <summary>
        /// A list of <see cref="Door"/>s on the map.
        /// </summary>
        internal static readonly List<Door> DoorsValue = new List<Door>(250);

        /// <summary>
        /// A list of <see cref="Camera079"/>s on the map.
        /// </summary>
        internal static readonly List<Camera079> CamerasValue = new List<Camera079>(250);

        /// <summary>
        /// A list of <see cref="Lift"/>s on the map.
        /// </summary>
        internal static readonly List<Lift> LiftsValue = new List<Lift>(10);

        /// <summary>
        /// A list of <see cref="Locker"/>s on the map.
        /// </summary>
        internal static readonly List<Locker> LockersValue = new List<Locker>(250);

        /// <summary>
        /// A list of <see cref="PocketDimensionTeleport"/>s on the map.
        /// </summary>
        internal static readonly List<PocketDimensionTeleport> TeleportsValue = new List<PocketDimensionTeleport>(8);

        /// <summary>
        /// A list of <see cref="TeslaGate"/>s on the map.
        /// </summary>
        internal static readonly List<TeslaGate> TeslasValue = new List<TeslaGate>(10);

        /// <summary>
        /// A list of <see cref="Ragdoll"/>s on the map.
        /// </summary>
        internal static readonly List<Ragdoll> RagdollsValue = new List<Ragdoll>();

        private static readonly ReadOnlyCollection<Room> ReadOnlyRoomsValue = RoomsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<Door> ReadOnlyDoorsValue = DoorsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<Lift> ReadOnlyLiftsValue = LiftsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<Camera079> ReadOnlyCamerasValue = CamerasValue.AsReadOnly();
        private static readonly ReadOnlyCollection<TeslaGate> ReadOnlyTeslasValue = TeslasValue.AsReadOnly();
        private static readonly ReadOnlyCollection<PocketDimensionTeleport> ReadOnlyTeleportsValue = TeleportsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<Locker> ReadOnlyLockersValue = LockersValue.AsReadOnly();
        private static readonly ReadOnlyCollection<Ragdoll> ReadOnlyRagdollsValue = RagdollsValue.AsReadOnly();

        private static readonly RaycastHit[] CachedFindParentRoomRaycast = new RaycastHit[1];

        private static System.Random random = new System.Random();

        /// <summary>
        /// Gets a value indicating whether decontamination has begun in the light containment zone.
        /// </summary>
        public static bool IsLczDecontaminated => DecontaminationController.Singleton._stopUpdating && !DecontaminationController.Singleton.disableDecontamination;

        /// <summary>
        /// Gets the number of activated generators.
        /// </summary>
        public static int ActivatedGenerators {
            get {
                int i = 0;
                foreach (Scp079Generator gen in Recontainer079.AllGenerators) {
                    if (gen.Engaged)
                        i++;
                }

                return i;
            }
        }

        /// <summary>
        /// Gets all <see cref="Room"/> objects.
        /// </summary>
        public static ReadOnlyCollection<Room> Rooms => ReadOnlyRoomsValue;

        /// <summary>
        /// Gets all <see cref="Door"/> objects.
        /// </summary>
        public static ReadOnlyCollection<Door> Doors => ReadOnlyDoorsValue;

        /// <summary>
        /// Gets all <see cref="Camera079"/> objects.
        /// </summary>
        public static ReadOnlyCollection<Camera079> Cameras => ReadOnlyCamerasValue;

        /// <summary>
        /// Gets all <see cref="Lift"/> objects.
        /// </summary>
        public static ReadOnlyCollection<Lift> Lifts => ReadOnlyLiftsValue;

        /// <summary>
        /// Gets all <see cref="TeslaGate"/> objects.
        /// </summary>
        public static ReadOnlyCollection<TeslaGate> TeslaGates => ReadOnlyTeslasValue;

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
        public static ReadOnlyCollection<Pickup> Pickups {
            get {
                List<Pickup> pickups = new List<Pickup>();
                foreach (ItemPickupBase itemPickupBase in Object.FindObjectsOfType<ItemPickupBase>()) {
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
        /// Gets the current state of the intercom.
        /// </summary>
        public static Intercom.State IntercomState => Intercom.host.IntercomState;

        /// <summary>
        /// Gets or sets the current seed of the map.
        /// </summary>
        public static int Seed {
            get => MapGeneration.SeedSynchronizer.Seed;
            set {
                if (!MapGeneration.SeedSynchronizer.MapGenerated)
                    MapGeneration.SeedSynchronizer._singleton.Network_syncSeed = value;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the intercom is currently being used.
        /// </summary>
        public static bool IntercomInUse => IntercomState == Intercom.State.Transmitting || IntercomState == Intercom.State.TransmittingBypass || IntercomState == Intercom.State.AdminSpeaking;

        /// <summary>
        /// Gets the <see cref="Player"/> that is using the intercom. Will be <see langword="null"/> if <see cref="IntercomInUse"/> is <see langword="false"/>.
        /// </summary>
        public static Player IntercomSpeaker => Player.Get(Intercom.host.speaker);

        /// <summary>
        /// Gets the <see cref="global::AmbientSoundPlayer"/>.
        /// </summary>
        public static AmbientSoundPlayer AmbientSoundPlayer { get; internal set; }

        /// <summary>
        /// Tries to find the room that a <see cref="GameObject"/> is inside, first using the <see cref="Transform"/>'s parents, then using a Raycast if no room was found.
        /// </summary>
        /// <param name="objectInRoom">The <see cref="GameObject"/> inside the room.</param>
        /// <returns>The <see cref="Room"/> that the <see cref="GameObject"/> is located inside.</returns>
        public static Room FindParentRoom(GameObject objectInRoom) {
            // Avoid errors by forcing Map.Rooms to populate when this is called.
            ReadOnlyCollection<Room> rooms = Rooms;

            Room room = null;

            const string playerTag = "Player";

            // First try to find the room owner quickly.
            if (!objectInRoom.CompareTag(playerTag)) {
                room = objectInRoom.GetComponentInParent<Room>();
            }
            else {
                // Check for Scp079 if it's a player
                Player ply = Player.Get(objectInRoom);

                // Raycasting doesn't make sense,
                // Scp079 position is constant,
                // let it be 'Outside' instead
                if (ply.Role == RoleType.Scp079)
                    room = FindParentRoom(ply.Camera.gameObject);
            }

            if (room == null) {
                // Then try for objects that aren't children, like players and pickups.
                Ray ray = new Ray(objectInRoom.transform.position, Vector3.down);

                if (Physics.RaycastNonAlloc(ray, CachedFindParentRoomRaycast, 10, 1 << 0, QueryTriggerInteraction.Ignore) == 1)
                    return CachedFindParentRoomRaycast[0].collider.gameObject.GetComponentInParent<Room>();

                // Always default to surface transform, since it's static.
                // The current index of the 'Outside' room is the last one
                if (rooms.Count != 0)
                    return rooms.FirstOrDefault(r => r.gameObject.name == "Outside");
            }

            return room;
        }

        /// <summary>
        /// Broadcasts a message to all <see cref="Player">players</see>.
        /// </summary>
        /// <param name="broadcast">The <see cref="Features.Broadcast"/> to be broadcasted.</param>
        /// <param name="shouldClearPrevious">Clears all players' broadcasts before sending the new one.</param>
        public static void Broadcast(Broadcast broadcast, bool shouldClearPrevious = false) {
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
        public static void Broadcast(ushort duration, string message, global::Broadcast.BroadcastFlags type = global::Broadcast.BroadcastFlags.Normal, bool shouldClearPrevious = false) {
            if (shouldClearPrevious)
                ClearBroadcasts();

            Server.Broadcast.RpcAddElement(message, duration, type);
        }

        /// <summary>
        /// Shows a hint to all <see cref="Player">players</see>.
        /// </summary>
        /// <param name="message">The message that will be broadcasted (supports Unity Rich Text formatting).</param>
        /// <param name="duration">The duration in seconds.</param>
        public static void ShowHint(string message, float duration) {
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
        public static void StartDecontamination() {
            DecontaminationController.Singleton.FinishDecontamination();
            DecontaminationController.Singleton.NetworkRoundStartTime = -1f;
        }

        /// <summary>
        /// Turns off all lights in the facility.
        /// </summary>
        /// <param name="duration">The duration of the blackout.</param>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        public static void TurnOffAllLights(float duration, ZoneType zoneTypes = ZoneType.Unspecified) {
            foreach (FlickerableLightController controller in FlickerableLightController.Instances) {
                Room room = controller.GetComponentInParent<Room>();
                if (zoneTypes.HasFlag(ZoneType.Unspecified) || (room != null && zoneTypes.HasFlag(room.Zone)))
                    controller.ServerFlickerLights(duration);
            }
        }

        /// <summary>
        /// Turns off all lights in the facility.
        /// </summary>
        /// <param name="duration">The duration of the blackout.</param>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        public static void TurnOffAllLights(float duration, IEnumerable<ZoneType> zoneTypes) {
            foreach (ZoneType zone in zoneTypes)
                TurnOffAllLights(duration, zone);
        }

        /// <summary>
        /// Locks all <see cref="Door">doors</see> in the facility.
        /// </summary>
        /// <param name="duration">The duration of the lockdown.</param>
        /// <param name="zoneType">The <see cref="ZoneType"/> to affect.</param>
        /// <param name="lockType">DoorLockType of the lockdown.</param>
        public static void LockAllDoors(float duration, ZoneType zoneType = ZoneType.Unspecified, DoorLockType lockType = DoorLockType.Regular079) {
            foreach (Room room in Rooms) {
                if (room != null && room.Zone == zoneType) {
                    foreach (Door door in room.Doors) {
                        door.IsOpen = false;
                        door.ChangeLock(lockType);
                        MEC.Timing.CallDelayed(duration, () => door.ChangeLock(DoorLockType.None));
                    }
                }
            }
        }

        /// <summary>
        /// Locks all <see cref="Door">doors</see> in the facility.
        /// </summary>
        /// <param name="duration">The duration of the lockdown.</param>
        /// <param name="zoneTypes">DoorLockType of the lockdown.</param>
        /// <param name="lockType">The <see cref="ZoneType"/>s to affect.</param>
        public static void LockAllDoors(float duration, IEnumerable<ZoneType> zoneTypes, DoorLockType lockType = DoorLockType.Regular079) {
            foreach (ZoneType zone in zoneTypes)
                LockAllDoors(duration, zone, lockType);
        }

        /// <summary>
        /// Unlocks all <see cref="Door">doors</see> in the facility.
        /// </summary>
        /// <param name="zoneType">The <see cref="ZoneType"/> to affect.</param>
        public static void UnlockAllDoors(ZoneType zoneType) {
            foreach (Room room in Rooms) {
                if (room != null && room.Zone == zoneType) {
                    foreach (Door door in room.Doors) {
                        door.ChangeLock(DoorLockType.None);
                    }
                }
            }
        }

        /// <summary>
        /// Unlocks all <see cref="Door">doors</see> in the facility.
        /// </summary>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        public static void UnlockAllDoors(IEnumerable<ZoneType> zoneTypes) {
            foreach (ZoneType zone in zoneTypes)
                UnlockAllDoors(zone);
        }

        /// <summary>
        /// Unlocks all <see cref="Door">doors</see> in the facility.
        /// </summary>
        public static void UnlockAllDoors() {
            foreach (Door door in Doors)
                door.ChangeLock(DoorLockType.None);
        }

        /// <summary>
        /// Gets an random <see cref="Room"/>.
        /// </summary>
        /// <param name="type">Filters by <see cref="ZoneType"/>.</param>
        /// <returns><see cref="Room"/> object.</returns>
        public static Room GetRandomRoom(ZoneType type = ZoneType.Unspecified) {
            List<Room> rooms = type != ZoneType.Unspecified ? RoomsValue.Where(r => r.Zone == type).ToList() : RoomsValue;
            return rooms[random.Next(Math.Max(0, rooms.Count - 1))];
        }

        /// <summary>
        /// Gets an random <see cref="Camera079"/>.
        /// </summary>
        /// <returns><see cref="Camera079"/> object.</returns>
        public static Camera079 GetRandomCamera() => Cameras[Random.Range(0, Cameras.Count)];

        /// <summary>
        /// Gets an random <see cref="Door"/>.
        /// </summary>
        /// <param name="type">Filters by <see cref="ZoneType"/>.</param>
        /// <param name="onlyUnbroken">Whether or not it filters broken doors.</param>
        /// <returns><see cref="Door"/> object.</returns>
        public static Door GetRandomDoor(ZoneType type = ZoneType.Unspecified, bool onlyUnbroken = false) {
            List<Door> doors = onlyUnbroken || type != ZoneType.Unspecified ? DoorsValue.Where(x => (x.Room == null || x.Room.Zone == type || type == ZoneType.Unspecified) && (!x.IsBroken || !onlyUnbroken)).ToList() : DoorsValue;
            return doors[random.Next(Math.Max(0, doors.Count - 1))];
        }

        /// <summary>
        /// Gets an random <see cref="Lift"/>.
        /// </summary>
        /// <returns><see cref="Lift"/> object.</returns>
        public static Lift GetRandomLift() => Lifts[Random.Range(0, Lifts.Count)];

        /// <summary>
        /// Gets an random <see cref="Locker"/>.
        /// </summary>
        /// <returns><see cref="Locker"/> object.</returns>
        public static Locker GetRandomLocker() => Lockers[Random.Range(0, Lockers.Count)];

        /// <summary>
        /// Gets an random <see cref="Pickup"/>.
        /// </summary>
        /// <param name="type">Filters by <see cref="ItemType"/>.</param>
        /// <returns><see cref="Pickup"/> object.</returns>
        public static Pickup GetRandomPickup(ItemType type = ItemType.None) {
            List<Pickup> pickups = type != ItemType.None
                ? Pickups.Where(p => p.Type == type).ToList()
                : Pickups.ToList();
            return pickups[Math.Max(0, random.Next(pickups.Count - 1))];
        }

        /// <summary>
        /// Gets the <see cref="Camera079">camera</see> with the given ID.
        /// </summary>
        /// <param name="cameraId">The camera id to be searched for.</param>
        /// <returns>The <see cref="Camera079"/> with the given ID.</returns>
        public static Camera079 GetCameraById(ushort cameraId) {
            foreach (Camera079 camera in Scp079PlayerScript.allCameras) {
                if (camera.cameraId == cameraId)
                    return camera;
            }

            return null;
        }

        /// <summary>
        /// Gets the <see cref="Camera079">camera</see> with the given camera type.
        /// </summary>
        /// <param name="cameraType">The <see cref="Enums.CameraType"/> to search for.</param>
        /// <returns>The <see cref="Camera079"/> with the given camera type.</returns>
        public static Camera079 GetCameraByType(Enums.CameraType cameraType) =>
            GetCameraById((ushort)cameraType);

        /// <summary>
        /// Gets the <see cref="Door">door</see> with the given door name.
        /// </summary>
        /// <param name="doorName">The door name.</param>
        /// <returns>The <see cref="Door"/> or null if a door with this name doesn't exist.</returns>
        public static Door GetDoorByName(string doorName) {
            DoorNametagExtension.NamedDoors.TryGetValue(doorName, out DoorNametagExtension nameExtension);
            return nameExtension == null ? null : Door.Get(nameExtension.TargetDoor);
        }

        /// <summary>
        /// Changes the color of a MTF unit.
        /// </summary>
        /// <param name="index">The index of the unit color you want to change.</param>
        /// <param name="color">The new color of the Unit.</param>
        public static void ChangeUnitColor(int index, string color) {
            string unit = Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames[index].UnitName;

            Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames.Remove(Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames[index]);
            Respawning.NamingRules.UnitNamingRules.AllNamingRules[Respawning.SpawnableTeamType.NineTailedFox].AddCombination($"<color={color}>{unit}</color>", Respawning.SpawnableTeamType.NineTailedFox);

            foreach (Player ply in Player.List.Where(x => x.UnitName == unit)) {
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
        public static void PlayAmbientSound(int id) {
            if (id >= AmbientSoundPlayer.clips.Length)
                throw new System.IndexOutOfRangeException($"There are only {AmbientSoundPlayer.clips.Length} sounds available.");

            AmbientSoundPlayer.RpcPlaySound(AmbientSoundPlayer.clips[id].index);
        }

        /// <summary>
        /// Places a Tantrum (Scp173's ability) in the indicated position.
        /// </summary>
        /// <param name="position">The position where you want to spawn the Tantrum.</param>
        /// <returns>The tantrum's <see cref="GameObject"/>.</returns>
        public static GameObject PlaceTantrum(Vector3 position) {
            GameObject gameObject =
                Object.Instantiate(ScpScriptableObjects.Instance.Scp173Data.TantrumPrefab);
            gameObject.transform.position = position;
            NetworkServer.Spawn(gameObject);

            return gameObject;
        }

        /// <summary>
        /// Plays the intercom's sound.
        /// </summary>
        /// <param name="start">Sets a value indicating whether or not the sound is the intercom's start speaking sound.</param>
        /// <param name="transmitterId">Sets the transmitterId.</param>
        public static void PlayIntercomSound(bool start, int transmitterId = 0) => Intercom.host.RpcPlaySound(start, transmitterId);

        /// <summary>
        /// Clears the lazy loading game object cache.
        /// </summary>
        internal static void ClearCache() {
            RoomsValue.Clear();
            DoorsValue.Clear();
            LiftsValue.Clear();
            TeslasValue.Clear();
            CamerasValue.Clear();
            TeleportsValue.Clear();
            LockersValue.Clear();
        }
    }
}
