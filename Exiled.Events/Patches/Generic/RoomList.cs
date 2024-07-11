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
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features;

    using Exiled.API.Features.Core.Generic.Pools;

    using HarmonyLib;

    using MapGeneration;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoomIdentifier.Awake"/>.
    /// </summary>
    [HarmonyPatch(typeof(RoomIdentifier), nameof(RoomIdentifier.TryAssignId))]
    internal class RoomList
    {
        private static void Postfix(RoomIdentifier __instance) => Room.Get(__instance);
    }

    /// <summary>
    /// Patches <see cref="RoomIdentifier.OnDestroy"/>.
    /// </summary>
    [HarmonyPatch(typeof(RoomIdentifier), nameof(RoomIdentifier.OnDestroy))]
    internal class RoomListRemove
    {
        private static void Postfix(RoomIdentifier __instance)
        {
            if (!Room.RoomIdentifierToRoom.TryGetValue(__instance, out Room room))
            {
                return;
            }

            room.WindowsValue.ForEach(window => Window.BreakableWindowToWindow.Remove(window.Base));

            room.WindowsValue.Clear();
            room.DoorsValue.Clear();
            room.CamerasValue.Clear();
            room.SpeakersValue.Clear();
            room.RoomLightControllersValue.Clear();
            room.RoomsValue.Clear();
            Room.RoomIdentifierToRoom.Remove(__instance);
        }
    }
}