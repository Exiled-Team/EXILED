// -----------------------------------------------------------------------
// <copyright file="Scp096PreWindupFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1118
#pragma warning disable SA1402
    using System.Collections.Generic;
    using System.Reflection.Emit;
    using HarmonyLib;
    using NorthwoodLib.Pools;
    using PlayableScps;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// patches <see cref="Scp096.PreWindup"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.PreWindup))]
    internal static class Scp096PreWindupFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 1;

            // Find the only "throw" call
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Throw) + offset;

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp096), nameof(Scp096._targets))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Count))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="Scp096.Windup"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.Windup))]
    internal static class Scp096WindupFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 1;

            // Find the only "throw" call
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Throw) + offset;

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Scp096), nameof(Scp096._targets))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Count))),
                new CodeInstruction(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
