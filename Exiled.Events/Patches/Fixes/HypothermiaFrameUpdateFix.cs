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

            const int offset = 1;
            int index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_1) + offset;

            LocalBuilder cachedIntensity = generator.DeclareLocal(typeof(byte));

            Label retLabel = generator.DefineLabel();

            newInstructions.Insert(index, new CodeInstruction(OpCodes.Pop));

            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Conv_U1) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Stloc_S, cachedIntensity.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, cachedIntensity.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(CustomPlayerEffects.PlayerEffect), nameof(CustomPlayerEffects.PlayerEffect.Intensity))),
                new CodeInstruction(OpCodes.Conv_U1),
                new CodeInstruction(OpCodes.Ceq),
                new CodeInstruction(OpCodes.Brtrue_S, retLabel),
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Ldloc_S, cachedIntensity.LocalIndex),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(retLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
