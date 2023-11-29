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
    using PlayerRoles.FirstPersonControl.NetworkMessages;
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

            Label skipLabel = generator.DefineLabel();

            newInstructions[0].labels.Add(skipLabel);

            newInstructions.InsertRange(
                0,
                new CodeInstruction[]
                {
                    // if (newRole is IFpcRole)
                    //  goto skipLabel
                    new(OpCodes.Ldarg_3),
                    new(OpCodes.Isinst, typeof(IFpcRole)),
                    new(OpCodes.Brtrue_S, skipLabel),

                    // Player.Get(hub)
                    new(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),

                    // Player::Position
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Player), nameof(Player.Position))),

                    // 0f
                    new(OpCodes.Ldc_R4, 0f),

                    // oldRole
                    new(OpCodes.Ldarg_2),

                    // SpawningEventArgs(Player, Vector3, 0, PlayerRoleBase)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningEventArgs))[0]),

                    // Handlers.Player.OnSpawning(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawning))),
                });

            int index = newInstructions.FindLastIndex(x => x.opcode == OpCodes.Ldarg_1);

            IEnumerable<Label> labels = newInstructions[index].labels;

            newInstructions.RemoveRange(index, 9);

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(hub)
                    new CodeInstruction(OpCodes.Ldarg_1).WithLabels(labels),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // position
                    new(OpCodes.Ldloc_1),

                    // rotation
                    new(OpCodes.Ldloc_2),

                    // oldRole
                    new(OpCodes.Ldarg_2),

                    // SpawningEventArgs(Player, Vector3, float, PlayerRoleBase)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnSpawning(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawning))),

                    // Set position and horizontal rotation
                    new(OpCodes.Call, Method(typeof(Spawning), nameof(Send))),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }

        private static void Send(SpawningEventArgs ev) => ev.Player.ReferenceHub.connectionToClient.Send(new FpcOverrideMessage(ev.Position, ev.HorizontalRotation));
    }
}
