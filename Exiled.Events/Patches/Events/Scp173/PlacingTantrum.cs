// -----------------------------------------------------------------------
// <copyright file="PlacingTantrum.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Scp173
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Scp173;

    using HarmonyLib;

    using Mirror;

    using PlayerRoles.PlayableScps.Scp173;
    using PlayerRoles.PlayableScps.Subroutines;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp173TantrumAbility.ServerProcessCmd(NetworkReader)"/>.
    /// Adds the <see cref="Handlers.Scp173.PlacingTantrum"/> event.
    /// </summary>
    [EventPatch(typeof(Handlers.Scp173), nameof(Handlers.Scp173.PlacingTantrum))]
    [HarmonyPatch(typeof(Scp173TantrumAbility), nameof(Scp173TantrumAbility.ServerProcessCmd))]
    internal static class PlacingTantrum
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            Label returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            LocalBuilder ev = generator.DeclareLocal(typeof(PlacingTantrumEventArgs));

            const int offset = -2;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Newobj) + offset;

            // PlacingTantrumEventArgs ev = new(this, Player, gameObject, cooldown, true);
            //
            // Handlers.Player.OnPlacingTantrum(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            //
            // cooldown = ev.Cooldown;
            newInstructions.InsertRange(
                index,
                new CodeInstruction[]
                {
                    // base.ScpRole
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new (OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp173Role>), nameof(ScpStandardSubroutine<Scp173Role>.ScpRole))),

                    // Player.Get(base.Hub)
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Call, PropertyGetter(typeof(ScpStandardSubroutine<Scp173Role>), nameof(ScpStandardSubroutine<Scp173Role>.Owner))),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // tantrumEnvironmentalHazard
                    new(OpCodes.Ldloc_1),

                    // this.Cooldown
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp173TantrumAbility), nameof(Scp173TantrumAbility.Cooldown))),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // PlacingTantrumEventArgs ev = new(Scp173Role, Player, TantrumEnvironmentalHazard, AbilityCooldown, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(PlacingTantrumEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Handlers.Player.OnPlacingTantrum(ev)
                    new(OpCodes.Call, Method(typeof(Handlers.Scp173), nameof(Handlers.Scp173.OnPlacingTantrum))),

                    // if (!ev.IsAllowed)
                    //   return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(PlacingTantrumEventArgs), nameof(PlacingTantrumEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, returnLabel),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}