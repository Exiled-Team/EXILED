// -----------------------------------------------------------------------
// <copyright file="RagdollCleanUpPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
#pragma warning disable 1584
#pragma warning disable SA1118 // Parameter should not span multiple lines
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="global::Ragdoll.UpdateCleanup"/>.
    /// </summary>
    [HarmonyPatch(typeof(global::Ragdoll), nameof(global::Ragdoll.UpdateCleanup))]
    internal class RagdollCleanUpPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label ret = generator.DefineLabel();

            newInstructions.InsertRange(0, new CodeInstruction[]
            {
                new(OpCodes.Call, PropertyGetter(typeof(Ragdoll), nameof(Ragdoll.IgnoredRagdolls))),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Callvirt, Method(typeof(HashSet<global::Ragdoll>), nameof(HashSet<global::Ragdoll>.Contains))),
                new(OpCodes.Brtrue_S, ret),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
