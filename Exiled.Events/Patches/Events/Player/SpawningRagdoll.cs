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

    using API.Features.Core.Generic.Pools;
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
    /// Patches <see cref="RagdollManager.ServerSpawnRagdoll(ReferenceHub, DamageHandlerBase)" />.
    /// <br>Adds the <see cref="Player.SpawningRagdoll" /> and <see cref="Player.SpawnedRagdoll"/> events.</br>
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.SpawningRagdoll))]
    [EventPatch(typeof(Player), nameof(Player.SpawnedRagdoll))]
    [HarmonyPatch(typeof(RagdollManager), nameof(RagdollManager.ServerSpawnRagdoll))]
    internal static class SpawningRagdoll
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label continueLabel = generator.DefineLabel();

            LocalBuilder ev = generator.DeclareLocal(typeof(SpawningRagdollEventArgs));

            int index = newInstructions.FindIndex(instruction => instruction.Calls(PropertySetter(typeof(BasicRagdoll), nameof(BasicRagdoll.NetworkInfo))));

            newInstructions.InsertRange(index, new[]
            {
                // true
                new(OpCodes.Ldc_I4_1),

                // SpawningRagdollEventArgs args = new(RagdollInfo, bool)
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningRagdollEventArgs))[0]),
                new(OpCodes.Dup),
                new(OpCodes.Dup),
                new(OpCodes.Stloc_S, ev.LocalIndex),

                // Player::OnSpawningRagdoll(args)
                new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnSpawningRagdoll))),

                // if (!args.IsAllowed)
                // {
                //     Object.Destroy(gameObject);
                //     return null;
                // }
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, continueLabel),

                new(OpCodes.Pop),
                new(OpCodes.Call, Method(typeof(Object), nameof(Object.Destroy), new[] { typeof(Object) })),
                new(OpCodes.Ldnull),
                new(OpCodes.Ret),

                // ragdoll transform
                new CodeInstruction(OpCodes.Ldloc_1).WithLabels(continueLabel),
                new(OpCodes.Callvirt, PropertyGetter(typeof(BasicRagdoll), nameof(BasicRagdoll.gameObject))),
                new(OpCodes.Callvirt, PropertyGetter(typeof(GameObject), nameof(GameObject.transform))),

                // ragdoll localScale
                new(OpCodes.Dup),
                new(OpCodes.Callvirt, PropertyGetter(typeof(Transform), nameof(Transform.localScale))),

                // SpawningRagdollEventArgs::Scale
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.Scale))),

                // newScale = Vector3::Scale(ragdollScale, SpawningRagdollEventArgs::Scale);
                new(OpCodes.Call, Method(typeof(Vector3), nameof(Vector3.Scale), new[] { typeof(Vector3), typeof(Vector3) })),

                // BasicRagdoll::gameObject::transform::localScale = targetScale
                new(OpCodes.Callvirt, PropertySetter(typeof(Transform), nameof(Transform.localScale))),

                // load ragdollInfo
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningRagdollEventArgs), nameof(SpawningRagdollEventArgs.Info))),
            });

            newInstructions.InsertRange(newInstructions.Count - 2, new CodeInstruction[]
            {
                // SpawningRagdollEventArgs::Player
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

            foreach (CodeInstruction instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}