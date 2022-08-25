// -----------------------------------------------------------------------
// <copyright file="SpawningRagdoll.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using Mirror;

    using NorthwoodLib.Pools;

    using PlayerStatsSystem;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Map = Exiled.API.Features.Map;

    /// <summary>
    ///     Patches <see cref="Ragdoll.ServerSpawnRagdoll(ReferenceHub, DamageHandlerBase)" />.
    ///     Adds the <see cref="Handlers.Player.SpawningRagdoll" /> event.
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

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(GameObject), nameof(GameObject.transform))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Transform), nameof(Transform.localPosition))),
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(GameObject), nameof(GameObject.transform))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Transform), nameof(Transform.localRotation))),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RagdollInfo))[0]),
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Ldc_I4_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningRagdollEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, mem_0x01.LocalIndex),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnSpawningRagdoll))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),
                });

            offset = -1;
            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_1) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_S, mem_0x01.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.Info))),
                });

            index = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Ldloc_1);

            newInstructions.RemoveRange(index, 4);

            newInstructions.InsertRange(
                newInstructions.Count - 1,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(API.Features.Ragdoll))[2]),
                    new(OpCodes.Stloc_S, mem_0x02.LocalIndex),
                    new(OpCodes.Ldloc_1, mem_0x01.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Ragdoll), nameof(Ragdoll.gameObject))),
                    new(OpCodes.Ldnull),
                    new(OpCodes.Call, Method(typeof(NetworkServer), nameof(NetworkServer.Spawn), new[] { typeof(GameObject), typeof(NetworkConnection) })),
                    new(OpCodes.Ldsfld, Field(typeof(Map), nameof(Map.RagdollsValue))),
                    new(OpCodes.Ldloc_S, mem_0x02.LocalIndex),
                    new(OpCodes.Callvirt, Method(typeof(List<API.Features.Ragdoll>), nameof(List<API.Features.Ragdoll>.Add))),
                });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}