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

    using Exiled.API.Features;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using NorthwoodLib.Pools;

    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.FirstPersonControl.Spawnpoints;
    using PlayerRoles.PlayableScps.Scp049;
    using PlayerRoles.PlayableScps.Subroutines;
    using UnityEngine;

    using static HarmonyLib.AccessTools;

#pragma warning disable SA1649
#pragma warning disable SA1402

    /// <summary>
    /// Patches <see cref="RoleSpawnpointManager.Init"/> delegate.
    /// Adds the <see cref="Handlers.Player.Spawning"/> event for fps roles without 0492.
    /// </summary>
    [HarmonyPatch]
    internal static class SpawningForNormalRoles
    {
        private static MethodInfo TargetMethod()
        {
            return Method(TypeByName("PlayerRoles.FirstPersonControl.Spawnpoints.RoleSpawnpointManager").GetNestedTypes(all)[1], "<Init>b__2_0");
        }

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label continueLabel = generator.DefineLabel();
            Label returnLabel = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(Player));

            const int offset = 0;
            int index = newInstructions.FindLastIndex(instruction => instruction.IsLdarg(1)) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(hub)
                    //
                    // if (player == null)
                    //    goto continueLabel;
                    new CodeInstruction(OpCodes.Ldarg_1).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // position
                    new(OpCodes.Ldloc_1),

                    // SpawningEventArgs ev = new(player, position)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Handlers.Player.OnSpawning(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawning))),

                    // position = ev.Position
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.Position))),
                    new(OpCodes.Stloc_1),

                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(returnLabel);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }

    /// <summary>
    /// Patches <see cref="RoleSpawnpointManager.Init"/> delegate.
    /// Adds the <see cref="Handlers.Player.Spawning"/> event for 0492.
    /// </summary>
    [HarmonyPatch(typeof(Scp049ResurrectAbility), nameof(Scp049ResurrectAbility.ServerComplete))]
    internal static class SpawningForScp0492
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label continueLabel = generator.DefineLabel();

            LocalBuilder player = generator.DeclareLocal(typeof(Player));
            LocalBuilder eventArgs = generator.DeclareLocal(typeof(SpawningEventArgs));

            const int toRemove = 7;
            int offset = -1;
            int index = newInstructions.FindLastIndex(instruction => instruction.Calls(PropertyGetter(typeof(Component), nameof(Component.transform)))) + offset;

            newInstructions[index + toRemove].MoveLabelsFrom(newInstructions[index]);

            newInstructions.RemoveRange(index, toRemove);

            offset = 1;
            index = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(PlayerRoleManager), nameof(PlayerRoleManager.ServerSetRole)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(hub)
                    //
                    // if (player == null)
                    //    goto continueLabel;
                    new CodeInstruction(OpCodes.Ldloc_0),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, player.LocalIndex),
                    new(OpCodes.Brfalse_S, continueLabel),

                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // this.ScpRole.FpcModule.Position
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ScpStandardSubroutine<Scp049Role>), nameof(ScpStandardSubroutine<Scp049Role>.ScpRole))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FpcStandardRoleBase), nameof(FpcStandardRoleBase.FpcModule))),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(FirstPersonMovementModule), nameof(FirstPersonMovementModule.Position))),

                    // SpawningEventArgs ev = new(player, this.ScpRole.FpcModule.Position)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SpawningEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, eventArgs.LocalIndex),

                    // Handlers.Player.OnSpawning(ev);
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnSpawning))),

                    // hub.transform.position = ev.Position
                    new(OpCodes.Ldloc_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Component), nameof(Component.transform))),
                    new(OpCodes.Ldloc_S, eventArgs.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(SpawningEventArgs), nameof(SpawningEventArgs.Position))),
                    new(OpCodes.Callvirt, PropertySetter(typeof(Transform), nameof(Transform.position))),

                    new CodeInstruction(OpCodes.Nop).WithLabels(continueLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}