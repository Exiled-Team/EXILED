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

    using LightContainmentZoneDecontamination;

    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// A set of tools to handle the in-game map more easily.
    /// </summary>
    public static class Map
    {
        private static readonly List<Room> RoomsValue = new List<Room>();
        private static readonly List<Door> DoorsValue = new List<Door>();
        private static readonly List<Lift> LiftsValue = new List<Lift>();
        private static readonly List<TeslaGate> TeslasValue = new List<TeslaGate>();

        private static readonly ReadOnlyCollection<Room> ReadOnlyRoomsValue = RoomsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<Door> ReadOnlyDoorsValue = DoorsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<Lift> ReadOnlyLiftsValue = LiftsValue.AsReadOnly();
        private static readonly ReadOnlyCollection<TeslaGate> ReadOnlyTeslasValue = TeslasValue.AsReadOnly();

        /// <summary>
        /// Gets a value indicating whether the decontamination has been completed or not.
        /// </summary>
        public static bool IsLCZDecontaminated => DecontaminationController.Singleton._stopUpdating;

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
        public static ReadOnlyCollection<Room> Rooms
        {
            get
            {
                if (RoomsValue.Count == 0)
                    RoomsValue.AddRange(GameObject.FindGameObjectsWithTag("Room").Select(r => new Room(r.name, r.transform, r.transform.position)));

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
                    DoorsValue.AddRange(Object.FindObjectsOfType<Door>());

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
        /// Broadcasts a message to all players.
        /// </summary>
        /// <param name="duration">The duration in seconds.</param>
        /// <param name="message">The message that will be broadcast (supports Unity Rich Text formatting).</param>
        /// <param name="type">The broadcast type.</param>
        public static void Broadcast(ushort duration, string message, Broadcast.BroadcastFlags type = global::Broadcast.BroadcastFlags.Normal)
        {
            Server.Broadcast.RpcAddElement(message, duration, type);
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
            GameObject randomPosition = Object.FindObjectOfType<SpawnpointManager>().GetRandomPosition(roleType);

            return randomPosition == null ? Vector3.zero : randomPosition.transform.position;
        }

        /// <summary>
        /// Starts the Decontamination process.
        /// </summary>
        public static void StartDecontamination() => DecontaminationController.Singleton._nextPhase = DecontaminationController.Singleton.DecontaminationPhases.Length - 1;

        /// <summary>
        /// Turns off all lights of the facility (except for the entrance zone).
        /// </summary>
        /// <param name="duration">The duration of the blackout.</param>
        /// <param name="isHeavyContainmentZoneOnly">Indicates whether only the heavy containment zone lights have to be turned off or not.</param>
        public static void TurnOffAllLights(float duration, bool isHeavyContainmentZoneOnly = false) => Generator079.Generators[0].RpcCustomOverchargeForOurBeautifulModCreators(duration, isHeavyContainmentZoneOnly);

        /// <summary>
        ///     Clears the lazy loading game object cache.
        /// </summary>
        internal static void ClearCache()
        {
            RoomsValue.Clear();
            DoorsValue.Clear();
            LiftsValue.Clear();
            TeslasValue.Clear();
        }

    }
}
