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

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using Handlers;

    using HarmonyLib;

    using PlayerRoles.Ragdolls;

    using PlayerStatsSystem;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="RagdollManager.ServerSpawnRagdoll(ReferenceHub, DamageHandlerBase)" />.
    ///     Adds the <see cref="Player.SpawningRagdoll" /> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.SpawningRagdoll))]
    [HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.ServerSpawnRagdoll))]
    internal static class SpawningRagdoll
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningRagdollEventArgs));
            LocalBuilder newRagdoll = generator.DeclareLocal(typeof(API.Features.Ragdoll));

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_1) + offset;

            newInstructions.RemoveRange(index, 7);

            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_1);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // hub
                    new CodeInstruction(OpCodes.Ldarg_0),

                    // handler
                    new(OpCodes.Ldarg_1),

                    // ragdollRole.transform.localPosition
                    new(OpCodes.Ldloc_2),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Transform), nameof(Transform.localPosition))),

                    // ragdollRole.transform.localRotation
                    new(OpCodes.Ldloc_2),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Transform), nameof(Transform.localRotation))),

                    // new RagdollInfo(ReferenceHub, DamageHandlerBase, Vector3, Quaternion)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(RagdollData))[0]),

                    // handler
                    new(OpCodes.Ldarg_1),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // SpawningRagdollEventArgs ev = new(RagdollInfo, DamageHandlerBase, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningRagdollEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Player.OnSpawningRagdoll(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnSpawningRagdoll))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),
                });

            // Search the index in which our logic will be injected
            offset = 0;
            index = newInstructions.FindIndex(instruction => instruction.Calls(PropertySetter(typeof(BasicRagdoll), nameof(BasicRagdoll.NetworkInfo)))) + offset;

            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // ev.Info
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.Info))),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}