// -----------------------------------------------------------------------
// <copyright file="MapGenerated.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Handlers.Internal
{
    using System;
    using System.Collections.Generic;

    using Exiled.API.Extensions;
    using Exiled.API.Features;

    using Interactables.Interobjects.DoorUtils;

    using MapGeneration.Distributors;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using Object = UnityEngine.Object;

    /// <summary>
    /// Handles <see cref="Exiled.Events.Handlers.Map.Generated"/> event.
    /// </summary>
    internal static class MapGenerated
    {
        /// <summary>
        /// Called once the map is generated.
        /// </summary>
        /// <remarks>
        /// This fixes an issue where
        /// all those extensions that
        /// require calling the central
        /// property of the Map class in
        /// the API were corrupted due to
        /// a missed call, such as before
        /// getting the elevator type.
        /// </remarks>
        public static void OnMapGenerated()
        {
            Map.ClearCache();
            GenerateCache();
            LiftTypeExtension.RegisterElevatorTypesOnLevelLoad();
            CameraExtensions.RegisterCameraInfoOnLevelLoad();
            DoorExtensions.RegisterDoorTypesOnLevelLoad();
        }

        private static void GenerateCache()
        {
            GenerateRooms();
            GenerateDoors();
            GenerateCameras();
            GenerateTeslaGates();
            GenerateLifts();
            GeneratePocketTeleports();
            GenerateLockers();
        }

        private static void GenerateRooms()
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

            GameObject pocket = GameObject.Find(PocketPath);

            if (pocket == null)
                Log.Send($"[{typeof(Map).FullName}]: Pocket Dimension not found. The name or location in the game's hierarchy might have changed.", Discord.LogLevel.Error, ConsoleColor.DarkRed);
            else
                roomObjects.Add(pocket);

            // Add the surface since it is not tagged Room. Add it last so we can use it as a default room since it never changes.
            const string surfaceRoomName = "Outside";

            GameObject surface = GameObject.Find(surfaceRoomName);

            if (surface == null)
                Log.Send($"[{typeof(Map).FullName}]: Surface not found. The name in the game's hierarchy might have changed.", Discord.LogLevel.Error, ConsoleColor.DarkRed);
            else
                roomObjects.Add(surface);

            foreach (GameObject roomObject in roomObjects)
                Map.RoomsValue.Add(Room.CreateComponent(roomObject));

            ListPool<GameObject>.Shared.Return(roomObjects);
        }

        private static void GenerateDoors() => Map.DoorsValue.AddRange(Object.FindObjectsOfType<DoorVariant>());

        private static void GenerateCameras() => Map.CamerasValue.AddRange(Object.FindObjectsOfType<Camera079>());

        private static void GenerateLifts() => Map.LiftsValue.AddRange(Object.FindObjectsOfType<Lift>());

        private static void GenerateTeslaGates() => Map.TeslasValue.AddRange(Object.FindObjectsOfType<TeslaGate>());

        private static void GeneratePocketTeleports() => Map.TeleportsValue.AddRange(Object.FindObjectsOfType<PocketDimensionTeleport>());

        private static void GenerateLockers() => Map.LockersValue.AddRange(Object.FindObjectsOfType<Locker>());
    }
}
