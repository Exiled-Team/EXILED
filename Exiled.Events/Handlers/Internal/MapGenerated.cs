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
    using System.Linq;

    using Exiled.API.Extensions;
    using Exiled.API.Features;
    using Exiled.API.Features.Items;
    using Exiled.API.Structs;

    using Interactables.Interobjects.DoorUtils;

    using InventorySystem.Items.Firearms.Attachments;

    using MapGeneration;
    using MapGeneration.Distributors;

    using MEC;

    using UnityEngine;

    using Camera = Exiled.API.Features.Camera;
    using Object = UnityEngine.Object;

    /// <summary>
    /// Handles <see cref="Handlers.Map.Generated"/> event.
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
            Timing.CallDelayed(0.5f, () =>
            {
                GenerateCache();
                Door.RegisterDoorTypesOnLevelLoad();
            });
        }

        private static void GenerateCache()
        {
            GenerateRooms();
            GenerateDoors();
            GenerateWindow();
            GenerateCameras();
            GenerateTeslaGates();
            GenerateLifts();
            GeneratePocketTeleports();
            GenerateAttachments();
            GenerateLockers();
            Map.AmbientSoundPlayer = PlayerManager.localPlayer.GetComponent<AmbientSoundPlayer>();
        }

        private static void GenerateRooms()
        {
            // Get bulk of rooms with sorted.
            IEnumerable<GameObject> roomObjects = Object.FindObjectsOfType<RoomIdentifier>().Select(x => x.gameObject);

            // If no rooms were found, it means a plugin is trying to access this before the map is created.
            if (!roomObjects.Any())
                throw new InvalidOperationException("Plugin is trying to access Rooms before they are created.");

            foreach (GameObject roomObject in roomObjects)
                Room.RoomsValue.Add(Room.CreateComponent(roomObject));
        }

        private static void GenerateDoors()
        {
            foreach (DoorVariant doorVariant in Object.FindObjectsOfType<DoorVariant>())
                Door.DoorsValue.Add(Door.Get(doorVariant));
        }

        private static void GenerateWindow()
        {
            foreach (BreakableWindow breakableWindow in Object.FindObjectsOfType<BreakableWindow>())
                Window.WindowValue.Add(Window.Get(breakableWindow));
        }

        private static void GenerateCameras()
        {
            foreach (Camera079 camera079 in Object.FindObjectsOfType<Camera079>())
                Camera.CamerasValue.Add(new Camera(camera079));
        }

        private static void GenerateLifts()
        {
            foreach (global::Lift lift in Object.FindObjectsOfType<global::Lift>())
                Lift.LiftsValue.Add(new Lift(lift));
        }

        private static void GenerateTeslaGates()
        {
            foreach (global::TeslaGate teslaGate in Object.FindObjectsOfType<global::TeslaGate>())
                TeslaGate.TeslasValue.Add(new TeslaGate(teslaGate));
        }

        private static void GeneratePocketTeleports() => Map.TeleportsValue.AddRange(Object.FindObjectsOfType<PocketDimensionTeleport>());

        private static void GenerateLockers() => Map.LockersValue.AddRange(Object.FindObjectsOfType<Locker>());

        private static void GenerateAttachments()
        {
            foreach (ItemType type in Enum.GetValues(typeof(ItemType)))
            {
                if (!type.IsWeapon(false))
                    continue;

                Item item = Item.Create(type);
                if (!(item is Firearm firearm))
                    continue;

                Firearm.FirearmInstances.Add(firearm);
                uint code = 1;
                List<AttachmentIdentifier> attachmentIdentifiers = new List<AttachmentIdentifier>();
                foreach (FirearmAttachment att in firearm.Attachments)
                {
                    attachmentIdentifiers.Add(new AttachmentIdentifier(code, att.Name, att.Slot));
                    code *= 2U;
                }

                Firearm.AvailableAttachmentsValue.Add(type, attachmentIdentifiers.ToArray());
            }
        }
    }
}
