// -----------------------------------------------------------------------
// <copyright file="DoubleSpawn.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1313
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    /// <summary>
    /// Fixes <see cref="CharacterClassManager.NetworkCurClass"/> property.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.NetworkCurClass), MethodType.Setter)]
    internal static class DoubleSpawn
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool isNOPDetected = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (instruction.opcode == OpCodes.Nop)
                    isNOPDetected = true;

                if (!isNOPDetected)
                    yield return new CodeInstruction(OpCodes.Nop);
                else
                    yield return instruction;
            }
        }
    }
}
