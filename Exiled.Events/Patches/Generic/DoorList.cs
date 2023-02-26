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
    using System;
    using System.Collections.Generic;
    using System.Linq;

    using API.Features;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;
    using MapGeneration;

    using UnityEngine;

    /// <summary>
    /// Patches <see cref="DoorVariant.RegisterRooms"/>.
    /// </summary>
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.RegisterRooms))]
    internal class DoorList
    {
        private static bool Prefix(DoorVariant __instance)
        {
            /*EXILED*/
            if (__instance.Rooms != null)
                return false;
            /*EXILED*/

            Vector3 position = __instance.transform.position;
            int length = 0;

            for (int index = 0; index < 4; ++index)
            {
                Vector3Int coords = RoomIdUtils.PositionToCoords(position + DoorVariant.WorldDirections[index]);

                if (RoomIdentifier.RoomsByCoordinates.TryGetValue(coords, out RoomIdentifier key) && DoorVariant.DoorsByRoom.GetOrAdd(key, () => new HashSet<DoorVariant>()).Add(__instance))
                {
                    DoorVariant.RoomsNonAlloc[length] = key;
                    ++length;
                }
            }

            __instance.Rooms = new RoomIdentifier[length];
            Array.Copy(DoorVariant.RoomsNonAlloc, __instance.Rooms, length);

            /*EXILED*/
            List<Room> rooms = __instance.Rooms.Select(identifier => Room.RoomIdentifierToRoom[identifier]).ToList();

            Door door = new(__instance, rooms);

            foreach (Room room in rooms)
                room.DoorsValue.Add(door);
            /*EXILED*/

            return false;
        }
    }

    /// <summary>
    /// Patches <see cref="DoorVariant.OnDestroy"/>.
    /// </summary>
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.OnDestroy))]
    internal class DoorListRemove
    {
        private static void Postfix(DoorVariant __instance)
        {
            Door.DoorVariantToDoor.Remove(__instance);
        }
    }
}