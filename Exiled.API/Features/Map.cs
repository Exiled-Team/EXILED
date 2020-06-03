// -----------------------------------------------------------------------
// <copyright file="Map.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.API.Features
{
    using System.Collections.Generic;
    using System.Linq;
    using LightContainmentZoneDecontamination;
    using UnityEngine;
    using Object = UnityEngine.Object;

    /// <summary>
    /// A set of tools to handle the in-game map more easily.
    /// </summary>
    public static class Map
    {
        private static List<Room> rooms = new List<Room>();
        private static List<Door> doors = new List<Door>();
        private static List<Lift> lifts = new List<Lift>();
        private static List<TeslaGate> teslas = new List<TeslaGate>();

        /// <summary>
        /// Gets a value indicating whether the decontamination has been completed or not.
        /// </summary>
        public static bool IsLCZDecontaminated => DecontaminationController.Singleton.stopUpdating;

        /// <summary>
        /// Gets the number of activated generators.
        /// </summary>
        public static int ActivatedGenerators => Generator079.mainGenerator.totalVoltage;

        /// <summary>
        /// Gets all cameras of SCP-079.
        /// </summary>
        public static Camera079[] Cameras => Scp079PlayerScript.allCameras;

        /// <summary>
        /// Gets all <see cref="Room"/>.
        /// </summary>
        public static List<Room> Rooms
        {
            get
            {
                if (rooms == null || rooms.Count == 0)
                    rooms = Object.FindObjectsOfType<Transform>().Where(transform => transform.CompareTag("Room")).Select(doorTransform => new Room(doorTransform.name, doorTransform, doorTransform.position)).ToList();

                return rooms;
            }
        }

        /// <summary>
        /// Gets all <see cref="Door"/>.
        /// </summary>
        public static List<Door> Doors
        {
            get
            {
                if (doors == null || doors.Count == 0)
                    doors = Object.FindObjectsOfType<Door>().ToList();

                return doors;
            }
        }

        /// <summary>
        /// Gets all <see cref="Lift"/>.
        /// </summary>
        public static List<Lift> Lifts
        {
            get
            {
                if (lifts == null || lifts.Count == 0)
                    lifts = Object.FindObjectsOfType<Lift>().ToList();

                return lifts;
            }
        }

        /// <summary>
        /// Gets all <see cref="TeslaGate"/>.
        /// </summary>
        public static List<TeslaGate> TeslaGates
        {
            get
            {
                if (teslas == null || teslas.Count == 0)
                    teslas = Object.FindObjectsOfType<TeslaGate>().ToList();

                return teslas;
            }
        }

        /// <summary>
        /// Broadcasts a message to all players.
        /// </summary>
        /// <param name="duration">The duration in seconds.</param>
        /// <param name="message">The message that will be broadcast (supports Unity Rich Text formatting).</param>
        /// <param name="type">The broadcast type.</param>
        public static void Broadcast(ushort duration, string message, Broadcast.BroadcastFlags type) => Server.Broadcast.RpcAddElement(message, duration, type);

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
            GameObject randomPosition = Object.FindObjectOfType<SpawnpointManager>().GetRandomPosition(roleType);

            return randomPosition == null ? Vector3.zero : randomPosition.transform.position;
        }

        /// <summary>
        /// Starts the Decontamination process.
        /// </summary>
        public static void StartDecontamination() => DecontaminationController.Singleton.nextPhase = DecontaminationController.Singleton.DecontaminationPhases.Length - 1;

        /// <summary>
        /// Turns off all lights of the facility (except for the entrance zone).
        /// </summary>
        /// <param name="duration">The duration of the blackout.</param>
        /// <param name="isHeavyContainmentZoneOnly">Indicates whether only the heavy containment zone lights have to be turned off or not.</param>
        public static void TurnOffAllLights(float duration, bool isHeavyContainmentZoneOnly = false) => Generator079.generators[0].RpcCustomOverchargeForOurBeautifulModCreators(duration, isHeavyContainmentZoneOnly);
    }
}
