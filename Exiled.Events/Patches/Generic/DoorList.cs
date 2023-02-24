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
    using System.Reflection.Emit;

    using API.Features;

    using Exiled.API.Features.Pools;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;

    using MapGeneration;

    /// <summary>
    /// Patches <see cref="DoorVariant.RegisterRooms"/>.
    /// </summary>
    [HarmonyPatch(typeof(DoorVariant), nameof(DoorVariant.RegisterRooms))]
    internal class DoorList
    {
        private static void Postfix(DoorVariant __instance)
        {
            List<Room> rooms = __instance.Rooms.Select(identifier => Room.RoomIdentifierToRoom[identifier]).ToList();
            Door door = new(__instance, rooms);
            foreach (Room room in rooms)
            {
                room.DoorsValue.Add(door);
            }
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