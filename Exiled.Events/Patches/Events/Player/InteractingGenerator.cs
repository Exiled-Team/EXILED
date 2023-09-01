// -----------------------------------------------------------------------
// <copyright file="InteractingGenerator.cs" company="Exiled Team">
// Copyright (c) Exiled Team. All rights reserved.
// Licensed under the CC BY-SA 3.0 license.
// </copyright>
// -----------------------------------------------------------------------

namespace Exiled.Events.Patches.Events.Player
{
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;
    using System.Reflection.Emit;

    using API.Features.Pools;

    using Exiled.Events.EventArgs.Player;

    using Handlers;

    using HarmonyLib;

    using MapGeneration.Distributors;

    using PluginAPI.Events;

    using static HarmonyLib.AccessTools;

    /// <summary>
    /// Patches <see cref="Scp079Generator.ServerInteract(ReferenceHub, byte)"/>.
    /// Adds the <see cref="Player.ActivatingGenerator"/>, <see cref="Player.ClosingGenerator"/>, <see cref="Player.OpeningGenerator"/>, <see cref="Player.UnlockingGenerator"/> and <see cref="Player.StoppingGenerator"/> events.
    /// </summary>
    [HarmonyPatch(typeof(Scp079Generator), nameof(Scp079Generator.ServerInteract))]
    internal static class InteractingGenerator
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator generator)
        {
            List<CodeInstruction> newInstructions = ListPool<CodeInstruction>.Pool.Get(instructions);

            LocalBuilder player = generator.DeclareLocal(typeof(API.Features.Player));
            LocalBuilder isAllowedUnlocking = generator.DeclareLocal(typeof(bool));

            Label isOpening = generator.DefineLabel();
            Label isActivating = generator.DefineLabel();
            Label check = generator.DefineLabel();
            Label check2 = generator.DefineLabel();
            Label notAllowed = generator.DefineLabel();
            Label skip = generator.DefineLabel();
            Label skip2 = generator.DefineLabel();
            Label @break = newInstructions.FindLast(instruction => instruction.IsLdarg(0)).labels[0];

            int offset = 1;
            int index = newInstructions.FindIndex(instruction => instruction.Calls(Method(typeof(Stopwatch), nameof(Stopwatch.Stop)))) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // Player player = Player.Get(ply)
                    new CodeInstruction(OpCodes.Ldarg_1),
                    new(OpCodes.Call, Method(typeof(API.Features.Player), nameof(API.Features.Player.Get), new[] { typeof(ReferenceHub) })),
                    new(OpCodes.Stloc_S, player.LocalIndex),
                });

            offset = -8;
            index = newInstructions.FindIndex(instruction => instruction.opcode == OpCodes.Newobj && (ConstructorInfo)instruction.operand == GetDeclaredConstructors(typeof(PlayerCloseGeneratorEvent))[0]) + offset;

            // if (this.HasFlag(_flags, GeneratorFlags.Open))
            // {
            //     ClosingGeneratorEventArgs ev = new(player, this, true);
            //
            //     Player.OnClosingGenerator(ev);
            //
            //     if (!ev.IsAllowed)
            //     {
            //         this._targetCooldown = this._unlockCooldownTime;
            //         this.RpcDenied();
            //         break;
            //     }
            // }
            // else
            // {
            //     OpeningGeneratorEventArgs ev = new(player, this, true);
            //
            //     Player.OnOpeningGenerator(ev);
            //
            //     if (!ev.IsAllowed)
            //     {
            //         this._targetCooldown = this._unlockCooldownTime;
            //         this.RpcDenied();
            //         break;
            //     }
            // }
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),

                    // this
                    new(OpCodes.Ldarg_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // if (!this.HasFlag(_flags, GeneratorFlags.Open))
                    //    goto isOpening;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079Generator), nameof(Scp079Generator._flags))),
                    new(OpCodes.Ldc_I4_4), // GeneratorFlags.Open
                    new(OpCodes.Callvirt, Method(typeof(Scp079Generator), nameof(Scp079Generator.HasFlag))),
                    new(OpCodes.Brfalse_S, isOpening),

                    // ClosingGeneratorEventArgs ev = new(Player, Scp079Generator, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(ClosingGeneratorEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnClosingGenerator(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnClosingGenerator))),

                    // ev.IsAllowed
                    //
                    // goto check
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ClosingGeneratorEventArgs), nameof(ClosingGeneratorEventArgs.IsAllowed))),
                    new(OpCodes.Br_S, check),

                    // OpeningGeneratorEventArgs ev = new(Player, Scp079Generator, bool)
                    new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(OpeningGeneratorEventArgs))[0]).WithLabels(isOpening),
                    new(OpCodes.Dup),

                    // Player.OnOpeningGenerator(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnOpeningGenerator))),

                    // loads ev.isAllowed
                    //
                    // if (ev.IsAllowed)
                    //    goto skip;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(OpeningGeneratorEventArgs), nameof(OpeningGeneratorEventArgs.IsAllowed))),
                    new CodeInstruction(OpCodes.Brtrue_S, skip).WithLabels(check),

                    // notAllowed:
                    //
                    // this._targetCooldown = this._unlockCooldownTime;
                    new CodeInstruction(OpCodes.Ldarg_0).WithLabels(notAllowed),
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Ldfld, Field(typeof(Scp079Generator), nameof(Scp079Generator._unlockCooldownTime))),
                    new(OpCodes.Stfld, Field(typeof(Scp079Generator), nameof(Scp079Generator._targetCooldown))),

                    // this.RpcDenied()
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, Method(typeof(Scp079Generator), nameof(Scp079Generator.RpcDenied))),

                    // goto @break
                    new(OpCodes.Br_S, @break),

                    // skip:
                    new CodeInstruction(OpCodes.Nop).WithLabels(skip),
                });

            offset = -5;
            index = newInstructions.FindLastIndex(instruction => instruction.Calls(Method(typeof(Scp079Generator), nameof(Scp079Generator.RpcDenied)))) + offset;

            newInstructions.RemoveRange(index, 7);

            offset = -2;
            int index2 = newInstructions.FindLastIndex(instruction => instruction.opcode == OpCodes.Newobj && (ConstructorInfo)instruction.operand == GetDeclaredConstructors(typeof(PlayerUnlockGeneratorEvent))[0]) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // isAllowed var set
                    new(OpCodes.Ldc_I4_0),
                    new(OpCodes.Br_S, skip2),
                    new CodeInstruction(OpCodes.Ldc_I4_1).MoveLabelsFrom(newInstructions[index2]),
                    new CodeInstruction(OpCodes.Stloc_S, isAllowedUnlocking.LocalIndex).WithLabels(skip2),

                    // player
                    new(OpCodes.Ldloc_S, player.LocalIndex),

                    // this
                    new(OpCodes.Ldarg_0),

                    // isAllowed
                    new(OpCodes.Ldloc_S, isAllowedUnlocking.LocalIndex),

                    // UnlockingGeneratorEventArgs ev = new(Player, Scp079Generator, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(UnlockingGeneratorEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnUnlockingGenerator(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnUnlockingGenerator))),

                    // if (!ev.IsAllowed)
                    //    goto notAllowed;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(UnlockingGeneratorEventArgs), nameof(UnlockingGeneratorEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, notAllowed),
                });

            offset = -5;
            index = newInstructions.FindIndex(i => i.Calls(PropertySetter(typeof(Scp079Generator), nameof(Scp079Generator.Activating)))) + offset;

            // if (this.Activating)
            // {
            //     StoppingGeneratorEventArgs ev = new(player, this, true);
            //
            //     Player.OnStoppingGenerator(ev);
            //
            //     if (!ev.IsAllowed)
            //     {
            //         this._targetCooldown = this._unlockCooldownTime;
            //         this.RpcDenied();
            //         break;
            //     }
            // }
            // else
            // {
            //     ActivatingGeneratorEventArgs ev = new(player, this, true);
            //
            //     Player.OnActivatingGenerator(ev);
            //
            //     if (!ev.IsAllowed)
            //     {
            //         this._targetCooldown = this._unlockCooldownTime;
            //         this.RpcDenied();
            //         break;
            //     }
            // }
            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex).MoveLabelsFrom(newInstructions[index]),

                    // this
                    new(OpCodes.Ldarg_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // if (!this.Activating)
                    //    goto isActivating;
                    new(OpCodes.Ldarg_0),
                    new(OpCodes.Callvirt, PropertyGetter(typeof(Scp079Generator), nameof(Scp079Generator.Activating))),
                    new(OpCodes.Brfalse_S, isActivating),

                    // StoppingGeneratorEventArgs ev = new(Player, Scp079Generator, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StoppingGeneratorEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnStoppingGenerator(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnStoppingGenerator))),

                    // ev.IsAllowed
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StoppingGeneratorEventArgs), nameof(StoppingGeneratorEventArgs.IsAllowed))),
                    new(OpCodes.Br_S, check2),

                    // isActivating:
                    //
                    // ActivatingGeneratorEventArgs ev = new(Player, Scp079Generator, bool)
                    new CodeInstruction(OpCodes.Newobj, GetDeclaredConstructors(typeof(ActivatingGeneratorEventArgs))[0]).WithLabels(isActivating),
                    new(OpCodes.Dup),

                    // Player.OnActivatingGenerator(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnActivatingGenerator))),

                    // if (!ev.IsAllowed)
                    //    goto notAllowed;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(ActivatingGeneratorEventArgs), nameof(ActivatingGeneratorEventArgs.IsAllowed))),
                    new CodeInstruction(OpCodes.Brfalse_S, notAllowed).WithLabels(check2),
                });

            offset = 1;
            index = newInstructions.FindLastIndex(i => i.opcode == OpCodes.Brfalse_S) + offset;

            newInstructions.InsertRange(
                index,
                new[]
                {
                    // player
                    new CodeInstruction(OpCodes.Ldloc_S, player.LocalIndex),

                    // this
                    new(OpCodes.Ldarg_0),

                    // true
                    new(OpCodes.Ldc_I4_1),

                    // StoppingGeneratorEventArgs ev = new(Player, Scp079Generator, bool)
                    new(OpCodes.Newobj, GetDeclaredConstructors(typeof(StoppingGeneratorEventArgs))[0]),
                    new(OpCodes.Dup),

                    // Player.OnStoppingGenerator(ev)
                    new(OpCodes.Call, Method(typeof(Player), nameof(Player.OnStoppingGenerator))),

                    // if (!ev.IsAllowed)
                    //     return notAllowed;
                    new(OpCodes.Callvirt, PropertyGetter(typeof(StoppingGeneratorEventArgs), nameof(StoppingGeneratorEventArgs.IsAllowed))),
                    new(OpCodes.Brfalse_S, notAllowed),
                });

            for (int z = 0; z < newInstructions.Count; z++)
                yield return newInstructions[z];

            ListPool<CodeInstruction>.Pool.Return(newInstructions);
        }
    }
}