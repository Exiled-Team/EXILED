// -----------------------------------------------------------------------
// <copyright file="SlappingPlayer.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp3114
{
    using PlayerRoles.PlayableScps;

#pragma warning disable SA1402 // File may only contain a single type

    using System;
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Attributes;
    using Exiled.API.Features.Pools;
    using Exiled.Events.EventArgs.Scp3114;
    using Handlers;
    using HarmonyLib;
    using PlayerRoles.PlayableScps.Scp3114;

    using static HarmonyLib.AccessTools;

    /// <summary>
    ///     Patches <see cref="Scp3114Strangle.ServerProcessCmd" />.
    ///     Adds the <see cref="Handlers.Scp3114.StranglingFinished" /> and <see cref="Handlers.Scp3114.StranglingFinished" /> event.
    /// </summary>
    // ReSharper disable UnusedMember.Local
    [EventPatch(typeof(Scp3114), nameof(Scp3114.SlappingPlayer))]
    [HarmonyPatch(typeof(Scp3114Slap), nameof(Scp3114Slap.DamagePlayers))]
    internal sealed class SlappingPlayer
    {
        [HarmonyTranspiler]
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);
            Label skipLabel = generator.DefineLabel();
            var local = generator.DeclareLocal(typeof(SlappingPlayerEventArgs));
            int index = newInstructions.FindIndex(x => x.Calls(PropertyGetter(typeof(ScpAttackAbilityBase<Scp3114Role>), nameof(ScpAttackAbilityBase<Scp3114Role>.DamageAmount))));
            int index2 = newInstructions.FindIndex(x => x.opcode == OpCodes.Ldfld && (FieldInfo)x.operand == Field(typeof(Scp3114Slap), nameof(Scp3114Slap._humeShield))) + 2;
            var injectedInstructions = new CodeInstruction[]
            {
                // ev = new SlappingPlayerArgs(this, targetHub, damage amount, hume to regen, true)
                new(OpCodes.Ldc_R4, 25f),
                new(OpCodes.Ldc_I4_1),
                new(OpCodes.Newobj, GetDeclaredConstructors(typeof(SlappingPlayerEventArgs))[0]),
                new(OpCodes.Stloc_S, local),

                // Events.OnSlappingPlayer(ev);
                new(OpCodes.Ldloc_S, local),
                new(OpCodes.Call, Method(typeof(Scp3114), nameof(Scp3114.OnSlappingPlayer))),
                new(OpCodes.Ldloc_S, local),

                // if (!ev.IsAllowed) return;
                new(OpCodes.Callvirt, PropertyGetter(typeof(SlappingPlayerEventArgs), nameof(SlappingPlayerEventArgs.IsAllowed))),
                new(OpCodes.Brtrue_S, skipLabel),
                new(OpCodes.Ret),

                // DamagePlayer(primaryTarget, ev.DamageAmount);
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_1),
                new(OpCodes.Ldloc_S, local),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SlappingPlayerEventArgs), nameof(SlappingPlayerEventArgs.DamageAmount))),
            };

            CodeInstruction[] injectedInstructions2 = new CodeInstruction[]
            {
                new(OpCodes.Ldloc_S, local),
                new(OpCodes.Callvirt, PropertyGetter(typeof(SlappingPlayerEventArgs), nameof(SlappingPlayerEventArgs.HumeShieldToReward))),
            };
            newInstructions.InsertRange(index, injectedInstructions);
            newInstructions.InsertRange(index2 + injectedInstructions.Length, injectedInstructions2);
            newInstructions.RemoveRange(index2 + injectedInstructions.Length, 1);
            for (int z = 0; z < newInstructions.Count; z++)
            {
                yield return (z == index + 10) ? newInstructions[z] : newInstructions[z].WithLabels(skipLabel);
            }

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}