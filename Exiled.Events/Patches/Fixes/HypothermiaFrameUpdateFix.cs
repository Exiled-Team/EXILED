// -----------------------------------------------------------------------
// <copyright file="HypothermiaFrameUpdateFix.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1118 // Parameter should not span multiple lines

    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using InventorySystem.Items.Usables.Scp244;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp244HypothermiaHandler.HandlePlayer(ReferenceHub)"/>.
    /// Prevents the effect to be updated every frame if the previous intensity is the same as the new one.
    /// </summary>
    [HarmonyPatch(typeof(Scp244HypothermiaHandler), nameof(Scp244HypothermiaHandler.HandlePlayer))]
    internal static class HypothermiaFrameUpdateFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_1);
            CodeInstruction m_ins = newInstructions[index];

            LocalBuilder cachedIntensity = generator.DeclareLocal(typeof(byte));

            Label retLabel = generator.DefineLabel();

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldc_R4, 10),
                new CodeInstruction(OpCodes.Mul),
                new CodeInstruction(OpCodes.Call, Method(typeof(UnityEngine.Mathf), nameof(UnityEngine.Mathf.RoundToInt))),
                new CodeInstruction(OpCodes.Ldc_I4_0),
                new CodeInstruction(OpCodes.Ldc_I4, 255),
                new CodeInstruction(OpCodes.Call, Method(typeof(UnityEngine.Mathf), nameof(UnityEngine.Mathf.Clamp), new[] { typeof(int), typeof(int), typeof(int) })),
                new CodeInstruction(OpCodes.Conv_U1),
                new CodeInstruction(OpCodes.Stloc_S, cachedIntensity.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, cachedIntensity.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue_S, retLabel),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldloc_S, cachedIntensity.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertySetter(typeof(CustomPlayerEffects.Hypothermia), nameof(CustomPlayerEffects.Hypothermia.Intensity))),
            });

            newInstructions.RemoveRange(newInstructions.IndexOf(m_ins), 10);

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
