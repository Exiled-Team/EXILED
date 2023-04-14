// -----------------------------------------------------------------------
// <copyright file="Attacking.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using Exiled.API.Features.Pools;
using PlayerRoles.PlayableScps;
using PlayerRoles.PlayableScps.Subroutines;

namespace Exiled.Events.Patches.Events.Scp0492
{
    using Exiled.Events.EventArgs.Scp0492;
    using HarmonyLib;
    using Mirror;
    using PlayerRoles;
    using PlayerRoles.FirstPersonControl;
    using PlayerRoles.PlayableScps.Scp049.Zombies;
    using RelativePositioning;
    using UnityEngine;
    using Utils.Networking;
    using Utils.NonAllocLINQ;

    /// <summary>
    /// Patches <see cref="ScpAttackAbilityBase{T}.ServerProcessCmd(NetworkReader)"/>
    /// to add <see cref="Attacking"/> event.
    /// </summary>
    [HarmonyPatch(typeof(ScpAttackAbilityBase<ZombieRole>), nameof(ScpAttackAbilityBase<ZombieRole>.ServerProcessCmd))]
    internal class Attacking
    {
        public static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            LocalBuilder ev = generator.DeclareLocal(typeof(AttackingEventArgs));

            Label ret = generator.DefineLabel();

            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            newInstructions.InsertRange(
                8,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ScpStandardSubroutine<ZombieRole>), nameof(ScpStandardSubroutine<ZombieRole>.Owner))),

                    new(OpCodes.Ldnull),

                    new(OpCodes.Ldc_I4_1),

                    new(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(AttackingEventArgs))[0]),
                    new(OpCodes.Dup),

                    new(OpCodes.Call, AccessTools.Method(typeof(Handlers.Scp0492), nameof(Handlers.Scp0492.OnAttacking))),

                    new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(AttackingEventArgs), nameof(AttackingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),
                });

            int offset = -3;
            int index = newInstructions.FindIndex(x => x.Calls(AccessTools.PropertyGetter(typeof(RelativePosition), nameof(RelativePosition.Position)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    new CodeInstruction(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(ScpStandardSubroutine<ZombieRole>), nameof(ScpStandardSubroutine<ZombieRole>.Owner))),

                    new(OpCodes.Ldloc_S, 4),

                    new(OpCodes.Ldc_I4_1),

                    new(OpCodes.Newobj, AccessTools.GetDeclaredConstructors(typeof(AttackingEventArgs))[0]),
                    new(OpCodes.Dup),

                    new(OpCodes.Call, AccessTools.Method(typeof(Handlers.Scp0492), nameof(Handlers.Scp0492.OnAttacking))),

                    new(OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(AttackingEventArgs), nameof(AttackingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            foreach (var instruction in newInstructions)
                yield return instruction;

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}
