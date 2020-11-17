// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;

    using Exiled.API.Extensions;

    using LightContainmentZoneDecontamination;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A set of tools to easily handle the in-game map.
    /// </summary>
    public static class Map
    {
        private static readonly List<Room> RoomsValue = new List<Room>(250);
        private static readonly List<Door> DoorsValue = new List<Door>(250);
        private static readonly List<Lift> LiftsValue = new List<Lift>(10);
        private static readonly List<TeslaGate> TeslasValue = new List<TeslaGate>(10);

        private static readonly ReadOnlyCollection<Room> ReadOnlyRoomsValue = RoomsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<Door> ReadOnlyDoorsValue = DoorsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<Lift> ReadOnlyLiftsValue = LiftsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<TeslaGate> ReadOnlyTeslasValue = TeslasValue.AsReadOnly();

        private static readonly RaycastHit[] CachedFindParentRoomRaycast = new RaycastHit[1];

        private static SpawnpointManager spawnpointManager;

        /// <summary>
        /// Gets a value indicating whether decontamination has begun in light containment.
        /// </summary>
        public static bool IsLCZDecontaminated => DecontaminationController.Singleton._stopUpdating;

        /// <summary>
        /// Gets the number of activated generators.
        /// </summary>
        public static int ActivatedGenerators => Generator079.mainGenerator.totalVoltage;

        /// <summary>
        /// Gets all SCP-079 cameras.
        /// </summary>
        public static Camera079[] Cameras => Scp079PlayerScript.allCameras;

        /// <summary>
        /// Gets all <see cref="Room"/>.
        /// </summary>
        public static ReadOnlyCollection<Room> Rooms
        {
            get
            {
                if (RoomsValue.Count == 0)
                {
                    List<GameObject> roomObjects = ListPool<GameObject>.Shared.Rent();

                    // Get bulk of rooms.
                    roomObjects.AddRange(GameObject.FindGameObjectsWithTag("Room"));

                    // If no rooms were found, it means a plugin is trying to access this before the map is created.
                    if (roomObjects.Count == 0)
                    {
                        ListPool<GameObject>.Shared.Return(roomObjects);
                        throw new InvalidOperationException("Plugin is trying to access Rooms before they are created.");
                    }

                    // Add the pocket dimension since it is not tagged Room.
                    const string PocketPath = "HeavyRooms/PocketWorld";
                    var pocket = GameObject.Find(PocketPath);
                    if (pocket == null)
                        Log.Send($"[{typeof(Map).FullName}]: Pocket Dimension not found. The name or location in the game's hierarchy might have changed.", Discord.LogLevel.Error, ConsoleColor.DarkRed);
                    else
                        roomObjects.Add(pocket);

                    // Add the surface since it is not tagged Room. Add it last so we can use it as a default room since it never changes.
                    const string surfaceRoomName = "Outside";
                    var surface = GameObject.Find(surfaceRoomName);
                    if (surface == null)
                        Log.Send($"[{typeof(Map).FullName}]: Surface not found. The name in the game's hierarchy might have changed.", Discord.LogLevel.Error, ConsoleColor.DarkRed);
                    else
                        roomObjects.Add(surface);

                    foreach (var roomObject in roomObjects)
                    {
                        RoomsValue.Add(Room.CreateComponent(roomObject));
                    }

                    ListPool<GameObject>.Shared.Return(roomObjects);
                }

                return ReadOnlyRoomsValue;
            }
        }

        /// <summary>
        /// Gets all <see cref="Door"/>.
        /// </summary>
        public static ReadOnlyCollection<Door> Doors
        {
            get
            {
                if (DoorsValue.Count == 0)
                {
                    DoorsValue.AddRange(Object.FindObjectsOfType<Door>());
                    DoorTypeExtension.RegisterDoorTypesOnLevelLoad();
                }

                return ReadOnlyDoorsValue;
            }
        }

        /// <summary>
        /// Gets all <see cref="Lift"/>.
        /// </summary>
        public static ReadOnlyCollection<Lift> Lifts
        {
            get
            {
                if (LiftsValue.Count == 0)
                    LiftsValue.AddRange(Object.FindObjectsOfType<Lift>());

                return ReadOnlyLiftsValue;
            }
        }

        /// <summary>
        /// Gets all <see cref="TeslaGate"/>.
        /// </summary>
        public static ReadOnlyCollection<TeslaGate> TeslaGates
        {
            get
            {
                if (TeslasValue.Count == 0)
                    TeslasValue.AddRange(Object.FindObjectsOfType<TeslaGate>());

                return ReadOnlyTeslasValue;
            }
        }

        /// <summary>
        /// Tries to find the room that a Game Object is inside, first using the transform's parents, then using a Raycast if no room was found.
        /// </summary>
        /// <param name="objectInRoom">The Game Object inside the room.</param>
        /// <returns>The Room.</returns>
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
                if (ply.Role == RoleType.Scp079)
                {
                    // Raycasting doesn't make sence,
                    // Scp079 position is constant,
                    // let it be 'Outside' instead
                    room = FindParentRoom(ply.ReferenceHub.scp079PlayerScript.currentCamera.gameObject);
                }
            }

            if (room == null)
            {
                // Then try for objects that aren't children, like players and pickups.
                Ray ray = new Ray(objectInRoom.transform.position, Vector3.down);

                if (Physics.RaycastNonAlloc(ray, CachedFindParentRoomRaycast, 10, 1 << 0, QueryTriggerInteraction.Ignore) == 1)
                {
                    room = CachedFindParentRoomRaycast[0].collider.gameObject.GetComponentInParent<Room>();
                }
            }

            // Always default to surface transform, since it's static.
            // The current index of the 'Outsise' room is the last one
            if (room == null && rooms.Count != 0)
                room = rooms[rooms.Count - 1];

            return room;
        }

        /// <summary>
        /// Spawns hands at the specified position with specified rotation.
        /// </summary>
        /// <param name="position">Hands position.</param>
        /// <param name="rotation">Hands rotation.</param>
        [Obsolete("This API was removed with SCP-330 and is no longer available.", true)]
        public static void SpawnHands(Vector3 position, Quaternion rotation)
        {
        }

        /// <summary>
        /// Broadcasts a message to all players.
        /// </summary>
        /// <param name="duration">The duration in seconds.</param>
        /// <param name="message">The message that will be broadcast (supports Unity Rich Text formatting).</param>
        /// <param name="type">The broadcast type.</param>
        public static void Broadcast(ushort duration, string message, global::Broadcast.BroadcastFlags type = global::Broadcast.BroadcastFlags.Normal)
        {
            Server.Broadcast.RpcAddElement(message, duration, type);
        }

        /// <summary>
        /// Shows a Hint to all players.
        /// </summary>
        /// <param name="message">The message that will be broadcast (supports Unity Rich Text formatting).</param>
        /// <param name="duration">The duration in seconds.</param>
        public static void ShowHint(string message, float duration)
        {
            foreach (Player player in Player.List)
            {
                player.ShowHint(message, duration);
            }
        }

        /// <summary>
        /// Clears all players' broadcasts.
        /// </summary>
        public static void ClearBroadcasts() => Server.Broadcast.RpcClearElements();

        /// <summary>
        /// Gets a random spawn point of a <see cref="RoleType"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleType"/> to get the spawn point from.</param>
        /// <returns>Returns the spawn point <see cref="Vector3"/>.</returns>
        public static Vector3 GetRandomSpawnPoint(this RoleType roleType)
        {
            GameObject randomPosition;

            if (spawnpointManager == null)
            {
                spawnpointManager = Object.FindObjectOfType<SpawnpointManager>();
            }

            randomPosition = spawnpointManager.GetRandomPosition(roleType);

            return randomPosition == null ? Vector3.zero : randomPosition.transform.position;
        }

        /// <summary>
        /// Starts the light containment decontamination process.
        /// </summary>
        public static void StartDecontamination()
        {
            DecontaminationController.Singleton.FinishDecontamination();
            DecontaminationController.Singleton.NetworkRoundStartTime = -1f;
        }

        /// <summary>
        /// Turns off all lights of the facility (except for the entrance zone).
        /// </summary>
        /// <param name="duration">The duration of the blackout.</param>
        /// <param name="isHeavyContainmentZoneOnly">Indicates whether only the heavy containment zone lights have to be turned off or not.</param>
        public static void TurnOffAllLights(float duration, bool isHeavyContainmentZoneOnly = false) => Generator079.Generators[0].ServerOvercharge(duration, isHeavyContainmentZoneOnly);

        /// <summary>
        /// Clears the lazy loading game object cache.
        /// </summary>
        internal static void ClearCache()
        {
            spawnpointManager = null;

            RoomsValue.Clear();
            DoorsValue.Clear();
            LiftsValue.Clear();
            TeslasValue.Clear();
        }
    }
}
