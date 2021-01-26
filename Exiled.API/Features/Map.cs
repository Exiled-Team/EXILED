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

    using Interactables.Interobjects.DoorUtils;

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
        private static readonly List<DoorVariant> DoorsValue = new List<DoorVariant>(250);
        private static readonly List<Camera079> CamerasValue = new List<Camera079>(250);
        private static readonly List<Lift> LiftsValue = new List<Lift>(10);
        private static readonly List<TeslaGate> TeslasValue = new List<TeslaGate>(10);

        private static readonly ReadOnlyCollection<Room> ReadOnlyRoomsValue = RoomsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<DoorVariant> ReadOnlyDoorsValue = DoorsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<Lift> ReadOnlyLiftsValue = LiftsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<Camera079> ReadOnlyCamerasValue = CamerasValue.AsReadOnly();
        private static readonly ReadOnlyCollection<TeslaGate> ReadOnlyTeslasValue = TeslasValue.AsReadOnly();

        private static readonly RaycastHit[] CachedFindParentRoomRaycast = new RaycastHit[1];

        /// <summary>
        /// Gets a value indicating whether decontamination has begun in the light containment zone.
        /// </summary>
        public static bool IsLCZDecontaminated => DecontaminationController.Singleton._stopUpdating;

        /// <summary>
        /// Gets the number of activated generators.
        /// </summary>
        public static int ActivatedGenerators => Generator079.mainGenerator.totalVoltage;

        /// <summary>
        /// Gets all <see cref="Room"/> objects.
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
                        RoomsValue.Add(Room.CreateComponent(roomObject));

                    ListPool<GameObject>.Shared.Return(roomObjects);
                }

                return ReadOnlyRoomsValue;
            }
        }

        /// <summary>
        /// Gets all <see cref="DoorVariant"/> objects.
        /// </summary>
        public static ReadOnlyCollection<DoorVariant> Doors
        {
            get
            {
                if (DoorsValue.Count == 0)
                    DoorsValue.AddRange(Object.FindObjectsOfType<DoorVariant>());

                return ReadOnlyDoorsValue;
            }
        }

        /// <summary>
        /// Gets all <see cref="Camera079"/> objects.
        /// </summary>
        public static ReadOnlyCollection<Camera079> Cameras
        {
            get
            {
                if (CamerasValue.Count == 0)
                    CamerasValue.AddRange(Object.FindObjectsOfType<Camera079>());

                return ReadOnlyCamerasValue;
            }
        }

        /// <summary>
        /// Gets all <see cref="Lift"/> objects.
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
        /// Gets all <see cref="TeslaGate"/> objects.
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
        /// Gets the Default <see cref="Ragdoll.Info"/>,
        /// used in <see cref="SpawnRagdoll(RoleType, string, PlayerStats.HitInfo, Vector3, Quaternion, Vector3, bool, int, string)"/>
        /// and <see cref="SpawnRagdoll(global::Role, Ragdoll.Info, Vector3, Quaternion, Vector3, bool)"/>.
        /// </summary>
        /// <remarks>
        /// This value can be modified to change the default Ragdoll's info.
        /// </remarks>
        public static Ragdoll.Info DefaultRagdollOwner { get; } = new Ragdoll.Info()
        {
            ownerHLAPI_id = null,
            PlayerId = -1,
            DeathCause = new PlayerStats.HitInfo(-1f, "[REDACTED]", DamageTypes.Com15, -1),
            ClassColor = new Color(1f, 0.556f, 0f),
            FullName = "Class-D",
            Nick = "[REDACTED]",
        };

        /// <summary>
        /// Gets the current state of the intercom.
        /// </summary>
        public static Intercom.State IntercomState => Intercom.host.IntercomState;

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
        /// Spawns a ragdoll for a player on a certain position.
        /// </summary>
        /// <param name="victim">Victim, represented as a player.</param>
        /// <param name="deathCause">The message to be displayed as his death.</param>
        /// <param name="position">Where the ragdoll will be spawned.</param>
        /// <param name="rotation">The rotation for the ragdoll.</param>
        /// <param name="velocity">The initial velocity the ragdoll will have, as if it was exploded.</param>
        /// <param name="allowRecall">Sets this ragdoll as respawnable by SCP-049.</param>
        /// <returns>The Ragdoll component (requires Assembly-CSharp to be referenced).</returns>
        public static global::Ragdoll SpawnRagdoll(Player victim, DamageTypes.DamageType deathCause, Vector3 position, Quaternion rotation = default, Vector3 velocity = default, bool allowRecall = true)
        {
            return SpawnRagdoll(
                        victim.Role,
                        deathCause,
                        victim.DisplayNickname,
                        position,
                        rotation,
                        velocity,
                        allowRecall,
                        victim.Id,
                        victim.GameObject.GetComponent<Dissonance.Integrations.MirrorIgnorance.MirrorIgnorancePlayer>().PlayerId);
        }

        /// <summary>
        /// Spawns a ragdoll on the map based on the different arguments.
        /// </summary>
        /// <remarks>
        /// Tip: You can do '<paramref name="allowRecall"/>: true, <paramref name="playerId"/>: MyPlayer.Id' to skip parameters.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Code to spawn a fake ragdoll
        /// if (ev.Player == MyPlugin.TheInmortalPlayer)
        /// {
        ///     var fakeRagdoll = Map.SpawnRagdoll(RoleType.ClassD, DamageTypes.Fall, "The Falling Guy", new Vector3(1234f, -1f, 4321f));
        /// }
        /// </code>
        /// </example>
        /// <param name="roleType">The <see cref="RoleType"/> to use as ragdoll.</param>
        /// <param name="deathCause">The death cause, expressed as a <see cref="DamageTypes.DamageType"/>.</param>
        /// <param name="victimNick">The name from the victim, who the corpse belongs to.</param>
        /// <param name="position">Where the ragdoll will be spawned.</param>
        /// <param name="rotation">The rotation for the ragdoll.</param>
        /// <param name="velocity">The initial velocity the ragdoll will have, as if it was exploded.</param>
        /// <param name="allowRecall">Sets this ragdoll as respawnable by SCP-049. Must have a valid <paramref name="playerId"/>.</param>
        /// <param name="playerId">Used for recall. The <see cref="Player.Id"/> to be recalled.</param>
        /// <param name="mirrorOwnerId">Can be ignored. The <see cref="Dissonance.Integrations.MirrorIgnorance.MirrorIgnorancePlayer"/>'s PlayerId field.</param>
        /// <returns>The Ragdoll component (requires Assembly-CSharp to be referenced).</returns>
        public static global::Ragdoll SpawnRagdoll(
                RoleType roleType,
                DamageTypes.DamageType deathCause,
                string victimNick,
                Vector3 position,
                Quaternion rotation = default,
                Vector3 velocity = default,
                bool allowRecall = false,
                int playerId = -1,
                string mirrorOwnerId = null)
        {
            var @default = DefaultRagdollOwner;
            return SpawnRagdoll(roleType, victimNick, new PlayerStats.HitInfo(@default.DeathCause.Amount, @default.DeathCause.Attacker, deathCause, -1), position, rotation, velocity, allowRecall, playerId, mirrorOwnerId);
        }

        /// <summary>
        /// Spawns a ragdoll on the map based on the different arguments.
        /// </summary>
        /// <remarks>
        /// Tip: You can do, for example, '<paramref name="velocity"/>: "Vector3.up * 3"' to skip parameters.
        /// </remarks>
        /// <example>
        /// <code>
        /// // Code to spawn a fake ragdoll
        /// if (ev.Player == MyPlugin.TheInmortalPlayer)
        /// {
        ///     var fakeRagdoll = Map.SpawnRagdoll(ev.Player.Role, ev.Player.Position, victimNick: ev.Player.DisplayNickname, playerId: ev.Player.Id);
        /// }
        /// </code>
        /// </example>
        /// <param name="roleType">The <see cref="RoleType"/> to use as ragdoll.</param>
        /// <param name="victimNick">The name from the victim, who the corpse belongs to.</param>
        /// <param name="hitInfo">The <see cref="PlayerStats.HitInfo"/> that displays who killed this ragdoll, and using which tool.</param>
        /// <param name="position">Where the ragdoll will be spawned.</param>
        /// <param name="rotation">The rotation for the ragdoll.</param>
        /// <param name="velocity">The initial velocity the ragdoll will have, as if it was exploded.</param>
        /// <param name="allowRecall">Sets this ragdoll as respawnable by SCP-049.</param>
        /// <param name="playerId">Used for recall. The <see cref="Player.Id"/> to be recalled.</param>
        /// <param name="mirrorOwnerId">Can be ignored. The <see cref="Dissonance.Integrations.MirrorIgnorance.MirrorIgnorancePlayer"/>'s PlayerId field, likely used in the client.</param>
        /// <returns>The Ragdoll component (requires Assembly-CSharp to be referenced).</returns>
        public static global::Ragdoll SpawnRagdoll(
                RoleType roleType,
                string victimNick,
                global::PlayerStats.HitInfo hitInfo,
                Vector3 position,
                Quaternion rotation = default,
                Vector3 velocity = default,
                bool allowRecall = false,
                int playerId = -1,
                string mirrorOwnerId = null)
        {
            global::Role role = CharacterClassManager._staticClasses.SafeGet(roleType);

            // Check if there's no ragdoll for this class, or if the class is invalid
            if (role.model_ragdoll == null)
                return null;
            var @default = DefaultRagdollOwner;

            var ragdollInfo = new Ragdoll.Info()
            {
                ownerHLAPI_id = mirrorOwnerId != null ? mirrorOwnerId : @default.ownerHLAPI_id,
                PlayerId = playerId,
                DeathCause = hitInfo != default ? hitInfo : @default.DeathCause,
                ClassColor = role.classColor,
                FullName = role.fullName,
                Nick = victimNick,
            };

            return SpawnRagdoll(role, ragdollInfo, position, rotation, velocity, allowRecall);
        }

        /// <summary>
        /// Optimized method to Spawn a ragdoll on the map.
        /// Will only allocate the newly created GameObject, requires extra work and pre-loaded base game roles.
        /// </summary>
        /// <remarks>
        /// <list type="number">
        /// <item>
        /// <para>
        /// EXILED already has an internal, default Ragdoll.Info: the use of this
        /// method to try to optimize a plugin is absolutely optional.
        /// </para>
        /// We recommend using: Map.SpawnRagdoll(RoleType roleType, string victimNick, Vector3 position)
        /// </item>
        /// <item>
        /// This method should only ever be used if you're dealing with massive
        /// server-sided lag.
        /// </item>
        /// <item>
        /// Ragdoll.Info's "ownerID" isn't the SteamID, but the
        /// <see cref="Dissonance.Integrations.MirrorIgnorance.MirrorIgnorancePlayer"/>'s PlayerId field.
        /// </item>
        /// </list>
        /// </remarks>
        /// <param name="role">Main game's <see cref="global::Role"/> thad defines the role to spawn a ragdoll.</param>
        /// <param name="ragdollInfo"><see cref="Ragdoll.Info"/> object containing the ragdoll's info.</param>
        /// <param name="position">Where the ragdoll will be spawned.</param>
        /// <param name="rotation">The rotation for the ragdoll.</param>
        /// <param name="velocity">The initial velocity the ragdoll will have, as if it was exploded.</param>
        /// <param name="allowRecall">Sets this ragdoll as respawnable by SCP-049.</param>
        /// <returns>The <see cref="Ragdoll"/> component created.</returns>
        public static global::Ragdoll SpawnRagdoll(
                global::Role role,
                Ragdoll.Info ragdollInfo,
                Vector3 position,
                Quaternion rotation = default,
                Vector3 velocity = default,
                bool allowRecall = false)
        {
            if (role.model_ragdoll == null)
                return null;

            GameObject gameObject = Object.Instantiate(role.model_ragdoll, position + role.ragdoll_offset.position, Quaternion.Euler(rotation.eulerAngles + role.ragdoll_offset.rotation));

            // Modify the Ragdoll's component
            global::Ragdoll ragdollObject = gameObject.GetComponent<global::Ragdoll>();
            ragdollObject.Networkowner = ragdollInfo != null ? ragdollInfo : DefaultRagdollOwner;
            ragdollObject.NetworkallowRecall = allowRecall;
            ragdollObject.NetworkPlayerVelo = velocity;

            Mirror.NetworkServer.Spawn(gameObject);

            return ragdollObject;
        }

        /// <summary>
        /// Spawns hands at the specified position with specified rotation.
        /// </summary>
        /// <param name="position">Hands position.</param>
        /// <param name="rotation">Hands rotation.</param>
        [Obsolete("Removed from the base-game.", true)]
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
        /// Gets a random spawn point of a <see cref="RoleType"/>.
        /// </summary>
        /// <param name="roleType">The <see cref="RoleType"/> to get the spawn point from.</param>
        /// <returns>Returns the spawn point <see cref="Vector3"/>.</returns>
        public static Vector3 GetRandomSpawnPoint(this RoleType roleType)
        {
            GameObject randomPosition = CharacterClassManager._spawnpointManager.GetRandomPosition(roleType);

            return randomPosition == null ? Vector3.zero : randomPosition.transform.position;
        }

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
        /// <param name="isHeavyContainmentZoneOnly">Indicates whether or not only lights in the heavy containment zone will be turned off.</param>
        public static void TurnOffAllLights(float duration, bool isHeavyContainmentZoneOnly = false) => Generator079.Generators[0].ServerOvercharge(duration, isHeavyContainmentZoneOnly);

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
        /// <param name="cameraType">The <see cref="CameraType"/> to search for.</param>
        /// <returns>The <see cref="Camera079"/> with the given camera type.</returns>
        public static Camera079 GetCameraByType(CameraType cameraType) =>
            GetCameraById((ushort)cameraType);

        /// <summary>
        /// Gets the door with the given door name.
        /// </summary>
        /// <param name="doorName">The door name.</param>
        /// <returns>The <see cref="DoorVariant"/> or null if a door with this name doesn't exist.</returns>
        public static DoorVariant GetDoorByName(string doorName)
        {
            DoorNametagExtension.NamedDoors.TryGetValue(doorName, out var nameExtension);
            return nameExtension == null ? null : nameExtension.TargetDoor;
        }

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
        }
    }
}
