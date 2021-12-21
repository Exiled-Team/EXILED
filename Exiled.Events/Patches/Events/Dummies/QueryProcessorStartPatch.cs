// -----------------------------------------------------------------------
// <copyright file="QueryProcessorStartPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Dummies
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using RemoteAdmin;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="QueryProcessor.Start"/> to prevent a NRE caused by the original method from more accessing of connectionToClient on dummies.
    /// </summary>
    [HarmonyPatch(typeof(QueryProcessor), nameof(QueryProcessor.Start))]
    internal static class QueryProcessorStartPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stfld &&
            (FieldInfo)instruction.operand == Field(typeof(QueryProcessor), nameof(QueryProcessor._ipAddress))) + offset;

            Label jmp = generator.DefineLabel();

            newInstructions[index].labels.Add(jmp);

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(QueryProcessor), nameof(QueryProcessor.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.IsDummy))),
                new CodeInstruction(OpCodes.Brtrue_S, jmp),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
