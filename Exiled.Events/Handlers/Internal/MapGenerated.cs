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

    using API.Features;
    using API.Features.Items;
    using API.Features.Pools;
    using API.Structs;

    using Exiled.API.Enums;
    using Exiled.API.Extensions;

    using Interactables.Interobjects;

    using InventorySystem.Items.Firearms.Attachments;
    using InventorySystem.Items.Firearms.Attachments.Components;

    using MapGeneration;
    using MapGeneration.Distributors;

    using MEC;

    using PlayerRoles.PlayableScps.Scp079.Cameras;

    using Utils.NonAllocLINQ;

    using Camera = API.Features.Camera;
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
            Timing.CallDelayed(1, GenerateCache);
        }

        private static void GenerateCache()
        {
            Warhead.OutsitePanel = Object.FindObjectOfType<AlphaWarheadOutsitePanel>();

            GenerateCamera();
            GenerateRooms();
            GenerateWindows();
            GenerateLifts();
            GeneratePocketTeleports();
            GenerateAttachments();
            GenerateLockers();

            Map.AmbientSoundPlayer = ReferenceHub.HostHub.GetComponent<AmbientSoundPlayer>();

            Handlers.Map.OnGenerated();

            Timing.CallDelayed(0.1f, Handlers.Server.OnWaitingForPlayers);
        }

        private static void GenerateRooms()
        {
            // Get bulk of rooms with sorted.
            List<RoomIdentifier> roomIdentifiers = ListPool<RoomIdentifier>.Pool.Get(RoomIdentifier.AllRoomIdentifiers);

            // If no rooms were found, it means a plugin is trying to access this before the map is created.
            if (roomIdentifiers.Count == 0)
                throw new InvalidOperationException("Plugin is trying to access Rooms before they are created.");

            foreach (RoomIdentifier roomIdentifier in roomIdentifiers)
                Room.RoomIdentifierToRoom.Add(roomIdentifier, Room.CreateComponent(roomIdentifier.gameObject));

            ListPool<RoomIdentifier>.Pool.Return(roomIdentifiers);
        }

        private static void GenerateWindows()
        {
            foreach (BreakableWindow breakableWindow in Object.FindObjectsOfType<BreakableWindow>())
                new Window(breakableWindow);
        }

        private static void GenerateLifts()
        {
            foreach (ElevatorChamber elevatorChamber in Object.FindObjectsOfType<ElevatorChamber>())
                new Lift(elevatorChamber);
        }

        private static void GenerateCamera()
        {
            foreach (Scp079Camera camera079 in Object.FindObjectsOfType<Scp079Camera>())
                new Camera(camera079);
        }

        private static void GeneratePocketTeleports() => Map.TeleportsValue.AddRange(Object.FindObjectsOfType<PocketDimensionTeleport>());

        private static void GenerateLockers() => Map.LockersValue.AddRange(Object.FindObjectsOfType<Locker>().Select(API.Features.Lockers.Locker.Get));

        private static void GenerateAttachments()
        {
            foreach (FirearmType firearmType in Enum.GetValues(typeof(FirearmType)))
            {
                if (firearmType == FirearmType.None)
                    continue;

                if (Item.Create(firearmType.GetItemType()) is not Firearm firearm)
                    continue;

                Firearm.ItemTypeToFirearmInstance.Add(firearmType, firearm);

                List<AttachmentIdentifier> attachmentIdentifiers = ListPool<AttachmentIdentifier>.Pool.Get();
                HashSet<AttachmentSlot> attachmentsSlots = HashSetPool<AttachmentSlot>.Pool.Get();

                uint code = 1;

                foreach (Attachment attachment in firearm.Attachments)
                {
                    attachmentsSlots.Add(attachment.Slot);
                    attachmentIdentifiers.Add(new(code, attachment.Name, attachment.Slot));
                    code *= 2U;
                }

                uint baseCode = 0;

                attachmentsSlots
                    .ForEach(slot => baseCode += attachmentIdentifiers
                    .Where(attachment => attachment.Slot == slot)
                    .Aggregate((curMin, nextEntry) => nextEntry.Code < curMin.Code ? nextEntry : curMin));

                Firearm.BaseCodesValue.Add(firearmType, baseCode);
                Firearm.AvailableAttachmentsValue.Add(firearmType, attachmentIdentifiers.ToArray());

                ListPool<AttachmentIdentifier>.Pool.Return(attachmentIdentifiers);
                HashSetPool<AttachmentSlot>.Pool.Return(attachmentsSlots);
            }
        }
    }
}