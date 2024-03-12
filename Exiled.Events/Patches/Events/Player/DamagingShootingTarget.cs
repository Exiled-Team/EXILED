// -----------------------------------------------------------------------
// <copyright file="DamagingShootingTarget.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using AdminToys;

    using API.Features;
    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;

    using HarmonyLib;

    using PlayerStatsSystem;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="ShootingTarget.Damage(float, DamageHandlerBase, Vector3)" />.
    /// Adds the <see cref="Handlers.Player.DamagingShootingTarget" /> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Player), nameof(Handlers.Player.DamagingShootingTarget))]
    [HarmonyPatch(typeof(ShootingTarget), nameof(ShootingTarget.Damage))]
    internal static class DamagingShootingTarget
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stloc_2) + offset;

            Label allowedLabel = generator.DefineLabel();

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(hub)
                    new(OpCodes.Ldloc_1),
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.Get), new[] { typeof(ReferenceHub) })),

                    // damage
                    new(OpCodes.Ldarg_1),

                    // distance
                    new(OpCodes.Ldloc_3),

                    // hitLocation
                    new(OpCodes.Ldarg_3),

                    // this
                    new(OpCodes.Ldarg_0),

                    // damageHandler
                    new(OpCodes.Ldarg_2),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // DamagingShootingTargetEventArgs ev = new(Player, float, float, Vector3, ShootingTarget, DamageHandlerBase, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(DamagingShootingTargetEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),

                    // Player.OnDamagingShootingTarget(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Player), nameof(Handlers.Player.OnDamagingShootingTarget))),

                    // if (ev.IsAllowed)
                    //    goto allowedLabel;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingShootingTargetEventArgs), nameof(DamagingShootingTargetEventArgs.IsAllowed))),
                    new(OpCodes.Brtrue_S, allowedLabel),

                    // Pop the ev still in the stack
                    new(OpCodes.Pop),

                    // return false;
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Ret),

                    // allowedLabel:
                    //
                    // damage = ev.Amount;
                    new CodeInstruction(OpCodes.Dup).WithLabels(allowedLabel),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingShootingTargetEventArgs), nameof(DamagingShootingTargetEventArgs.Amount))),
                    new(OpCodes.Starg_S, 1),

                    // distance = ev.Distance;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(DamagingShootingTargetEventArgs), nameof(DamagingShootingTargetEventArgs.Distance))),
                    new(OpCodes.Stloc_2),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}