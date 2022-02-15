// -----------------------------------------------------------------------
// <copyright file="RagdollCleanupFix.cs" company="SEXiled Team">
// Copyright (c) SEXiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace SEXiled.Events.Patches.Fixes
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Ragdoll.UpdateCleanup"/>.
    /// <para>Fixes <see cref="API.Features.Ragdoll"/>s not being removed from <see cref="API.Features.Map.Ragdolls"/> when they have already been cleaned up.</para>
    /// </summary>
    [HarmonyPatch(typeof(Ragdoll), nameof(Ragdoll.UpdateCleanup))]
    internal static class RagdollCleanupFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(API.Features.Map), nameof(API.Features.Map.RagdollsValue))),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Ragdoll), nameof(API.Features.Ragdoll.Get), new[] { typeof(Ragdoll) })),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(List<API.Features.Ragdoll>), nameof(List<API.Features.Ragdoll>.Remove))),
                new CodeInstruction(OpCodes.Pop),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
