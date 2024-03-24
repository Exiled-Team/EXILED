// -----------------------------------------------------------------------
// <copyright file="Jumping.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Reflection.Emit;

    using API.Features.Core.Generic.Pools;
    using Exiled.Events.Attributes;
    using Exiled.Events.EventArgs.Player;
    using Exiled.Events.Handlers;

    using HarmonyLib;

    using PlayerRoles.FirstPersonControl;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="FpcMotor.UpdateGrounded(ref bool, float)" />
    /// Adds the <see cref="Player.Jumping" /> event.
    /// </summary>
    [EventPatch(typeof(Player), nameof(Player.Jumping))]
    [HarmonyPatch(typeof(FpcMotor), nameof(FpcMotor.UpdateGrounded))]
    internal static class Jumping
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder ev = generator.DeclareLocal(typeof(JumpingEventArgs));

            Label ret = generator.DefineLabel();

            const int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Stfld) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player.Get(this.Hub)
                    new CodeInstruction(OpCodes.Ldarg_0).MoveLabelsFrom(newInstructions[index]),
                    new(OpCodes.Ldfld, Field(typeof(FpcMotor), nameof(FpcMotor.Hub))),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),

                    // moveDir
                    new(OpCodes.Ldloc_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // JumpingEventArgs ev = new(Player, Vector3, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(JumpingEventArgs))[0]),
                    new(OpCodes.Dup),
                    new(OpCodes.Dup),
                    new(OpCodes.Stloc_S, ev.LocalIndex),

                    // Player.OnJumping(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnJumping))),

                    // if (!ev.IsAllowed)
                    //    return;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(JumpingEventArgs), nameof(JumpingEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, ret),

                    // moveDir = ev.Direction
                    new(OpCodes.Ldloc_S, ev.LocalIndex),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(JumpingEventArgs), nameof(JumpingEventArgs.Direction))),
                    new(OpCodes.Stloc_0),
                });

            newInstructions[newInstructions.Count - 1].WithLabels(ret);

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}