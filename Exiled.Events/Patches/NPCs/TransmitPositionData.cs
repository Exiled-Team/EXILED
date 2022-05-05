// -----------------------------------------------------------------------
// <copyright file="TransmitPositionData.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.NPCs
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Reflection.Emit;
    using Exiled.API.Extensions;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using UnityEngine;

#pragma warning disable SA1118
    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerPositionManager.TransmitData"/> to prevent the updating of networked positions on npcs.
    /// </summary>
    [HarmonyPatch(typeof(PlayerPositionManager), nameof(PlayerPositionManager.TransmitData))]
    internal static class TransmitPositionData
    {
        private static List<GameObject> GetPlayers => PlayerManager.players.Where(gameObject => !gameObject.IsNpc()).ToList();

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldsfld);

            newInstructions.RemoveAt(index);
            newInstructions.Insert(index, new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(TransmitPositionData), nameof(GetPlayers))));

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
