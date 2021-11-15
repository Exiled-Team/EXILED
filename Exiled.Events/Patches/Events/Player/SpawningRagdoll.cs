// -----------------------------------------------------------------------
// <copyright file="SpawningRagdoll.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
#pragma warning disable SA1118
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RagdollManager.SpawnRagdoll(Vector3, Quaternion, Vector3, int, PlayerStats.HitInfo, bool, string, string, int, bool)"/>.
    /// Adds the <see cref="SpawningRagdoll"/> event.
    /// </summary>
    [HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.SpawnRagdoll))]
    internal static class SpawningRagdoll
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 4;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Dup) + offset;

            Label ret = generator.DefineLabel();
            Label cmova = generator.DefineLabel();
            Label jmp = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningRagdollEventArgs));
            LocalBuilder mem_0x01 = generator.DeclareLocal(typeof(API.Features.Player));
            LocalBuilder mem_0x02 = generator.DeclareLocal(typeof(int));
            LocalBuilder mem_0x03 = generator.DeclareLocal(typeof(API.Features.Ragdoll));
            LocalBuilder mem_0x04 = generator.DeclareLocal(typeof(Ragdoll));
            LocalBuilder mem_0x05 = generator.DeclareLocal(typeof(GameObject));

            MethodInfo getComponent = Method(typeof(GameObject), nameof(GameObject.GetComponent)).MakeGenericMethod(typeof(Ragdoll));

            newInstructions.InsertRange(0, new[]
            {
                new CodeInstruction(OpCodes.Ldarga_S, 5),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(PlayerStats.HitInfo), nameof(PlayerStats.HitInfo.PlayerId))),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Brfalse_S, cmova),
                new CodeInstruction(OpCodes.Ldarg_S, 9),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(int) })),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Br_S, jmp),
                new CodeInstruction(OpCodes.Ldnull).WithLabels(cmova),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x01.LocalIndex).WithLabels(jmp),
                new CodeInstruction(OpCodes.Ldarg_S, 9),
                new CodeInstruction(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(int) })),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldarg_2),
                new CodeInstruction(OpCodes.Ldarg_3),
                new CodeInstruction(OpCodes.Ldarg_S, 4),
                new CodeInstruction(OpCodes.Ldarg_S, 5),
                new CodeInstruction(OpCodes.Ldarg_S, 6),
                new CodeInstruction(OpCodes.Ldarg_S, 7),
                new CodeInstruction(OpCodes.Ldarg_S, 8),
                new CodeInstruction(OpCodes.Ldarg_S, 9),
                new CodeInstruction(OpCodes.Ldarg_S, 10),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningRagdollEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawningRagdoll))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.Position))),
                new CodeInstruction(OpCodes.Starg_S, 1),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.Rotation))),
                new CodeInstruction(OpCodes.Starg_S, 2),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.Velocity))),
                new CodeInstruction(OpCodes.Starg_S, 3),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.RoleType))),
                new CodeInstruction(OpCodes.Starg_S, 4),
                new CodeInstruction(OpCodes.Ldloca_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.HitInformations))),
                new CodeInstruction(OpCodes.Starg_S, 5),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.IsRecallAllowed))),
                new CodeInstruction(OpCodes.Starg_S, 6),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.DissonanceId))),
                new CodeInstruction(OpCodes.Starg_S, 7),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.PlayerNickname))),
                new CodeInstruction(OpCodes.Starg_S, 8),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.PlayerId))),
                new CodeInstruction(OpCodes.Starg_S, 9),
                new CodeInstruction(OpCodes.Ldloc_S, ev.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.Scp096Death))),
                new CodeInstruction(OpCodes.Starg_S, 10),
            });

            offset = -1;
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Call &&
            (MethodInfo)instruction.operand == Method(typeof(Mirror.NetworkServer), nameof(Mirror.NetworkServer.Spawn), new[] { typeof(GameObject), typeof(Mirror.NetworkConnection) })) + offset;

            newInstructions.Insert(index, new CodeInstruction(OpCodes.Stloc_S, mem_0x05.LocalIndex));

            offset = -1;
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Call &&
            (MethodInfo)instruction.operand == Method(typeof(Mirror.NetworkServer), nameof(Mirror.NetworkServer.Spawn), new[] { typeof(GameObject), typeof(Mirror.NetworkConnection) })) + offset;

            newInstructions.RemoveRange(index, 2);

            offset = 2;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Callvirt) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x04.LocalIndex),
            });

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x04.LocalIndex),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(API.Features.Ragdoll))[2]),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x05.LocalIndex),
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Call, Method(typeof(Mirror.NetworkServer), nameof(Mirror.NetworkServer.Spawn), new[] { typeof(GameObject), typeof(Mirror.NetworkConnection) })),
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(API.Features.Map), nameof(API.Features.Map.RagdollsValue))),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x03.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(List<API.Features.Ragdoll>), nameof(List<API.Features.Ragdoll>.Add))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
