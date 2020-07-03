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
    /// Patches <see cref="CharacterClassManager.NetworkCurClass"/>.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), "set_" + nameof(CharacterClassManager.NetworkCurClass))]
    public static class DoubleSpawn
    {
        /// <summary>
        /// Fix the <see cref="CharacterClassManager.NetworkCurClass"/> property.
        /// </summary>
        /// <param name="instructions">The list of property OpCodes.</param>
        /// <returns>Used to wait.</returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
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
