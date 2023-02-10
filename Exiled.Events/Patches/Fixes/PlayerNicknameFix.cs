// -----------------------------------------------------------------------
// <copyright file="PlayerNicknameFix.cs" company="Exiled Team">
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

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch <see cref="NicknameSync.CombinedName"/> to fix the player nickname when changing it.
    /// </summary>
    [HarmonyPatch(typeof(NicknameSync), nameof(NicknameSync.CombinedName), MethodType.Getter)]
    internal static class PlayerNicknameFix
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get();

            newInstructions.AddRange(new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(NicknameSync), nameof(NicknameSync.MyNick))),
                new CodeInstruction(OpCodes.Ret),
            });

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}