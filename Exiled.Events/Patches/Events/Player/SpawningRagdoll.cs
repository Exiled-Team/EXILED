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
    using Exiled.API.Features;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;
    using PlayerRoles.Ragdolls;

    using PlayerStatsSystem;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    using Player = Handlers.Player;

    /// <summary>
    ///     Patches <see cref="RagdollManager.ServerSpawnRagdoll(ReferenceHub, DamageHandlerBase)" />.
    ///     <br>Adds the <see cref="Player.SpawningRagdoll" /> and <see cref="Player.SpawnedRagdoll"/> events.</br>
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.SpawningRagdoll))]
    [EventPatch(typeof(Player), nameof(Player.SpawnedRagdoll))]
    [HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.ServerSpawnRagdoll))]
    internal static class SpawningRagdoll
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label ret = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningRagdollEventArgs));
            LocalBuilder newRagdoll = generator.DeclareLocal(typeof(Ragdoll));
            LocalBuilder localScale = generator.DeclareLocal(typeof(Vector3));
            LocalBuilder evScale = generator.DeclareLocal(typeof(Vector3));
            LocalBuilder targetScale = generator.DeclareLocal(typeof(Vector3));

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_1) + offset;

            newInstructions.RemoveRange(index, 7);

            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Ldloc_1);

            newInstructions.InsertRange(index, new[]
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

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // ev.Info
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.Info))),

                // new Vector3()
                new(OpCodes.Ldloca_S, targetScale.LocalIndex),
                new(OpCodes.Initobj, typeof(Vector3)),

                // localScale = ragdoll.gameObject.transform.localScale
                new(OpCodes.Ldloc_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(BasicRagdoll), nameof(BasicRagdoll.gameObject))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GameObject), nameof(GameObject.transform))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Transform), nameof(Transform.localScale))),
                new(OpCodes.Stloc_S, localScale.LocalIndex),

                // evScale = ev.Scale
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.Scale))),
                new(OpCodes.Stloc_S, evScale.LocalIndex),

                // targetScale.x = evScale.x * localScale.x
                new(OpCodes.Ldloca_S, targetScale.LocalIndex),
                new(OpCodes.Ldloc_S, evScale.LocalIndex),
                new(OpCodes.Ldfld, Field(typeof(Vector3), nameof(Vector3.x))),
                new(OpCodes.Ldloc_S, localScale.LocalIndex),
                new(OpCodes.Ldfld, Field(typeof(Vector3), nameof(Vector3.x))),
                new(OpCodes.Mul),
                new(OpCodes.Stfld, Field(typeof(Vector3), nameof(Vector3.x))),

                // targetScale.y = evScale.y * localScale.y
                new(OpCodes.Ldloca_S, targetScale.LocalIndex),
                new(OpCodes.Ldloc_S, evScale.LocalIndex),
                new(OpCodes.Ldfld, Field(typeof(Vector3), nameof(Vector3.y))),
                new(OpCodes.Ldloc_S, localScale.LocalIndex),
                new(OpCodes.Ldfld, Field(typeof(Vector3), nameof(Vector3.y))),
                new(OpCodes.Mul),
                new(OpCodes.Stfld, Field(typeof(Vector3), nameof(Vector3.y))),

                // targetScale.z = evScale.z * localScale.z
                new(OpCodes.Ldloca_S, targetScale.LocalIndex),
                new(OpCodes.Ldloc_S, evScale.LocalIndex),
                new(OpCodes.Ldfld, Field(typeof(Vector3), nameof(Vector3.z))),
                new(OpCodes.Ldloc_S, localScale.LocalIndex),
                new(OpCodes.Ldfld, Field(typeof(Vector3), nameof(Vector3.z))),
                new(OpCodes.Mul),
                new(OpCodes.Stfld, Field(typeof(Vector3), nameof(Vector3.z))),

                // ragdoll.gameObject.transform.localScale = targetScale
                new(OpCodes.Ldloc_1),
                new(OpCodes.Callvirt, PropertyGetter(typeof(BasicRagdoll), nameof(BasicRagdoll.gameObject))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GameObject), nameof(GameObject.transform))),
                new(OpCodes.Ldloc_S, targetScale.LocalIndex),
                new(OpCodes.Callvirt, PropertySetter(typeof(Transform), nameof(Transform.localScale))),
            });

            newInstructions.InsertRange(newInstructions.Count - 2, new CodeInstruction[]
            {
                // ev.Player
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.Player))),

                // Ragdoll::Get(basicRagdoll)
                new(OpCodes.Ldloc_1),
                new(OpCodes.Call, Method(typeof(Ragdoll), nameof(Ragdoll.Get), new[] { typeof(BasicRagdoll) })),

                // ev.Info
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.Info))),

                // ev.DamageHandlerBase
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.DamageHandlerBase))),

                // Player::OnSpawnedRagdoll(new SpawnedRagdollEventArgs(ev.Player, Ragdoll::Get(basicRagdoll), ev.Info, ev.DamageHandlerBase))
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawnedRagdollEventArgs))[0]),
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnSpawnedRagdoll))),
            });

            newInstructions[newInstructions.Count - 1].labels.Add(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}