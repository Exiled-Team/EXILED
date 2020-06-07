// -----------------------------------------------------------------------
// <copyright file="CharacterClassManagerLoadSpam.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
#pragma warning disable SA1313
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;
    using HarmonyLib;
    using Mirror;

    /// <summary>
    /// Patches <see cref="CharacterClassManager.Start"/>.
    /// </summary>
    [HarmonyPatch(typeof(CharacterClassManager), nameof(CharacterClassManager.Start))]
    public static class CharacterClassManagerLoadSpam
    {
        /// <summary>
        /// Fix the <see cref="NetworkBehaviour.isServer"/> property.
        /// </summary>
        /// <param name="instructions">The list of OpCodes.</param>
        /// <returns>Used to wait.</returns>
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            bool isFirst = false;

            foreach (CodeInstruction instruction in instructions)
            {
                if (!isFirst
                    && instruction.opcode == OpCodes.Call
                    && instruction.operand != null
                    && instruction.operand is MethodBase methodBase
                    && methodBase.Name == "get_" + nameof(NetworkBehaviour.isServer))
                {
                    isFirst = true;
                    yield return new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(CharacterClassManager), "get_" + nameof(CharacterClassManager.isLocalPlayer)));
                }
                else
                {
                    yield return instruction;
                }
            }
        }
    }
}
