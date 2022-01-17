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
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using PlayerStatsSystem;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Ragdoll.ServerSpawnRagdoll(ReferenceHub, DamageHandlerBase)"/>.
    /// Adds the <see cref="Handlers.Player.SpawningRagdoll"/> event.
    /// </summary>
    [HarmonyPatch(typeof(Ragdoll), nameof(Ragdoll.ServerSpawnRagdoll))]
    internal static class SpawningRagdoll
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_1) + offset;

            LocalBuilder mem_0x01 = generator.DeclareLocal(typeof(SpawningRagdollEventArgs));
            LocalBuilder mem_0x02 = generator.DeclareLocal(typeof(API.Features.Ragdoll));

            Label ret = generator.DefineLabel();

            newInstructions.RemoveRange(index, 9);

            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_1);

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(GameObject), nameof(GameObject.transform))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Transform), nameof(Transform.localPosition))),
                new CodeInstruction(OpCodes.Ldloc_0),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(GameObject), nameof(GameObject.transform))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Transform), nameof(Transform.localRotation))),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(RagdollInfo))[0]),
                new CodeInstruction(OpCodes.Ldarg_1),
                new CodeInstruction(OpCodes.Ldc_I4_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningRagdollEventArgs))[0]),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Dup),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawningRagdoll))),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.IsAllowed))),
                new CodeInstruction(OpCodes.Brfalse_S, ret),
            });

            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_1) + offset;

            newInstructions.InsertRange(index, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.Info))),
            });

            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_1);

            newInstructions.RemoveRange(index, 4);

            newInstructions.InsertRange(newInstructions.Count - 1, new[]
            {
                new CodeInstruction(OpCodes.Ldloc_1),
                new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(API.Features.Ragdoll))[2]),
                new CodeInstruction(OpCodes.Stloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Ldloc_1, mem_0x01.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, PropertyGetter(typeof(Ragdoll), nameof(Ragdoll.gameObject))),
                new CodeInstruction(OpCodes.Ldnull),
                new CodeInstruction(OpCodes.Call, Method(typeof(Mirror.NetworkServer), nameof(Mirror.NetworkServer.Spawn), new[] { typeof(GameObject), typeof(Mirror.NetworkConnection) })),
                new CodeInstruction(OpCodes.Ldsfld, Field(typeof(API.Features.Map), nameof(API.Features.Map.RagdollsValue))),
                new CodeInstruction(OpCodes.Ldloc_S, mem_0x02.LocalIndex),
                new CodeInstruction(OpCodes.Callvirt, Method(typeof(List<API.Features.Ragdoll>), nameof(List<API.Features.Ragdoll>.Add))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
