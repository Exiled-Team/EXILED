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
    using System.Reflection.Emit;

    using API.Features;

    using Exiled.API.Features.Doors;
    using Exiled.API.Features.Pools;

    using HarmonyLib;

    using Interactables.Interobjects.DoorUtils;
    using MapGeneration;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="DoorVariant.RegisterRooms"/>.
    /// </summary>
    // TODO: transpiler
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

            Door door = Door.Create(__instance, rooms);

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
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(codeInstructions);

            // Door.DoorVariantToDoor.Remove(this)
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldsfld, Field(typeof(Door), nameof(Door.DoorVariantToDoor))),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, Method(typeof(Dictionary<DoorVariant, Door>), nameof(Dictionary<DoorVariant, Door>.Remove), new[] { typeof(DoorVariant) })),
                    new(OpCodes.Pop),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="Interactables.Interobjects.CheckpointDoor.Start"/>.
    /// </summary>
    [HarmonyPatch(typeof(Interactables.Interobjects.CheckpointDoor), nameof(Interactables.Interobjects.CheckpointDoor.Start))]
    internal class CheckpointDoorsFix
    {
        private static void Postfix(Interactables.Interobjects.CheckpointDoor __instance)
        {
            CheckpointDoor checkpoint = Door.Get(__instance).Cast<CheckpointDoor>();
            foreach (DoorVariant door in __instance.SubDoors)
            {
                door.RegisterRooms();
                checkpoint.SubDoorsValue.Add(Door.Get(door).Cast<BreakableDoor>());
            }
        }
    }
}