// -----------------------------------------------------------------------
// <copyright file="PlacingTantrum.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection;
    using System.Reflection.Emit;

    using Exiled.Events.EventArgs;

    using HarmonyLib;

    using Mirror;

    using NorthwoodLib.Pools;

    using UnityEngine;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="PlayableScps.Scp173.ServerDoTantrum"/>.
    /// Adds the <see cref="Handlers.Scp173.PlacingTantrum"/> event.
    /// </summary>
    [HarmonyPatch(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.ServerDoTantrum))]
    internal static class PlacingTantrum
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Shared.Rent(instructions);

            Label returnLabel = newInstructions[newInstructions.Count - 1].labels[0];

            LocalBuilder ev = generator.DeclareLocal(typeof(PlacingTantrumEventArgs));

            int offset = -2;

            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Call &&
            (MethodInfo)instruction.operand == Method(typeof(NetworkServer), nameof(NetworkServer.Spawn), new[] { typeof(GameObject), typeof(NetworkConnection) })) + offset;

            // var ev = new PlacingTantrumEventArgs(this, Player, gameObject, cooldown, true);
            //
            // Handlers.Player.OnPlacingTantrum(ev);
            //
            // if (!ev.IsAllowed)
            //   return;
            //
            // cooldown = ev.Cooldown;
            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // this
                new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                new(OpCodes.Dup),

                // Player.Get(this.Hub)
                new(OpCodes.Ldfld, Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173.Hub))),
                new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                // gameObject
                new(OpCodes.Ldloc_0),

                // cooldown
                new(OpCodes.Ldc_R4, PlayableScps.Scp173.TantrumBaseCooldown),

                // true
                new(OpCodes.Ldc_I4_1),

                // var ev = new PlacingTantrumEventArgs(...)
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

            offset = 1;

            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Call &&
            (MethodInfo)instruction.operand == Method(typeof(NetworkServer), nameof(NetworkServer.Spawn), new[] { typeof(GameObject), typeof(NetworkConnection) })) + offset;

            newInstructions.InsertRange(index, new CodeInstruction[]
            {
                // this._tantrumCooldownRemaining = ev.Cooldown
                new(OpCodes.Ldarg_0),
                new(OpCodes.Ldloc_S, ev.LocalIndex),
                new(OpCodes.Call, PropertyGetter(typeof(PlacingTantrumEventArgs), nameof(PlacingTantrumEventArgs.Cooldown))),
                new(OpCodes.Stfld, Field(typeof(PlayableScps.Scp173), nameof(PlayableScps.Scp173._tantrumCooldownRemaining))),
            });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Shared.Return(newInstructions);
        }
    }
}
