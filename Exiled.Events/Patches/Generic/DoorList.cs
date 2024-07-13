// -----------------------------------------------------------------------
// <copyright file="DoorList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
#pragma warning disable SA1402
    using System.Collections.Generic;
    using System.Linq;

    using API.Features;

    using Exiled.API.Features.Doors;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;

    /// <summary>
    /// Patches <see cref="DoorVariant.RegisterRooms"/>.
    /// </summary>
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.RegisterRooms))]
    internal class DoorList
    {
        private static void Postfix(DoorVariant __instance)
        {
            if (Door.DoorVariantToDoor.ContainsKey(__instance))
                return;

            List<Room> rooms = __instance.Rooms.Select(identifier => Room.RoomIdentifierToRoom[identifier]).ToList();

            Door door = Door.Create(__instance, rooms);

            foreach (Room room in rooms)
            {
                room.DoorsValue.Add(door);
                room.NearestRoomsValue.AddRange(rooms.Where(r => r != room));
            }

            if (door.Is(out CheckpointDoor checkpoint))
            {
                foreach (DoorVariant subDoor in checkpoint.Base.SubDoors)
                {
                    subDoor.RegisterRooms();
                    BreakableDoor targetDoor = Door.Get(subDoor).Cast<BreakableDoor>();

                    checkpoint.SubDoorsValue.Add(targetDoor);
                }
            }

            return;
        }
    }

    /// <summary>
    /// Patches <see cref="DoorVariant.OnDestroy"/>.
    /// </summary>
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.OnDestroy))]
    internal class DoorListRemove
    {
        private static void Prefix(DoorVariant __instance)
        {
            Door.DoorVariantToDoor.Remove(__instance);
        }
    }
}