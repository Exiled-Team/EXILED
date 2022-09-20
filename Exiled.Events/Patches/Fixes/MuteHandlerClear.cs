// -----------------------------------------------------------------------
// <copyright file="MuteHandlerClear.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Fixes
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Fixes <see cref="MuteHandler.Reload"/> method.
    /// </summary>
    [HarmonyPatch(typeof(MuteHandler), nameof(MuteHandler.Reload))]
    internal static class MuteHandlerClear
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            newInstructions.InsertRange(
                0,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldsfld, Field(typeof(MuteHandler), nameof(MuteHandler.Mutes))),
                    new(OpCodes.Callvirt, Method(typeof(HashSet<string>), nameof(HashSet<string>.Clear))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}