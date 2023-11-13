// -----------------------------------------------------------------------
// <copyright file="Spawning.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features;
    using Exiled.API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using HarmonyLib;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.FirstPersonControl.Spawnpoints;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="RoleSpawnpointManager.Init"/> delegate.
    /// Adds the <see cref="Handlers.Player.Spawning"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.Spawning))]
    [HarmonyPatch]
    internal static class Spawning
    {
        private static MethodInfo TargetMethod()
        {
            return Method(TypeByName("PlayerRoles.FirstPersonControl.Spawnpoints.RoleSpawnpointManager").GetNestedTypes(all)[1], "<Init>b__2_0");
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldarg_1);

            Label skipLabel = generator.DefineLabel();

            newInstructions[0].labels.Add(skipLabel);

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    new(OpCodes.Ldarg_3),
                    new(OpCodes.Isinst, typeof(IFpcRole)),
                    new(OpCodes.Brtrue_S, skipLabel),

                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Position))),

                    new(OpCodes.Ldc_R4, 0f),

                    new(OpCodes.Ldarg_2),

                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningEventArgs))[0]),

                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawning))),
                });

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    new(OpCodes.Ldloc_1),

                    new(OpCodes.Ldloc_2),

                    new(OpCodes.Ldarg_2),

                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawning))),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.Position))),
                    new(OpCodes.Stloc_1),

                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.HorizontalRotation))),
                    new(OpCodes.Stloc_2),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}