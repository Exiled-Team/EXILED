// -----------------------------------------------------------------------
// <copyright file="ServerVersionPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Generic
{
    using System;
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.API.Features.Pools;
    using HarmonyLib;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patch the <see cref="ServerConsole.RefreshServerData"/>.
    /// </summary>
    [HarmonyPatch(typeof(ServerConsole), nameof(ServerConsole.RefreshServerData))]
    internal static class ServerVersionPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 3;
            int index = newInstructions.FindLastIndex(instruction => instruction.operand == (object)"gameVersion=") + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // Exiled.Events.Events.Instance.Version.ToString(3)
                    new(OpCodes.Call, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Instance))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Exiled.Events.Events), nameof(Exiled.Events.Events.Version))),
                    new(OpCodes.Ldc_I4_3),
                    new(OpCodes.Callvirt, Method(typeof(Version), nameof(Version.ToString), new[] { typeof(int) })),
                    new(OpCodes.Callvirt, Method(typeof(string), nameof(string.Concat), new[] { typeof(string), typeof(string) })),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}