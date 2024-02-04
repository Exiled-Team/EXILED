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
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(codeInstructions);

            int offset = -3;
            int index = newInstructions.FindIndex(i => i.Calls(Method(typeof(RoomIdUtils), nameof(RoomIdUtils.PositionToCoords)))) + offset;

            // new Room(gameObject)
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(Room))[0]),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
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

            Room.RoomIdentifierToRoom.Remove(__instance);
        }
    }
}