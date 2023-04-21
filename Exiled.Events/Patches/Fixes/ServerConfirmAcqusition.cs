// -----------------------------------------------------------------------
// <copyright file="ServerConfirmAcqusition.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using HarmonyLib;
    using InventorySystem.Items.Firearms;
    using Mirror;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Firearm.ServerConfirmAcqusition"/> to prevent the acquisition of firearms by NPCs (Null Connections).
    /// </summary>
    [HarmonyPatch(typeof(Firearm), nameof(Firearm.ServerConfirmAcqusition))]
    internal static class ServerConfirmAcqusition
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> OnConfirmingAcqusition(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label defLabel = generator.DefineLabel();
            var index = newInstructions.FindIndex(instruction => instruction.OperandIs(PropertyGetter(typeof(NetworkBehaviour), nameof(NetworkBehaviour.connectionToClient)))) + 1;
            newInstructions[index].labels.Add(defLabel);

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                new(OpCodes.Dup),
                new(OpCodes.Brtrue_S, defLabel),
                new(OpCodes.Pop),
                new(OpCodes.Ret),
            });
            foreach (var instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}