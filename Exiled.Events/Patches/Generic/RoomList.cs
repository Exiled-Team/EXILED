// -----------------------------------------------------------------------
// <copyright file="RoomList.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable SA1313
#pragma warning disable SA1402
    using API.Features;

    using HarmonyLib;

    using MapGeneration;

    /// <summary>
    /// Patches <see cref="RoomIdentifier.Awake"/>.
    /// </summary>
    [HarmonyPatch(typeof(RoomIdentifier), nameof(RoomIdentifier.Awake))]
    internal class RoomList
    {
        private static void Postfix(RoomIdentifier __instance)
        {
            Room.CreateComponent(__instance.gameObject);
        }
    }

    /// <summary>
    /// Patches <see cref="RoomIdentifier.OnDestroy"/>.
    /// </summary>
    [HarmonyPatch(typeof(RoomIdentifier), nameof(RoomIdentifier.OnDestroy))]
    internal class RoomListRemove
    {
        private static void Postfix(RoomIdentifier __instance)
        {
            Room.RoomIdentifierToRoom[__instance].Doors.Do(door =>
            {
                Log.Info("Removed door");
                Door.DoorVariantToDoor.Remove(door.Base);
            });

            Room.RoomIdentifierToRoom.Remove(__instance);
        }
    }
}