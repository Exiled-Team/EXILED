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

    using Exiled.API.Features.Pools;

    using HarmonyLib;

    using MapGeneration;
    using MapGeneration.Distributors;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoomIdentifier.Awake"/>.
    /// </summary>
    [HarmonyPatch(typeof(RoomIdentifier), nameof(RoomIdentifier.Awake))]
    internal class RoomList
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> codeInstructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(codeInstructions);

            // Room.CreateComponent(gameObject);
            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.gameObject))),
                    new(OpCodes.Call, Method(typeof(Room), nameof(Room.CreateComponent))),
                    new(OpCodes.Pop),
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
            Room room = Room.RoomIdentifierToRoom[__instance];

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