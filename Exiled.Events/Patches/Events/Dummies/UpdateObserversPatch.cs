// -----------------------------------------------------------------------
// <copyright file="UpdateObserversPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Dummies
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using RemoteAdmin;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayableScps.Scp173.UpdateObservers"/> to prevent counting dummies as looking at Scp173.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.UpdateObservers))]
    internal static class UpdateObserversPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Br);

            Label rep = (Label)newInstructions[index].operand;

            const int offset = 1;
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_2) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloca_S, 2),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(KeyValuePair<GameObject, ReferenceHub>), nameof(KeyValuePair<GameObject, ReferenceHub>.Key))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.IsDummy))),
                new CodeInstruction(OpCodes.Brtrue_S, rep),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
