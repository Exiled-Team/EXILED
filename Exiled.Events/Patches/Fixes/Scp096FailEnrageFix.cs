// -----------------------------------------------------------------------
// <copyright file="Scp096FailEnrageFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1118
#pragma warning disable SA1402
#pragma warning disable SA1649
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;
    using NorthwoodLib.Pools;
    using PlayableScps;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Handles building new instructions.
    /// </summary>
    internal static class InstructionBuilder
    {
        /// <summary>
        /// Builds new instructions for enrage fix transpilers.
        /// </summary>
        /// <param name="instructions"><see cref="CodeInstruction"/>.</param>
        /// <param name="generator"><see cref="ILGenerator"/>.</param>
        /// <returns>New <see cref="CodeInstruction"/>.</returns>
        internal static IEnumerable<CodeInstruction> FixInstructions(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            const int offset = 1;

            // Find the only "throw" call
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Throw) + offset;

            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Ldfld, Field(typeof(Scp096), nameof(Scp096._targets))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Count))),
                new(OpCodes.Brfalse_S, returnLabel),
            });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    /// <summary>
    /// patches <see cref="Scp096.PreWindup"/>.
    /// </summary>
    [HarmonyPatch(typeof(Scp096), nameof(Scp096.PreWindup))]
    internal static class Scp096FailEnrageFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator) => InstructionBuilder.FixInstructions(instructions, generator);
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
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Throw) + offset;
            Label continueLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Brtrue_S, continueLabel),

                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldfld, Field(typeof(Scp096), nameof(Scp096._targets))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(HashSet<ReferenceHub>), nameof(HashSet<ReferenceHub>.Count))),
                new(OpCodes.Brtrue, continueLabel),
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldc_R4, 0.0f),
                new(OpCodes.Call, PropertySetter(typeof(Scp096), nameof(Scp096.AddedTimeThisRage))),
                new(OpCodes.Ret),
            });

            newInstructions[newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldsfld)].WithLabels(continueLabel);
            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];
            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
