// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
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

    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A set of tools to easily handle the in-game map.
    /// </summary>
    public static class Map
    {
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

        /// <summary>
        /// Gets a value indicating whether decontamination has begun in the light containment zone.
        /// </summary>
        public static bool IsLczDecontaminated => DecontaminationController.Singleton._stopUpdating;

        /// <summary>
        /// Gets the number of activated generators.
        /// </summary>
        public static int ActivatedGenerators
        {
            get
            {
                int i = 0;
                foreach (Scp079Generator gen in Recontainer079.AllGenerators)
                {
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
        public static ReadOnlyCollection<Pickup> Pickups
        {
            get
            {
                List<Pickup> pickups = new List<Pickup>();
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
        /// Gets the current state of the intercom.
        /// </summary>
        public static Intercom.State IntercomState => Intercom.host.IntercomState;

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
        /// Gets a value indicating whether or not the intercom is currently being used.
        /// </summary>
        public static bool IntercomInUse => IntercomState == Intercom.State.Transmitting || IntercomState == Intercom.State.TransmittingBypass || IntercomState == Intercom.State.AdminSpeaking;

        /// <summary>
        /// Gets the <see cref="Player"/> that is using the intercom. Will be null if <see cref="IntercomInUse"/> is false.
        /// </summary>
        public static Player IntercomSpeaker => Player.Get(Intercom.host.speaker);

        /// <summary>
        /// Tries to find the room that a <see cref="GameObject"/> is inside, first using the <see cref="Transform"/>'s parents, then using a Raycast if no room was found.
        /// </summary>
        /// <param name="objectInRoom">The <see cref="GameObject"/> inside the room.</param>
        /// <returns>The <see cref="Room"/> that the <see cref="GameObject"/> is located inside.</returns>
        public static Room FindParentRoom(GameObject objectInRoom)
        {
            // Avoid errors by forcing Map.Rooms to populate when this is called.
            var rooms = Rooms;

            Room room = null;

            const string playerTag = "Player";

            // First try to find the room owner quickly.
            if (!objectInRoom.CompareTag(playerTag))
            {
                room = objectInRoom.GetComponentInParent<Room>();
            }
            else
            {
                // Check for Scp079 if it's a player
                var ply = Player.Get(objectInRoom);

                // Raycasting doesn't make sense,
                // Scp079 position is constant,
                // let it be 'Outside' instead
                if (ply.Role == RoleType.Scp079)
                    room = FindParentRoom(ply.ReferenceHub.scp079PlayerScript.currentCamera.gameObject);
            }

            if (room == null)
            {
                // Then try for objects that aren't children, like players and pickups.
                Ray ray = new Ray(objectInRoom.transform.position, Vector3.down);

                if (Physics.RaycastNonAlloc(ray, CachedFindParentRoomRaycast, 10, 1 << 0, QueryTriggerInteraction.Ignore) == 1)
                    room = CachedFindParentRoomRaycast[0].collider.gameObject.GetComponentInParent<Room>();
            }

            // Always default to surface transform, since it's static.
            // The current index of the 'Outsise' room is the last one
            if (room == null && rooms.Count != 0)
                room = rooms[rooms.Count - 1];

            return room;
        }

        /// <summary>
        /// Broadcasts a message to all players.
        /// </summary>
        /// <param name="broadcast">The <see cref="Features.Broadcast"/> to be broadcasted.</param>
        /// <param name="shouldClearPrevious">Clears all players' broadcasts before sending the new one.</param>
        public static void Broadcast(Broadcast broadcast, bool shouldClearPrevious = false)
        {
            if (broadcast.Show)
                Broadcast(broadcast.Duration, broadcast.Content, broadcast.Type, shouldClearPrevious);
        }

        /// <summary>
        /// Broadcasts a message to all players.
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
        /// Shows a hint to all players.
        /// </summary>
        /// <param name="message">The message that will be broadcasted (supports Unity Rich Text formatting).</param>
        /// <param name="duration">The duration in seconds.</param>
        public static void ShowHint(string message, float duration)
        {
            foreach (Player player in Player.List)
                player.ShowHint(message, duration);
        }

        /// <summary>
        /// Clears all players' broadcasts.
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
        /// Turns off all lights of the facility.
        /// </summary>
        /// <param name="duration">The duration of the blackout.</param>
        /// <param name="zoneTypes">The <see cref="ZoneType"/>s to affect.</param>
        public static void TurnOffAllLights(float duration, ZoneType zoneTypes = ZoneType.Unspecified)
        {
            foreach (FlickerableLightController controller in FlickerableLightController.Instances)
            {
                Room room = controller.GetComponentInParent<Room>();
                if (zoneTypes.HasFlag(ZoneType.Unspecified) || (room != null && zoneTypes.HasFlag(room.Zone)))
                    controller.ServerFlickerLights(duration);
            }
        }

        /// <summary>
        /// Gets the camera with the given ID.
        /// </summary>
        /// <param name="cameraId">The camera id to be searched for.</param>
        /// <returns>The <see cref="Camera079"/> with the given ID.</returns>
        public static Camera079 GetCameraById(ushort cameraId)
        {
            foreach (Camera079 camera in Scp079PlayerScript.allCameras)
            {
                if (camera.cameraId == cameraId)
                    return camera;
            }

            return null;
        }

        /// <summary>
        /// Gets the camera with the given camera type.
        /// </summary>
        /// <param name="cameraType">The <see cref="Enums.CameraType"/> to search for.</param>
        /// <returns>The <see cref="Camera079"/> with the given camera type.</returns>
        public static Camera079 GetCameraByType(Enums.CameraType cameraType) =>
            GetCameraById((ushort)cameraType);

        /// <summary>
        /// Gets the door with the given door name.
        /// </summary>
        /// <param name="doorName">The door name.</param>
        /// <returns>The <see cref="Door"/> or null if a door with this name doesn't exist.</returns>
        public static Door GetDoorByName(string doorName)
        {
            DoorNametagExtension.NamedDoors.TryGetValue(doorName, out DoorNametagExtension nameExtension);
            return nameExtension == null ? null : Door.Get(nameExtension.TargetDoor);
        }

        /// <summary>
        /// Changes the color of a MTF unit.
        /// </summary>
        /// <param name="index">The index of the unit color you want to change.</param>
        /// <param name="color">The new color of the Unit.</param>
        public static void ChangeUnitColor(int index, string color)
        {
            var unit = Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames[index].UnitName;

            Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames.Remove(Respawning.RespawnManager.Singleton.NamingManager.AllUnitNames[index]);
            Respawning.NamingRules.UnitNamingRules.AllNamingRules[Respawning.SpawnableTeamType.NineTailedFox].AddCombination($"<color={color}>{unit}</color>", Respawning.SpawnableTeamType.NineTailedFox);

            foreach (var ply in Player.List.Where(x => x.ReferenceHub.characterClassManager.CurUnitName == unit))
            {
                var modifiedUnit = Regex.Replace(unit, "<[^>]*?>", string.Empty);
                if (!string.IsNullOrEmpty(color))
                    modifiedUnit = $"<color={color}>{modifiedUnit}</color>";

                ply.ReferenceHub.characterClassManager.NetworkCurUnitName = modifiedUnit;
            }
        }

        /// <summary>
        /// Plays a random ambient sound.
        /// </summary>
        public static void PlayAmbientSound() => PlayAmbientSound(Random.Range(0, 32));

        /// <summary>
        /// Plays an ambient sound.
        /// </summary>
        /// <param name="id">The id of the sound to play.</param>
        public static void PlayAmbientSound(int id) => PlayerManager.localPlayer.GetComponent<AmbientSoundPlayer>().RpcPlaySound(id);

        /// <summary>
        /// Clears the lazy loading game object cache.
        /// </summary>
        internal static void ClearCache()
        {
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
