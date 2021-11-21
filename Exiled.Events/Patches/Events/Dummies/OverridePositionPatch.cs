// -----------------------------------------------------------------------
// <copyright file="OverridePositionPatch.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Dummies
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayerMovementSync.OverridePosition(Vector3, float, bool)"/>.
    /// </summary>
    [HarmonyPatch(typeof(PlayerMovementSync), nameof(PlayerMovementSync.OverridePosition))]
    internal static class OverridePositionPatch
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            LocalBuilder mem_0x01 = generator.DeclareLocal(typeof(Vector3));
            LocalBuilder mem_0x02 = generator.DeclareLocal(typeof(float));

            Label ret = generator.DefineLabel();
            Label jmp = generator.DefineLabel();

            int offset = -2;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Starg_S) + offset;

            CodeInstruction ins_0x01 = newInstructions[index];

            offset = 1;
            index += offset;

            CodeInstruction ins_0x02 = newInstructions[index];

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            offset = 1;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Brfalse_S) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PlayerMovementSync), nameof(PlayerMovementSync.gameObject))),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(GameObject) })),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(API.Features.Player), nameof(API.Features.Player.IsDummy))),
                new CodeInstruction(OpCodes.Brfalse_S, jmp),
                new CodeInstruction(OpCodes.Ldloca_S, 0),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(RaycastHit), nameof(RaycastHit.point))),
                new CodeInstruction(OpCodes.Call, PropertyGetter(typeof(Vector3), nameof(Vector3.up))),
                new CodeInstruction(OpCodes.Ldc_R4, 1.23f),
                ins_0x01,
                ins_0x02,
                new CodeInstruction(OpCodes.Starg_S, 1),
                new CodeInstruction(OpCodes.Ldc_R4, 1f),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(PlayerMovementSync), nameof(PlayerMovementSync._hub))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(ReferenceHub), nameof(ReferenceHub.transform))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Transform), nameof(Transform.localScale))),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Vector3), nameof(Vector3.y))),
                new CodeInstruction(OpCodes.Sub),
                new CodeInstruction(OpCodes.Ldc_R4, 1.3f),
                new CodeInstruction(OpCodes.Mul),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Vector3), nameof(Vector3.x))),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldfld, Field(typeof(Vector3), nameof(Vector3.z))),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(Vector3))[1]),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Pop),
                new CodeInstruction(OpCodes.Ldarg_0),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(PlayerMovementSync), nameof(PlayerMovementSync.ForcePosition), new[] { typeof(Vector3) })),
                new CodeInstruction(OpCodes.Br_S, ret),
            });

            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldarg_1) + offset;

            newInstructions[index].labels.Add(jmp);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
